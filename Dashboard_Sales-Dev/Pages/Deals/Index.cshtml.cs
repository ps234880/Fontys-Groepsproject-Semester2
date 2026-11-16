using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class IndexModel : PageModel
{
    private readonly DealService _dealService;

    public IndexModel(DealService dealService)
    {
        _dealService = dealService;
    }

    public List<Deal> Deals { get; private set; } = [];

    public void OnGet()
    {
        Deals = _dealService.GetAll();
    }
}
