using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Client;

public class Index : PageModel
{
    private readonly ClientService _clientService;

    public Index(ClientService clientService)
    {
        _clientService = clientService;
    }

    public List<Logic.Models.Client> Clients { get; private set; } = [];

    public void OnGet()
    {
        Clients = _clientService.GetAll();
    }
}
