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

    public DealService(IDealRepository dealRepository, IClientRepository clientRepository,
        IEmployeeRepository employeeRepository, IStatusRepository statusRepository)
    {
        _dealRepository = dealRepository;
        _clientRepository = clientRepository;
        _employeeRepository = employeeRepository;
        _statusRepository = statusRepository;
    }

    public List<Deal> GetAll()
    {
        return _dealRepository.GetAll();
    }

    public Deal? GetById(int id)
    {
        return _dealRepository.GetById(id);
    }

    public async Task<int> CreateWithNewClientAsync(Client client, Deal deal)
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

        int clientId = await _clientRepository.AddClientAsync(client);

        deal.ClientId = clientId;
        deal.StatusId = newStatus.Id;

        return await _dealRepository.AddDealAsync(deal);
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

    public async Task SetModulePriceAsync(int dealId, int moduleId, int? priceInEuros)
    {
        if (priceInEuros < 0 || priceInEuros > int.MaxValue / 100)
        {
            throw new ArgumentException($"Enter a deal price between 0 and {int.MaxValue / 100} euros.");
        }

        if (!GetModules(dealId).Any(module => module.Id == moduleId))
        {
            throw new ArgumentException("This module is not linked to the deal.");
        }

        await _dealRepository.UpdateModulePriceAsync(dealId, moduleId, priceInEuros * 100);
    }

    public int GetTotalPriceInCents(IEnumerable<DealModule> modules)
    {
        return modules.Sum(module => module.FinalPriceInCents);
    }

    public int GetProductionTimeInWeeks(IEnumerable<Module> modules)
    {
        return modules.Sum(module => module.ProductionTimeInWeeks);
    }

    public DateTime GetEarliestDeliveryDate(IEnumerable<Module> modules, DateTime today)
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
}