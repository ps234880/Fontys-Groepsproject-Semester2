using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Module;

public class EditModel : PageModel
{
    private readonly ModuleService _moduleService;

    public EditModel(ModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [BindProperty]
    public Logic.Models.Module Module { get; set; } = default!;

    public IActionResult OnGet(int id)
    {
        Logic.Models.Module? module = _moduleService.GetById(id);

        if (module == null)
        {
            return NotFound();
        }

        Module = module;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Module.Id = id;

        bool updated = await _moduleService.UpdateModuleAsync(Module);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToPage("/ModuleLibrary");
    }
}
