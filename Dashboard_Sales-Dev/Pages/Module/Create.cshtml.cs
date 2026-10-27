using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard_Sales_Dev.Models;
using Dashboard_Sales_Dev.Repository;

namespace Dashboard_Sales_Dev.Pages.Module
{
    public class CreateModel : PageModel
    {
        private readonly ModuleRepository _moduleRepository;
        
        public CreateModel(ModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }

        [BindProperty]
        public Models.Module Module { get; set; } = default!;

      
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _moduleRepository.AddModuleAsync(Module);

            return RedirectToPage("./Index");
        }
    }
}