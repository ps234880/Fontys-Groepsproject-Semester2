using Dashboard_Sales_Dev.Authentication;
using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class IndexModel : PageModel
{
    private readonly DealService _dealService;
    private readonly EmployeeService _employeeService;
    private readonly SavedFilterService _savedFilterService;

    public IndexModel(DealService dealService, EmployeeService employeeService, SavedFilterService savedFilterService)
    {
        _dealService = dealService;
        _employeeService = employeeService;
        _savedFilterService = savedFilterService;
    }

    [BindProperty(SupportsGet = true)]
    public string? Owner { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Stage { get; set; }

    [BindProperty]
    public string? FilterName { get; set; }

    public List<Deal> Deals { get; private set; } = [];
    public int TotalCount { get; private set; }
    public List<SelectListItem> OwnerOptions { get; private set; } = [];
    public List<SelectListItem> StageOptions { get; private set; } = [];
    public List<SavedFilter> SavedFilters { get; private set; } = [];
    public string? FilterError { get; private set; }

    public DealFilter CurrentFilter => new(Owner, Stage);

    public IActionResult OnGet()
    {
        try
        {
            LoadPage();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        try
        {
            await _savedFilterService.SaveAsync(User.GetEmployeeId(), FilterName, CurrentFilter);
        }
        catch (ArgumentException exception)
        {
            FilterError = exception.Message;
            LoadPage();
            return Page();
        }

        return RedirectToPage(new { Owner, Stage });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int filterId)
    {
        await _savedFilterService.DeleteAsync(User.GetEmployeeId(), filterId);

        return RedirectToPage(new { Owner, Stage });
    }

    public bool IsActive(SavedFilter filter)
    {
        return (filter.Owner ?? string.Empty) == (Owner ?? string.Empty)
            && (filter.Stage ?? string.Empty) == (Stage ?? string.Empty);
    }

    private void LoadPage()
    {
        List<Deal> all = _dealService.GetAll();
        TotalCount = all.Count;
        Deals = _dealService.Filter(all, CurrentFilter, User.GetEmployeeId());
        SavedFilters = _savedFilterService.GetForEmployee(User.GetEmployeeId());

        OwnerOptions =
        [
            new SelectListItem("Everyone", string.Empty),
            new SelectListItem("My deals", DealFilter.MyDeals)
        ];
        OwnerOptions.AddRange(_employeeService.GetSalesEmployees()
            .Select(employee => new SelectListItem(employee.FullName, employee.Id.ToString())));

        StageOptions =
        [
            new SelectListItem("All stages", string.Empty),
            new SelectListItem("Open deals", DealFilter.OpenDeals),
            new SelectListItem("Closed deals", DealFilter.ClosedDeals)
        ];
        StageOptions.AddRange(_dealService.GetStatuses()
            .Select(status => new SelectListItem(status.Name, status.Id.ToString())));

        OwnerOptions.ForEach(option => option.Selected = option.Value == (Owner ?? string.Empty));
        StageOptions.ForEach(option => option.Selected = option.Value == (Stage ?? string.Empty));
    }
}
