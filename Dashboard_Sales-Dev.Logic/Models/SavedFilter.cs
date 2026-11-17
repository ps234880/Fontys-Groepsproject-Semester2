namespace Dashboard_Sales_Dev.Logic.Models;

public class SavedFilter
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Owner { get; set; }
    public string? Stage { get; set; }
    public DateTime CreatedAt { get; set; }
}
