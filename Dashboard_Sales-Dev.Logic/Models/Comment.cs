namespace Dashboard_Sales_Dev.Logic.Models;

public class Comment
{
    public int Id { get; set; }
    public int QuotationId { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeRole { get; set; } = string.Empty;
    public string QuotationTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string Initials
    {
        get
        {
            string[] parts = EmployeeName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Take(2).Select(part => char.ToUpperInvariant(part[0])));
        }
    }
}
