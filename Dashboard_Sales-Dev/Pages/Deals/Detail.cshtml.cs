using Dashboard_Sales_Dev.Logic.Models;
using LibraryModule = Dashboard_Sales_Dev.Logic.Models.Module;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class DetailModel : PageModel
{
    private readonly DealService _dealService;
    private readonly ModuleService _moduleService;
    private readonly CommentService _commentService;
    private readonly EmployeeService _employeeService;

    public DetailModel(DealService dealService, ModuleService moduleService, CommentService commentService, EmployeeService employeeService)
    {
        _dealService = dealService;
        _moduleService = moduleService;
        _commentService = commentService;
        _employeeService = employeeService;
    }

    public Deal Deal { get; private set; } = default!;
    public List<LibraryModule> LinkedModules { get; private set; } = new();
    public List<SelectListItem> AvailableModules { get; private set; } = new();
    public List<SelectListItem> Statuses { get; private set; } = new();
    public string? StatusError { get; private set; }
    public int ProductionTimeInWeeks { get; private set; }
    public DateTime EarliestDeliveryDate { get; private set; }
    public bool EstimatedDateTooEarly { get; private set; }

    public List<Comment> Comments { get; private set; } = new();
    public List<SelectListItem> Authors { get; private set; } = new();
    public string? CommentError { get; private set; }

    [BindProperty]
    public int ModuleId { get; set; }

    [BindProperty]
    public int StatusId { get; set; }

    [BindProperty]
    public int CommentEmployeeId { get; set; }

    [BindProperty]
    public string? CommentMessage { get; set; }

    public IActionResult OnGet(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        Deal = deal;
        LoadPage(id);

        return Page();
    }

    public async Task<IActionResult> OnPostStatusAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        try
        {
            await _dealService.ChangeStatusAsync(id, StatusId);
        }
        catch (ArgumentException exception)
        {
            Deal = deal;
            StatusError = exception.Message;
            LoadPage(id);
            return Page();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCommentAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        try
        {
            await _commentService.AddAsync(id, CommentEmployeeId, CommentMessage);
        }
        catch (ArgumentException exception)
        {
            Deal = deal;
            CommentError = exception.Message;
            LoadPage(id);
            return Page();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostLinkAsync(int id)
    {
        if (_dealService.GetById(id) == null)
        {
            return NotFound();
        }

        if (ModuleId > 0)
        {
            await _dealService.LinkModuleAsync(id, ModuleId);
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnlinkAsync(int id, int moduleId)
    {
        await _dealService.UnlinkModuleAsync(id, moduleId);

        return RedirectToPage(new { id });
    }

    private void LoadPage(int id)
    {
        if (StatusId == 0)
        {
            StatusId = Deal.StatusId;
        }

        Statuses = _dealService.GetStatuses()
            .Select(status => new SelectListItem(status.Name, status.Id.ToString(), status.Id == Deal.StatusId))
            .ToList();
        LoadModules(id);
        LoadComments(id);
    }

    private void LoadComments(int id)
    {
        Comments = _commentService.GetForDeal(id);
        Authors = _employeeService.GetAll()
            .Select(employee => new SelectListItem($"{employee.FullName} ({employee.RoleName})", employee.Id.ToString()))
            .ToList();
    }

    private void LoadModules(int id)
    {
        LinkedModules = _dealService.GetModules(id);
        ProductionTimeInWeeks = _dealService.GetProductionTimeInWeeks(LinkedModules);
        EarliestDeliveryDate = _dealService.GetEarliestDeliveryDate(LinkedModules, DateTime.Today);
        EstimatedDateTooEarly = LinkedModules.Count > 0 && Deal.EstimatedDate.Date < EarliestDeliveryDate;

        HashSet<int> linkedIds = LinkedModules.Select(module => module.Id).ToHashSet();
        AvailableModules = _moduleService.GetAll()
            .Where(module => !linkedIds.Contains(module.Id))
            .OrderBy(module => module.Name)
            .Select(module => new SelectListItem($"{module.Name} ({module.ProductionTimeInWeeks} {(module.ProductionTimeInWeeks == 1 ? "week" : "weeks")})", module.Id.ToString()))
            .ToList();
    }
}
