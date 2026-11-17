using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IDealRepository
{
    List<Deal> GetAll();
    Deal? GetById(int id);
    Task<int> AddDealAsync(Deal deal, IReadOnlyList<string> goals, IReadOnlyList<DealScopeItem> scopeItems);
    Task UpdateStatusAsync(int dealId, int statusId);
    Task UpdateWaitingOnAsync(int dealId, string? roleName);

    List<DealGoal> GetGoals(int dealId);
    Task AddGoalAsync(int dealId, string description);
    Task RemoveGoalAsync(int dealId, int goalId);

    List<DealScopeItem> GetScopeItems(int dealId);
    Task AddScopeItemAsync(int dealId, string description, bool inScope);
    Task UpdateScopeItemAsync(int dealId, int itemId, bool inScope);
    Task RemoveScopeItemAsync(int dealId, int itemId);

    List<DealModule> GetModules(int dealId);
    Task AddModuleAsync(int dealId, int moduleId);
    Task RemoveModuleAsync(int dealId, int moduleId);
    Task UpdateDealModuleAsync(int dealId, int moduleId, int? priceInCents, int? productionTimeInWeeks);

    List<Client> GetClients(int dealId);
    Task AddClientAsync(int dealId, int clientId);
    Task RemoveClientAsync(int dealId, int clientId);
    Task ReplacePrimaryClientAsync(int dealId, int newClientId);

    List<Employee> GetTeamMembers(int dealId);
    Task AddTeamMemberAsync(int dealId, int employeeId);
    Task RemoveTeamMemberAsync(int dealId, int employeeId);
}