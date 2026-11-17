using System.Security.Claims;

namespace Dashboard_Sales_Dev.Authentication;

public static class CurrentUser
{
    public static int GetEmployeeId(this ClaimsPrincipal user)
    {
        string? value = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out int employeeId))
        {
            throw new InvalidOperationException("The signed-in user has no valid employee id claim.");
        }

        return employeeId;
    }

    public static string GetInitials(this ClaimsPrincipal user)
    {
        string first = user.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        string last = user.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

        return (first.Length > 0 ? first[..1] : string.Empty) + (last.Length > 0 ? last[..1].ToUpperInvariant() : string.Empty);
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}
