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
    public NewDealInput Input { get; set; } = new();

    public List<SelectListItem> Owners { get; private set; } = new();

    public void OnGet()
    {
        Input.EstimatedDate = DateTime.Today.AddMonths(1);
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
            CompanyName = Input.CompanyName,
            FirstName = Input.FirstName,
            LastName = Input.LastName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber
        };

        Deal deal = new Deal
        {
            Title = Input.Title,
            Requirement = Input.Requirement,
            EstimatedDate = Input.EstimatedDate,
            EmployeeId = Input.OwnerId
        };

        try
        {
            await _dealService.CreateWithNewClientAsync(client, deal);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError("Input.OwnerId", exception.Message);
            LoadOwners();
            return Page();
        }

        return RedirectToPage("./Index");
    }

    private void LoadOwners()
    {
        Owners = _employeeService.GetSalesEmployees()
            .Select(employee => new SelectListItem(employee.FullName, employee.Id.ToString()))
            .ToList();
    }

    public class NewDealInput
    {
        [Required, StringLength(255)]
        public string CompanyName { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Requirement { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime EstimatedDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Choose the Sales employee who owns this deal.")]
        public int OwnerId { get; set; }
    }
}
