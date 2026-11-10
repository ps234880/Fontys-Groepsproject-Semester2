using Dashboard_Sales_Dev.Logic.Exceptions;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages;

public class ModuleLibraryModel : PageModel
{
    private readonly ModuleService _moduleService;

    public ModuleLibraryModel(ModuleService moduleService)
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
            ErrorMessage = "This module cannot be deleted, because it is still linked to a deal or an order.";
            Modules = _moduleService.GetAll();

            return Page();
        }

        return RedirectToPage();
    }
}
