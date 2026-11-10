using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Module;

public class CreateModel : PageModel
{
    private readonly ModuleService _moduleService;

    public CreateModel(ModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [BindProperty]
    public Logic.Models.Module Module { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _moduleService.AddModuleAsync(Module);

        return RedirectToPage("/ModuleLibrary");
    }
}
