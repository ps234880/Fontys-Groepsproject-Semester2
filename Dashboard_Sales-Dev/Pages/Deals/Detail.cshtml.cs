using LibraryModule = Dashboard_Sales_Dev.Logic.Models.Module;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Deal = Dashboard_Sales_Dev.Logic.Models.Quotation;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class DetailModel : PageModel
{
    private readonly QuotationService _quotationService;
    private readonly ModuleService _moduleService;

    public DetailModel(QuotationService quotationService, ModuleService moduleService)
    {
        _quotationService = quotationService;
        _moduleService = moduleService;
    }

    public Deal Deal { get; private set; } = default!;
    public List<LibraryModule> LinkedModules { get; private set; } = new();
    public List<SelectListItem> AvailableModules { get; private set; } = new();
    public int ProductionTimeInWeeks { get; private set; }
    public DateTime EarliestDeliveryDate { get; private set; }
    public bool EstimatedDateTooEarly { get; private set; }

    [BindProperty]
    public int ModuleId { get; set; }

    public IActionResult OnGet(int id)
    {
        Deal? deal = _quotationService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        Deal = deal;
        LoadModules(id);

        return Page();
    }

    public async Task<IActionResult> OnPostLinkAsync(int id)
    {
        if (_quotationService.GetById(id) == null)
        {
            return NotFound();
        }

        if (ModuleId > 0)
        {
            await _quotationService.LinkModuleAsync(id, ModuleId);
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnlinkAsync(int id, int moduleId)
    {
        await _quotationService.UnlinkModuleAsync(id, moduleId);

        return RedirectToPage(new { id });
    }

    private void LoadModules(int id)
    {
        LinkedModules = _quotationService.GetModules(id);
        ProductionTimeInWeeks = _quotationService.GetProductionTimeInWeeks(LinkedModules);
        EarliestDeliveryDate = _quotationService.GetEarliestDeliveryDate(LinkedModules, DateTime.Today);
        EstimatedDateTooEarly = LinkedModules.Count > 0 && Deal.EstimatedDate.Date < EarliestDeliveryDate;

        HashSet<int> linkedIds = LinkedModules.Select(module => module.Id).ToHashSet();
        AvailableModules = _moduleService.GetAll()
            .Where(module => !linkedIds.Contains(module.Id))
            .OrderBy(module => module.Name)
            .Select(module => new SelectListItem($"{module.Name} ({module.ProductionTimeInWeeks} {(module.ProductionTimeInWeeks == 1 ? "week" : "weeks")})", module.Id.ToString()))
            .ToList();
    }
}
