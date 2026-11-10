using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Deal = Dashboard_Sales_Dev.Logic.Models.Quotation;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class DetailModel : PageModel
{
    private readonly QuotationService _quotationService;

    public DetailModel(QuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    public Deal Deal { get; private set; } = default!;

    public IActionResult OnGet(int id)
    {
        Deal? deal = _quotationService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        Deal = deal;

        return Page();
    }
}
