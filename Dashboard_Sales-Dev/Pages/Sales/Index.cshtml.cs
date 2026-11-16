using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Sales;

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
    public int DealsNeedingAction => Deals.Count(deal => deal.Progress != DealProgress.OnTrack);

    public void OnGet()
    {
        Deals = _dealService.GetOpenDealOverviews(DateTime.Today);
        LatestComments = _commentService.GetLatest(LatestCommentCount);
    }
}
