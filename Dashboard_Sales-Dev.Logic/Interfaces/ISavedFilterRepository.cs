using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface ISavedFilterRepository
{
    List<SavedFilter> GetForEmployee(int employeeId);
    Task AddAsync(SavedFilter filter);
    Task DeleteAsync(int employeeId, int filterId);
}
