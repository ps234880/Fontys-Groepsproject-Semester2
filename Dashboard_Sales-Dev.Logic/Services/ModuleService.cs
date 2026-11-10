using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class ModuleService
{
    private readonly IModuleRepository _moduleRepository;

    public ModuleService(IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public List<Module> GetAll()
    {
        return _moduleRepository.GetAll();
    }

    public Module? GetById(int id)
    {
        return _moduleRepository.GetById(id);
    }

    public Task AddModuleAsync(Module module)
    {
        return _moduleRepository.AddModuleAsync(module);
    }

    public Task<bool> UpdateModuleAsync(Module module)
    {
        return _moduleRepository.UpdateModuleAsync(module);
    }

    public Task<bool> DeleteModuleAsync(int id)
    {
        return _moduleRepository.DeleteModuleAsync(id);
    }
}
