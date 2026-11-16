namespace Dashboard_Sales_Dev.Logic.Models;

public enum DealProgress
{
    OnTrack,
    NoModules,
    DateTooEarly
}

public record DealOverview(Deal Deal, int ModuleCount, int ProductionTimeInWeeks, DealProgress Progress);
