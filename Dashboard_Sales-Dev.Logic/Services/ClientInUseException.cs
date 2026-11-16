namespace Dashboard_Sales_Dev.Logic.Exceptions;

public class ClientInUseException(int clientId, Exception? inner = null)
    : Exception($"Client {clientId} is referenced by other records and cannot be deleted.", inner)
{
    public int ClientId { get; } = clientId;
}