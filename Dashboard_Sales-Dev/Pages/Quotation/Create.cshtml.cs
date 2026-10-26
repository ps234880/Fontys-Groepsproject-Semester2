using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard_Sales_Dev.Models;
using Dashboard_Sales_Dev.Repository;

namespace Dashboard_Sales_Dev.Pages.Quotation
{
    public class CreateModel : PageModel
    {
        private readonly ClientRepository _clientRepository;
        private readonly QuotationRepository _quotationRepository;
        
        public CreateModel(ClientRepository clientRepository, QuotationRepository quotationRepository)
        {
            _clientRepository = clientRepository;
            _quotationRepository = quotationRepository;
        }

        [BindProperty]
        public Models.Client Client { get; set; } = default!;

        [BindProperty]
        public Models.Quotation Quotation { get; set; } = default!;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create client and retrieve its generated ID
            int clientId = await _clientRepository.AddClientAsync(Client);

            // Assign the ID using the property your repository expects
            Quotation.ClientId = clientId;
            Quotation.EmployeeId = 1;

            // Save quotation
            await _quotationRepository.AddQuotationAsync(Quotation);

            return RedirectToPage("./Index");
        }
    }
}