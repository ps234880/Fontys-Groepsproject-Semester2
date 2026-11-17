namespace Dashboard_Sales_Dev.Logic.Models;

public record DealFilter(string? Owner, string? Stage)
{
    public const string MyDeals = "me";
    public const string OpenDeals = "open";
    public const string ClosedDeals = "closed";

    public bool IsEmpty => string.IsNullOrEmpty(Owner) && string.IsNullOrEmpty(Stage);
}
