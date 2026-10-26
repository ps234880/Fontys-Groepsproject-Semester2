using Dashboard_Sales_Dev.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dashboard_Sales_Dev.Models;

namespace Dashboard_Sales_Dev.Pages.Quotation;

public class Index : PageModel
{
    private readonly QuotationRepository _repository;

    // 1. Vraag de repository op via de constructor
    public Index(QuotationRepository repository)
    {
        _repository = repository;
    }

    // 2. De lijst die je HTML-pagina straks uitleest
    public List<Models.Quotation> Quotations { get; private set; } = [];

    // 3. Haal de data op zodra de pagina geladen wordt
    public void OnGet()
    {
        Quotations = _repository.GetAll();
    }
}