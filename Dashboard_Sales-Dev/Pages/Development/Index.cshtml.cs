using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Development;

public class IndexModel : PageModel
{
    private const int LatestCommentCount = 5;

    private readonly DealService _dealService;
    private readonly CommentService _commentService;

    public IndexModel(DealService dealService, CommentService commentService)
    {
        _dealService = dealService;
        _commentService = commentService;
    }

    public List<DealOverview> Deals { get; private set; } = new();
    public List<Comment> LatestComments { get; private set; } = new();
    public int DealsWithoutModules => Deals.Count(deal => deal.Progress == DealProgress.NoModules);

    public void OnGet()
    {
        Deals = _dealService.GetOpenDealOverviews(DateTime.Today)
            .OrderByDescending(deal => deal.Progress == DealProgress.NoModules)
            .ThenBy(deal => deal.Deal.EstimatedDate)
            .ToList();
        LatestComments = _commentService.GetLatest(LatestCommentCount);
    }
}
