using System.ComponentModel.DataAnnotations;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Customer = Dashboard_Sales_Dev.Logic.Models.Client;
using Deal = Dashboard_Sales_Dev.Logic.Models.Quotation;

namespace Dashboard_Sales_Dev.Pages.Deals;

public class NewModel : PageModel
{
    private readonly QuotationService _quotationService;

    public NewModel(QuotationService quotationService)
    {
        _quotationService = quotationService;
    }

    [BindProperty]
    public NewDealInput Input { get; set; } = new();

    public void OnGet()
    {
        Input.EstimatedDate = DateTime.Today.AddMonths(1);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
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
            Description = Input.Description,
            EstimatedDate = Input.EstimatedDate
        };

        await _quotationService.CreateWithNewClientAsync(client, deal);

        return RedirectToPage("./Index");
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
        public string Description { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime EstimatedDate { get; set; }
    }
}
