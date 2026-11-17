using System.ComponentModel.DataAnnotations;
using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class DealService
{
    public const string NewStatus = "New";
    public const string WonStatus = "Won";
    public const string LostStatus = "Lost";

    private readonly IDealRepository _dealRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly IModuleRepository _moduleRepository;

    public DealService(IDealRepository dealRepository, IClientRepository clientRepository,
        IEmployeeRepository employeeRepository, IStatusRepository statusRepository, IModuleRepository moduleRepository)
    {
        _dealRepository = dealRepository;
        _clientRepository = clientRepository;
        _employeeRepository = employeeRepository;
        _statusRepository = statusRepository;
        _moduleRepository = moduleRepository;
    }

    public List<Deal> GetAll()
    {
        return _dealRepository.GetAll();
    }

    public Deal? GetById(int id)
    {
        return _dealRepository.GetById(id);
    }

    public const int MaxDescriptionLength = 255;

    public async Task<int> CreateWithNewClientAsync(Client client, Deal deal, List<string> goals, List<DealScopeItem> scopeItems)
    {
        (List<string> cleanGoals, List<DealScopeItem> cleanScopeItems) = CleanGoalsAndScope(goals, scopeItems);
        int statusId = GetStatusIdForNewDeal(deal);

        deal.ClientId = await _clientRepository.AddClientAsync(client);
        deal.StatusId = statusId;

        return await _dealRepository.AddDealAsync(deal, cleanGoals, cleanScopeItems);
    }

    public async Task<int> CreateWithExistingClientAsync(int clientId, Deal deal, List<string> goals, List<DealScopeItem> scopeItems)
    {
        if (_clientRepository.GetById(clientId) == null)
        {
            throw new ArgumentException($"Customer {clientId} does not exist. Choose another customer.");
        }

        (List<string> cleanGoals, List<DealScopeItem> cleanScopeItems) = CleanGoalsAndScope(goals, scopeItems);

        deal.ClientId = clientId;
        deal.StatusId = GetStatusIdForNewDeal(deal);

        return await _dealRepository.AddDealAsync(deal, cleanGoals, cleanScopeItems);
    }

    public List<Client> GetAllClients()
    {
        return _clientRepository.GetAll()
            .OrderBy(client => client.CompanyName)
            .ThenBy(client => client.LastName)
            .ToList();
    }

    private (List<string>, List<DealScopeItem>) CleanGoalsAndScope(List<string> goals, List<DealScopeItem> scopeItems)
    {
        List<string> cleanGoals = goals.Select(goal => CleanDescription(goal, "goal")).ToList();
        List<DealScopeItem> cleanScopeItems = scopeItems
            .Select(item => new DealScopeItem { Description = CleanDescription(item.Description, "scope item"), InScope = item.InScope })
            .ToList();

        if (cleanGoals.Count == 0)
        {
            throw new ArgumentException("Add at least one goal for the project.");
        }

        return (cleanGoals, cleanScopeItems);
    }

    private int GetStatusIdForNewDeal(Deal deal)
    {
        Employee? owner = _employeeRepository.GetById(deal.EmployeeId);

        if (owner == null || owner.RoleName != EmployeeService.SalesRole)
        {
            throw new ArgumentException("The owner of a deal must be a Sales employee.");
        }

        Status? newStatus = _statusRepository.GetByName(NewStatus);

        if (newStatus == null)
        {
            throw new InvalidOperationException($"The status '{NewStatus}' is missing from the database.");
        }

        return newStatus.Id;
    }

    public List<DealGoal> GetGoals(int dealId)
    {
        return _dealRepository.GetGoals(dealId);
    }

    public Task AddGoalAsync(int dealId, string? description)
    {
        return _dealRepository.AddGoalAsync(dealId, CleanDescription(description, "goal"));
    }

    public Task RemoveGoalAsync(int dealId, int goalId)
    {
        return _dealRepository.RemoveGoalAsync(dealId, goalId);
    }

    public List<DealScopeItem> GetScopeItems(int dealId)
    {
        return _dealRepository.GetScopeItems(dealId);
    }

    public Task AddScopeItemAsync(int dealId, string? description, bool inScope)
    {
        return _dealRepository.AddScopeItemAsync(dealId, CleanDescription(description, "scope item"), inScope);
    }

    public Task SetScopeItemInScopeAsync(int dealId, int itemId, bool inScope)
    {
        return _dealRepository.UpdateScopeItemAsync(dealId, itemId, inScope);
    }

    public Task RemoveScopeItemAsync(int dealId, int itemId)
    {
        return _dealRepository.RemoveScopeItemAsync(dealId, itemId);
    }

    private static string CleanDescription(string? description, string kind)
    {
        string trimmed = (description ?? string.Empty).Trim();

        if (trimmed.Length == 0)
        {
            throw new ArgumentException($"Describe the {kind}.");
        }

        if (trimmed.Length > MaxDescriptionLength)
        {
            throw new ArgumentException($"Keep each {kind} under {MaxDescriptionLength} characters. This one has {trimmed.Length}: '{trimmed[..40]}...'");
        }

        return trimmed;
    }

    public async Task CreateContactAsync(int dealId, Client contact)
    {
        int clientId = await _clientRepository.AddClientAsync(contact);
        await _dealRepository.AddClientAsync(dealId, clientId);
    }

    public List<Status> GetStatuses()
    {
        return _statusRepository.GetAll();
    }

    public async Task ChangeStatusAsync(int dealId, int statusId)
    {
        if (_statusRepository.GetById(statusId) == null)
        {
            throw new ArgumentException("Choose a valid stage for this deal.");
        }

        await _dealRepository.UpdateStatusAsync(dealId, statusId);
    }

    public static readonly string[] WaitingOnRoles = [EmployeeService.SalesRole, EmployeeService.DevelopmentRole];

    public async Task ChangeWaitingOnAsync(int dealId, string? roleName)
    {
        string? role = string.IsNullOrWhiteSpace(roleName) ? null : roleName;

        if (role != null && !WaitingOnRoles.Contains(role))
        {
            throw new ArgumentException($"'{roleName}' is not a team that can be waited on. Choose Sales or Development.");
        }

        await _dealRepository.UpdateWaitingOnAsync(dealId, role);
    }

    public bool IsOpen(Deal deal)
    {
        return deal.StatusName != WonStatus && deal.StatusName != LostStatus;
    }

    public List<DealOverview> GetOpenDealOverviews(DateTime today)
    {
        List<DealOverview> overviews = new List<DealOverview>();

        foreach (Deal deal in GetAll().Where(IsOpen))
        {
            List<DealModule> modules = GetModules(deal.Id);
            DealProgress progress = DealProgress.OnTrack;

            if (modules.Count == 0)
            {
                progress = DealProgress.NoModules;
            }
            else if (deal.EstimatedDate.Date < GetEarliestDeliveryDate(modules, today))
            {
                progress = DealProgress.DateTooEarly;
            }

            overviews.Add(new DealOverview(deal, modules.Count, GetProductionTimeInWeeks(modules), progress));
        }

        return overviews;
    }

    public List<DealModule> GetModules(int dealId)
    {
        return _dealRepository.GetModules(dealId);
    }

    public async Task LinkModuleAsync(int dealId, int moduleId)
    {
        bool alreadyLinked = GetModules(dealId).Any(module => module.Id == moduleId);

        if (alreadyLinked)
        {
            return;
        }

        await _dealRepository.AddModuleAsync(dealId, moduleId);
    }

    public Task UnlinkModuleAsync(int dealId, int moduleId)
    {
        return _dealRepository.RemoveModuleAsync(dealId, moduleId);
    }

    public const int MaxDealProductionTimeInWeeks = 520;

    public async Task UpdateDealModuleAsync(int dealId, int moduleId, int? priceInEuros, int? productionTimeInWeeks)
    {
        if (priceInEuros < 0 || priceInEuros > int.MaxValue / 100)
        {
            throw new ArgumentException($"Enter a deal price between 0 and {int.MaxValue / 100} euros.");
        }

        if (productionTimeInWeeks < 1 || productionTimeInWeeks > MaxDealProductionTimeInWeeks)
        {
            throw new ArgumentException($"Enter a production time between 1 and {MaxDealProductionTimeInWeeks} weeks.");
        }

        if (!GetModules(dealId).Any(module => module.Id == moduleId))
        {
            throw new ArgumentException("This module is not linked to the deal.");
        }

        await _dealRepository.UpdateDealModuleAsync(dealId, moduleId, priceInEuros * 100, productionTimeInWeeks);
    }

    public async Task<int> CreateModuleForDealAsync(int dealId, Module module)
    {
        module.Name = (module.Name ?? string.Empty).Trim();
        module.Description = (module.Description ?? string.Empty).Trim();

        List<ValidationResult> errors = new List<ValidationResult>();

        if (!Validator.TryValidateObject(module, new ValidationContext(module), errors, validateAllProperties: true))
        {
            throw new ArgumentException(string.Join(" ", errors.Select(error => error.ErrorMessage)));
        }

        int moduleId = await _moduleRepository.AddModuleAsync(module);
        await _dealRepository.AddModuleAsync(dealId, moduleId);

        return moduleId;
    }

    public int GetTotalPriceInCents(IEnumerable<DealModule> modules)
    {
        return modules.Sum(module => module.FinalPriceInCents);
    }

    public int GetProductionTimeInWeeks(IEnumerable<DealModule> modules)
    {
        return modules.Sum(module => module.FinalProductionTimeInWeeks);
    }

    public DateTime GetEarliestDeliveryDate(IEnumerable<DealModule> modules, DateTime today)
    {
        return today.Date.AddDays(7 * GetProductionTimeInWeeks(modules));
    }

    public List<Client> GetContacts(int dealId)
    {
        return _dealRepository.GetClients(dealId);
    }

    public List<Client> GetAvailableContacts(Deal deal)
    {
        HashSet<int> linkedIds = GetContacts(deal.Id).Select(client => client.Id).ToHashSet();

        return _clientRepository.GetAll()
            .Where(client => client.Id != deal.ClientId && !linkedIds.Contains(client.Id))
            .OrderBy(client => client.CompanyName)
            .ThenBy(client => client.LastName)
            .ToList();
    }

    public async Task LinkContactAsync(Deal deal, int clientId)
    {
        if (clientId == deal.ClientId || GetContacts(deal.Id).Any(client => client.Id == clientId))
        {
            return;
        }

        await _dealRepository.AddClientAsync(deal.Id, clientId);
    }

    public Task UnlinkContactAsync(int dealId, int clientId)
    {
        return _dealRepository.RemoveClientAsync(dealId, clientId);
    }

    public async Task RemovePrimaryContactAsync(Deal deal)
    {
        Client? next = GetContacts(deal.Id).FirstOrDefault();

        if (next == null)
        {
            return;
        }

        await _dealRepository.ReplacePrimaryClientAsync(deal.Id, next.Id);
    }

    public List<Employee> GetTeamMembers(int dealId)
    {
        return _dealRepository.GetTeamMembers(dealId);
    }

    public List<Employee> GetAvailableTeamMembers(Deal deal)
    {
        HashSet<int> memberIds = GetTeamMembers(deal.Id).Select(employee => employee.Id).ToHashSet();

        return _employeeRepository.GetAll()
            .Where(employee => employee.Id != deal.EmployeeId && !memberIds.Contains(employee.Id))
            .ToList();
    }

    public async Task AddTeamMemberAsync(Deal deal, int employeeId)
    {
        if (employeeId <= 0)
        {
            throw new ArgumentException("Choose a colleague to add to the team.");
        }

        if (employeeId == deal.EmployeeId)
        {
            throw new ArgumentException("The owner is already part of the team.");
        }

        if (_employeeRepository.GetById(employeeId) == null)
        {
            throw new ArgumentException($"Employee {employeeId} does not exist. Choose another team member.");
        }

        await _dealRepository.AddTeamMemberAsync(deal.Id, employeeId);
    }

    public Task RemoveTeamMemberAsync(int dealId, int employeeId)
    {
        return _dealRepository.RemoveTeamMemberAsync(dealId, employeeId);
    }
}
