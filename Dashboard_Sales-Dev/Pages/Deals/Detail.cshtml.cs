using Dashboard_Sales_Dev.Logic.Models;
using Customer = Dashboard_Sales_Dev.Logic.Models.Client;
using LibraryModule = Dashboard_Sales_Dev.Logic.Models.Module;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    public List<DealModule> LinkedModules { get; private set; } = new();
    public int TotalPriceInCents { get; private set; }
    public string? ModuleError { get; private set; }
    public string? NewModuleError { get; private set; }
    public List<SelectListItem> AvailableModules { get; private set; } = new();
    public List<Customer> Contacts { get; private set; } = new();
    public List<SelectListItem> AvailableContacts { get; private set; } = new();
    public List<SelectListItem> Statuses { get; private set; } = new();
    public string? StatusError { get; private set; }
    public List<SelectListItem> WaitingOnOptions { get; private set; } = new();
    public string? WaitingOnError { get; private set; }
    public int ProductionTimeInWeeks { get; private set; }
    public DateTime EarliestDeliveryDate { get; private set; }
    public bool EstimatedDateTooEarly { get; private set; }

    public List<Comment> Comments { get; private set; } = new();
    public List<SelectListItem> Authors { get; private set; } = new();
    public string? CommentError { get; private set; }

    public string? NewContactError { get; private set; }

    public List<Employee> TeamMembers { get; private set; } = new();
    public List<SelectListItem> AvailableTeamMembers { get; private set; } = new();
    public string? TeamError { get; private set; }

    [BindProperty]
    public int TeamMemberId { get; set; }

    [BindProperty]
    public int ModuleId { get; set; }

    [BindProperty]
    public int ContactId { get; set; }

    [BindProperty]
    public int? ModulePriceInEuros { get; set; }

    [BindProperty]
    public int? ModuleWeeks { get; set; }

    [BindProperty]
    public LibraryModule NewModule { get; set; } = new();

    [BindProperty]
    public int StatusId { get; set; }

    [BindProperty]
    public string? WaitingOn { get; set; }

    [BindProperty]
    public int CommentEmployeeId { get; set; }

    [BindProperty]
    public string? CommentMessage { get; set; }

    [BindProperty]
    public Customer NewContact { get; set; } = new();

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

    public async Task<IActionResult> OnPostWaitingOnAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        try
        {
            await _dealService.ChangeWaitingOnAsync(id, WaitingOn);
        }
        catch (ArgumentException exception)
        {
            Deal = deal;
            WaitingOnError = exception.Message;
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

    public async Task<IActionResult> OnPostUpdateModuleAsync(int id, int moduleId)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        try
        {
            if (ModelState.GetFieldValidationState(nameof(ModuleWeeks)) == ModelValidationState.Invalid)
            {
                throw new ArgumentException("Enter the production time in whole weeks, for example 3.");
            }

            if (ModelState.GetFieldValidationState(nameof(ModulePriceInEuros)) == ModelValidationState.Invalid)
            {
                throw new ArgumentException("Enter the deal price in whole euros, for example 12500.");
            }

            await _dealService.UpdateDealModuleAsync(id, moduleId, ModulePriceInEuros, ModuleWeeks);
        }
        catch (ArgumentException exception)
        {
            Deal = deal;
            ModuleError = exception.Message;
            LoadPage(id);
            return Page();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCreateModuleAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        try
        {
            string? bindingError = ModelState
                .Where(entry => entry.Key.StartsWith(nameof(NewModule) + ".") && entry.Value is { ValidationState: ModelValidationState.Invalid })
                .SelectMany(entry => entry.Value!.Errors)
                .Select(error => string.IsNullOrEmpty(error.ErrorMessage) ? "Check the module fields: price and weeks must be whole numbers." : error.ErrorMessage)
                .FirstOrDefault();

            if (bindingError != null)
            {
                throw new ArgumentException(bindingError);
            }

            await _dealService.CreateModuleForDealAsync(id, NewModule);
        }
        catch (ArgumentException exception)
        {
            Deal = deal;
            NewModuleError = exception.Message;
            LoadPage(id);
            return Page();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnlinkAsync(int id, int moduleId)
    {
        await _dealService.UnlinkModuleAsync(id, moduleId);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostLinkContactAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        if (ContactId > 0)
        {
            await _dealService.LinkContactAsync(deal, ContactId);
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCreateContactAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(NewContact.FirstName) || string.IsNullOrWhiteSpace(NewContact.LastName) ||
            string.IsNullOrWhiteSpace(NewContact.Email) || string.IsNullOrWhiteSpace(NewContact.PhoneNumber))
        {
            Deal = deal;
            NewContactError = "Fill in first name, last name, email address and phone number.";
            LoadPage(id);
            return Page();
        }

        NewContact.CompanyName = deal.ClientCompanyName;
        await _dealService.CreateContactAsync(id, NewContact);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUnlinkContactAsync(int id, int clientId)
    {
        await _dealService.UnlinkContactAsync(id, clientId);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostAddTeamMemberAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        try
        {
            await _dealService.AddTeamMemberAsync(deal, TeamMemberId);
        }
        catch (ArgumentException exception)
        {
            Deal = deal;
            TeamError = exception.Message;
            LoadPage(id);
            return Page();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveTeamMemberAsync(int id, int employeeId)
    {
        await _dealService.RemoveTeamMemberAsync(id, employeeId);

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemovePrimaryAsync(int id)
    {
        Deal? deal = _dealService.GetById(id);

        if (deal == null)
        {
            return NotFound();
        }

        await _dealService.RemovePrimaryContactAsync(deal);

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
        WaitingOnOptions = new List<SelectListItem> { new("Nobody, the deal is not blocked", string.Empty, Deal.WaitingOnRole == null) };
        WaitingOnOptions.AddRange(DealService.WaitingOnRoles
            .Select(role => new SelectListItem($"Waiting for {role}", role, role == Deal.WaitingOnRole)));
        LoadModules(id);
        LoadContacts(id);
        LoadTeam(id);
        LoadComments(id);
    }

    private void LoadContacts(int id)
    {
        Contacts = _dealService.GetContacts(id);
        AvailableContacts = _dealService.GetAvailableContacts(Deal)
            .Select(client => new SelectListItem($"{client.CompanyName} - {client.FirstName} {client.LastName}", client.Id.ToString()))
            .ToList();
    }

    private void LoadTeam(int id)
    {
        TeamMembers = _dealService.GetTeamMembers(id);
        AvailableTeamMembers = _dealService.GetAvailableTeamMembers(Deal)
            .Select(employee => new SelectListItem($"{employee.FullName} ({employee.RoleName})", employee.Id.ToString()))
            .ToList();
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
        TotalPriceInCents = _dealService.GetTotalPriceInCents(LinkedModules);
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