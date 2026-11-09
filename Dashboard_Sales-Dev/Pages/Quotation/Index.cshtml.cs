using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Quotation;

public class Index : PageModel
{
    private readonly QuotationService _quotationService;

    public Index(QuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    public List<Logic.Models.Quotation> Quotations { get; private set; } = [];

    public void OnGet()
    {
        Quotations = _quotationService.GetAll();
    }
}
