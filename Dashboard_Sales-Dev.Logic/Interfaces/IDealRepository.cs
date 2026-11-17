using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IDealRepository
{
    List<Deal> GetAll();
    Deal? GetById(int id);
    Task<int> AddDealAsync(Deal deal);
    Task UpdateStatusAsync(int dealId, int statusId);

    List<Module> GetModules(int dealId);
    Task AddModuleAsync(int dealId, int moduleId);
    Task RemoveModuleAsync(int dealId, int moduleId);

    List<Client> GetClients(int dealId);
    Task AddClientAsync(int dealId, int clientId);
    Task RemoveClientAsync(int dealId, int clientId);
    Task ReplacePrimaryClientAsync(int dealId, int newClientId);

    List<Employee> GetTeamMembers(int dealId);
    Task AddTeamMemberAsync(int dealId, int employeeId);
    Task RemoveTeamMemberAsync(int dealId, int employeeId);
}