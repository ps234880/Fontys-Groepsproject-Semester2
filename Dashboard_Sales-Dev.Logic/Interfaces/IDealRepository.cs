using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IDealRepository
{
    List<Deal> GetAll();
    Deal? GetById(int id);
    Task<int> AddDealAsync(Deal deal);
    Task UpdateStatusAsync(int dealId, int statusId);

    List<DealModule> GetModules(int dealId);
    Task AddModuleAsync(int dealId, int moduleId);
    Task RemoveModuleAsync(int dealId, int moduleId);
    Task UpdateDealModuleAsync(int dealId, int moduleId, int? priceInCents, int? productionTimeInWeeks);

    List<Client> GetClients(int dealId);
    Task AddClientAsync(int dealId, int clientId);
    Task RemoveClientAsync(int dealId, int clientId);
    Task ReplacePrimaryClientAsync(int dealId, int newClientId);
}