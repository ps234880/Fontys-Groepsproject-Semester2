using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Deal = Dashboard_Sales_Dev.Logic.Models.Quotation;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class IndexModel : PageModel
{
    private readonly QuotationService _quotationService;

    public IndexModel(QuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    public List<Deal> Deals { get; private set; } = [];

    public void OnGet()
    {
        Deals = _quotationService.GetAll();
    }
}
