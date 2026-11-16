using Dashboard_Sales_Dev.Logic.Models;
using LibraryModule = Dashboard_Sales_Dev.Logic.Models.Module;
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
        foreach (Deal deal in _dealService.GetAll().Where(_dealService.IsOpen))
        {
            List<LibraryModule> modules = _dealService.GetModules(deal.Id);
            DateTime earliest = _dealService.GetEarliestDeliveryDate(modules, DateTime.Today);

            DealProgress progress = DealProgress.OnTrack;
            if (modules.Count == 0)
            {
                progress = DealProgress.NoModules;
            }
            else if (deal.EstimatedDate.Date < earliest)
            {
                progress = DealProgress.DateTooEarly;
            }

            Deals.Add(new DealOverview(deal, modules.Count, _dealService.GetProductionTimeInWeeks(modules), progress));
        }

        LatestComments = _commentService.GetLatest(LatestCommentCount);
    }

    public enum DealProgress
    {
        OnTrack,
        NoModules,
        DateTooEarly
    }

    public record DealOverview(Deal Deal, int ModuleCount, int ProductionTimeInWeeks, DealProgress Progress);
}
