using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Quotation;

public class CreateModel : PageModel
{
    private readonly QuotationService _quotationService;

    public CreateModel(QuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    [BindProperty]
    public Logic.Models.Client Client { get; set; } = default!;

    [BindProperty]
    public Logic.Models.Quotation Quotation { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _quotationService.CreateWithNewClientAsync(Client, Quotation);

        return RedirectToPage("./Index");
    }
}
