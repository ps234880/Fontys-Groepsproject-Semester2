using System.Security.Claims;
using Dashboard_Sales_Dev.Authentication;
using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages;

public class IndexModel : PageModel
{
    private readonly EmployeeService _employeeService;

    public IndexModel(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [BindProperty]
    public string? Email { get; set; }

    [BindProperty]
    public string? Password { get; set; }

    [BindProperty]
    public bool RememberMe { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? Error { get; private set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage(HomePage(User.GetRole()));
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Employee? employee = _employeeService.Authenticate(Email, Password);

        if (employee == null)
        {
            Error = "The email address or password is incorrect.";
            Password = null;
            return Page();
        }

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Name, employee.FullName),
            new(ClaimTypes.GivenName, employee.FirstName),
            new(ClaimTypes.Surname, employee.LastName),
            new(ClaimTypes.Role, employee.RoleName)
        ];

        ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = RememberMe });

        if (Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage(HomePage(employee.RoleName));
    }

    private static string HomePage(string role)
    {
        return role == EmployeeService.DevelopmentRole ? "/Development/Index" : "/Sales/Index";
    }
}
