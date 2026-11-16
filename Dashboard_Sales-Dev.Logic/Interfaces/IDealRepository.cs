using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IDealRepository
{
    List<Deal> GetAll();
    Deal? GetById(int id);
    Task AddDealAsync(Deal deal);
    Task UpdateStatusAsync(int dealId, int statusId);
    List<Module> GetModules(int dealId);
    Task AddModuleAsync(int dealId, int moduleId);
    Task RemoveModuleAsync(int dealId, int moduleId);
}
