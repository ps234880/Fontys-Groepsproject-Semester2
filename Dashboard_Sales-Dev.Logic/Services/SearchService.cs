using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class SearchService
{
    public const int MaxQueryLength = 100;

    private readonly IDealRepository _dealRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IModuleRepository _moduleRepository;

    public SearchService(IDealRepository dealRepository, IClientRepository clientRepository, IModuleRepository moduleRepository)
    {
        _dealRepository = dealRepository;
        _clientRepository = clientRepository;
        _moduleRepository = moduleRepository;
    }

    public SearchResults Search(string? query)
    {
        string term = (query ?? string.Empty).Trim();

        if (term.Length == 0)
        {
            return SearchResults.Empty;
        }

        if (term.Length > MaxQueryLength)
        {
            term = term[..MaxQueryLength];
        }

        List<Deal> deals = _dealRepository.GetAll()
            .Where(deal => Matches(term, deal.ClientCompanyName, deal.Title, deal.ClientName, deal.EmployeeName))
            .ToList();

        List<Client> clients = _clientRepository.GetAll()
            .Where(client => Matches(term, client.CompanyName, client.FirstName + " " + client.LastName, client.Email))
            .OrderBy(client => client.CompanyName)
            .ToList();

        List<Module> modules = _moduleRepository.GetAll()
            .Where(module => Matches(term, module.Name, module.Description))
            .OrderBy(module => module.Name)
            .ToList();

        return new SearchResults(deals, clients, modules);
    }

    private static bool Matches(string term, params string[] values)
    {
        return values.Any(value => value.Contains(term, StringComparison.CurrentCultureIgnoreCase));
    }
}
