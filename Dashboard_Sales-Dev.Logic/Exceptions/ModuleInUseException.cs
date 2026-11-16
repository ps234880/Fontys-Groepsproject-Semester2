namespace Dashboard_Sales_Dev.Logic.Exceptions;

public class ModuleInUseException : Exception
{
    public ModuleInUseException(int moduleId, Exception innerException)
        : base($"Module {moduleId} cannot be deleted because it is linked to a deal.", innerException)
    {
        ModuleId = moduleId;
    }

    public int ModuleId { get; }
}
