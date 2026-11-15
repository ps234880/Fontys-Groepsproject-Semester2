using Dashboard_Sales_Dev.Logic.Models;
using LibraryModule = Dashboard_Sales_Dev.Logic.Models.Module;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Sales;

public class IndexModel : PageModel
{
    private const int LatestCommentCount = 5;

    private readonly QuotationService _quotationService;
    private readonly CommentService _commentService;

    public IndexModel(QuotationService quotationService, CommentService commentService)
    {
        _quotationService = quotationService;
        _commentService = commentService;
    }

    public List<DealOverview> Deals { get; private set; } = new();
    public List<Comment> LatestComments { get; private set; } = new();
    public int DealsNeedingAction => Deals.Count(deal => deal.Status != DealStatus.OnTrack);

    public void OnGet()
    {
        foreach (Quotation deal in _quotationService.GetAll())
        {
            List<LibraryModule> modules = _quotationService.GetModules(deal.Id);
            DateTime earliest = _quotationService.GetEarliestDeliveryDate(modules, DateTime.Today);

            DealStatus status = DealStatus.OnTrack;
            if (modules.Count == 0)
            {
                status = DealStatus.NoModules;
            }
            else if (deal.EstimatedDate.Date < earliest)
            {
                status = DealStatus.DateTooEarly;
            }

            Deals.Add(new DealOverview(deal, modules.Count, _quotationService.GetProductionTimeInWeeks(modules), status));
        }

        LatestComments = _commentService.GetLatest(LatestCommentCount);
    }

    public enum DealStatus
    {
        OnTrack,
        NoModules,
        DateTooEarly
    }

    public record DealOverview(Quotation Deal, int ModuleCount, int ProductionTimeInWeeks, DealStatus Status);
}
