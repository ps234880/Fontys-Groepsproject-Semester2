using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IModuleRepository
{
    List<Module> GetAll();
    Module? GetById(int id);
    Task AddModuleAsync(Module module);
    Task<bool> UpdateModuleAsync(Module module);
    Task<bool> DeleteModuleAsync(int id);
}
