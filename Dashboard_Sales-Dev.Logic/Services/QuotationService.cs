using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class QuotationService
{
    private readonly IQuotationRepository _quotationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public QuotationService(IQuotationRepository quotationRepository, IClientRepository clientRepository, IEmployeeRepository employeeRepository)
    {
        _quotationRepository = quotationRepository;
        _clientRepository = clientRepository;
        _employeeRepository = employeeRepository;
    }

    public List<Quotation> GetAll()
    {
        return _quotationRepository.GetAll();
    }

    public Quotation? GetById(int id)
    {
        return _quotationRepository.GetById(id);
    }

    public async Task CreateWithNewClientAsync(Client client, Quotation quotation)
    {
        Employee? owner = _employeeRepository.GetById(quotation.EmployeeId);

        if (owner == null || owner.RoleName != EmployeeService.SalesRole)
        {
            throw new ArgumentException("The owner of a deal must be a Sales employee.");
        }

        int clientId = await _clientRepository.AddClientAsync(client);

        quotation.ClientId = clientId;

        await _quotationRepository.AddQuotationAsync(quotation);
    }

    public List<Module> GetModules(int quotationId)
    {
        return _quotationRepository.GetModules(quotationId);
    }

    public async Task LinkModuleAsync(int quotationId, int moduleId)
    {
        bool alreadyLinked = GetModules(quotationId).Any(module => module.Id == moduleId);

        if (alreadyLinked)
        {
            return;
        }

        await _quotationRepository.AddModuleAsync(quotationId, moduleId);
    }

    public Task UnlinkModuleAsync(int quotationId, int moduleId)
    {
        return _quotationRepository.RemoveModuleAsync(quotationId, moduleId);
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
