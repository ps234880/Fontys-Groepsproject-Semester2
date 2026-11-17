namespace Dashboard_Sales_Dev.Logic.Models;

public record SearchResults(List<Deal> Deals, List<Client> Clients, List<Module> Modules)
{
    public static SearchResults Empty => new([], [], []);

    public int Count => Deals.Count + Clients.Count + Modules.Count;
}
