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

    public DealService(IDealRepository dealRepository, IClientRepository clientRepository, IEmployeeRepository employeeRepository, IStatusRepository statusRepository)
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

    public async Task CreateWithNewClientAsync(Client client, Deal deal)
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

        await _dealRepository.AddDealAsync(deal);
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
            List<Module> modules = GetModules(deal.Id);
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

    public List<Module> GetModules(int dealId)
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

    public int GetProductionTimeInWeeks(List<Module> modules)
    {
        return modules.Sum(module => module.ProductionTimeInWeeks);
    }

    public DateTime GetEarliestDeliveryDate(List<Module> modules, DateTime today)
    {
        return today.Date.AddDays(7 * GetProductionTimeInWeeks(modules));
    }
}
