using System.ComponentModel.DataAnnotations;

namespace Dashboard_Sales_Dev.Logic.Models;

public class Module
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int PriceInCents { get; set; }

    [Range(1, int.MaxValue)]
    public int ProductionTimeInWeeks { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
