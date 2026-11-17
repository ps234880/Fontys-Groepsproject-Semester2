namespace Dashboard_Sales_Dev.Logic.Models;

public class DealScopeItem
{
    public int Id { get; set; }
    public int DealId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool InScope { get; set; }
}
