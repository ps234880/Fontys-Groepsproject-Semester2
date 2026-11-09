using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IModuleRepository
{
    List<Module> GetAll();
    Task AddModuleAsync(Module module);
}
