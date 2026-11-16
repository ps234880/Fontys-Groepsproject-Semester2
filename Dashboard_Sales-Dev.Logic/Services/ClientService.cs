using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class ClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public List<Client> GetAll()
    {
        return _clientRepository.GetAll();
    }

    public Client? GetById(int id)
    {
        return _clientRepository.GetById(id);
    }

    public Task<bool> UpdateClientAsync(Client client)
    {
        return _clientRepository.UpdateClientAsync(client);
    }

    public Task<bool> DeleteClientAsync(int id)
    {
        return _clientRepository.DeleteClientAsync(id);
    }
}