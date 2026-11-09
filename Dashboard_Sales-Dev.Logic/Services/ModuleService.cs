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

    public Task AddModuleAsync(Module module)
    {
        return _moduleRepository.AddModuleAsync(module);
    }
}
