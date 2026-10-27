using Dashboard_Sales_Dev.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Module;

public class Index : PageModel
{
    private readonly ModuleRepository _module;

    public Index(ModuleRepository repository)
    {
        _module = repository;
    }

    public List<Models.Module> Modules { get; private set; } = [];

    public void OnGet()
    {
        Modules = _module.GetAll();
    }
}