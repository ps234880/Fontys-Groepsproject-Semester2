using Dashboard_Sales_Dev.Logic.Exceptions;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
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

    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
        Modules = _moduleService.GetAll();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _moduleService.DeleteModuleAsync(id);
        }
        catch (ModuleInUseException)
        {
            ErrorMessage = "Deze module kan niet worden verwijderd, omdat hij nog aan een offerte of order gekoppeld is.";
            Modules = _moduleService.GetAll();

            return Page();
        }

        return RedirectToPage();
    }
}
