using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IClientRepository
{
    List<Client> GetAll();
    Task<int> AddClientAsync(Client client);
}
