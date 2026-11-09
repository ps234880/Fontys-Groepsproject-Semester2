using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Module;

public class Index : PageModel
{
    private readonly ModuleService _moduleService;

    public Index(ModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    public List<Logic.Models.Module> Modules { get; private set; } = [];

    public void OnGet()
    {
        Modules = _moduleService.GetAll();
    }
}
