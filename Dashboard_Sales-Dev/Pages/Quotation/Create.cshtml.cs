using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard_Sales_Dev.Models;
using Dashboard_Sales_Dev.Repository;

namespace Dashboard_Sales_Dev.Pages.Quotation
{
    public class CreateModel : PageModel
    {
        private readonly ClientRepository _clientRepository;
        
        public CreateModel(ClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Gebruik de nieuwe repository om op te slaan
            await _clientRepository.AddClientAsync(Client);

            return RedirectToPage("./Index");
        }
    }
}