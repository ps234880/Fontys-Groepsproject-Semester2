using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class QuotationService
{
    // There is no login yet, so every quotation is assigned to employee 1.
    private const int DefaultEmployeeId = 1;

    private readonly IQuotationRepository _quotationRepository;
    private readonly IClientRepository _clientRepository;

    public QuotationService(IQuotationRepository quotationRepository, IClientRepository clientRepository)
    {
        _quotationRepository = quotationRepository;
        _clientRepository = clientRepository;
    }

    public List<Quotation> GetAll()
    {
        return _quotationRepository.GetAll();
    }

    public async Task CreateWithNewClientAsync(Client client, Quotation quotation)
    {
        int clientId = await _clientRepository.AddClientAsync(client);

        quotation.ClientId = clientId;
        quotation.EmployeeId = DefaultEmployeeId;

        await _quotationRepository.AddQuotationAsync(quotation);
    }
}
