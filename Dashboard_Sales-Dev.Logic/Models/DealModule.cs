namespace Dashboard_Sales_Dev.Logic.Models;

public class DealModule : Module
{
    public int? DealPriceInCents { get; set; }

    public int FinalPriceInCents => DealPriceInCents ?? PriceInCents;
}
