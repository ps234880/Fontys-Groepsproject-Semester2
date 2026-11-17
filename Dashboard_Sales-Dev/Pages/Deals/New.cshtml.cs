using System.ComponentModel.DataAnnotations;
using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Customer = Dashboard_Sales_Dev.Logic.Models.Client;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class NewModel : PageModel
{
    private readonly DealService _dealService;
    private readonly EmployeeService _employeeService;

    public NewModel(DealService dealService, EmployeeService employeeService)
    {
        _dealService = dealService;
        _employeeService = employeeService;
    }

    [BindProperty]
    public DealInput Input { get; set; } = new();

    public List<SelectListItem> Owners { get; private set; } = new();

    public void OnGet()
    {
        Input.EstimatedDate = DateTime.Today;
        LoadOwners();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            LoadOwners();
            return Page();
        }

        Customer client = new Customer
        {
            CompanyName = Input.CompanyName.Trim(),
            FirstName = Input.FirstName.Trim(),
            LastName = Input.LastName.Trim(),
            Email = Input.Email.Trim(),
            PhoneNumber = Input.PhoneNumber.Trim()
        };

        Deal deal = new Deal
        {
            EmployeeId = Input.OwnerId,
            Title = Input.Title.Trim(),
            Goal = Input.Goal.Trim(),
            Scope = Input.Scope.Trim(),
            EstimatedDate = Input.EstimatedDate
        };

        int dealId;

        try
        {
            dealId = await _dealService.CreateWithNewClientAsync(client, deal);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError("Input.OwnerId", exception.Message);
            LoadOwners();
            return Page();
        }

        return RedirectToPage("/Deals/Detail", new { id = dealId });
    }

    private void LoadOwners()
    {
        Owners = _employeeService.GetAll()
            .Where(employee => employee.RoleName == EmployeeService.SalesRole)
            .Select(employee => new SelectListItem(employee.FullName, employee.Id.ToString()))
            .ToList();
    }

    public class DealInput
    {
        [Required(ErrorMessage = "Enter a company name.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a first name.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a last name.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter an email address."), EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a project name.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Describe the goal of the project.")]
        [StringLength(255, ErrorMessage = "Keep the goal under 255 characters.")]
        public string Goal { get; set; } = string.Empty;

        [Required(ErrorMessage = "Describe the scope of the project.")]
        [StringLength(10000, ErrorMessage = "Keep the scope under 10,000 characters.")]
        public string Scope { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose an estimated date.")]
        public DateTime EstimatedDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Choose an owner.")]
        public int OwnerId { get; set; }
    }
}