using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Search;

public class IndexModel : PageModel
{
    private readonly SearchService _searchService;

    public IndexModel(SearchService searchService)
    {
        _searchService = searchService;
    }

    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    public SearchResults Results { get; private set; } = SearchResults.Empty;

    public void OnGet()
    {
        Results = _searchService.Search(Q);
    }
}
