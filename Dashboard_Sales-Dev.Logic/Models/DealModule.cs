namespace Dashboard_Sales_Dev.Logic.Models;

public class DealModule : Module
{
    public int? DealPriceInCents { get; set; }

    public int? DealProductionTimeInWeeks { get; set; }

    public bool HasPrice => DealPriceInCents != null;

    public int FinalPriceInCents => DealPriceInCents ?? 0;

    public int FinalProductionTimeInWeeks => DealProductionTimeInWeeks ?? ProductionTimeInWeeks;
}
