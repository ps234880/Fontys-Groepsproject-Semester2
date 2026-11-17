namespace Dashboard_Sales_Dev.Logic.Models;

public class DealModule : Module
{
    public int? DealPriceInCents { get; set; }

    public int? DealProductionTimeInWeeks { get; set; }

    public int FinalPriceInCents => DealPriceInCents ?? PriceInCents;

    public int FinalProductionTimeInWeeks => DealProductionTimeInWeeks ?? ProductionTimeInWeeks;
}
