namespace Dashboard_Sales_Dev.Logic.Models;

public class Deal
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int EmployeeId { get; set; }
    public int StatusId { get; set; }

    public string ClientName { get; set; } = string.Empty;
    public string ClientCompanyName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string ClientPhoneNumber { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Requirement { get; set; } = string.Empty;
    public DateTime EstimatedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
