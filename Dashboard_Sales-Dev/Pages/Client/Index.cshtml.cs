using Dashboard_Sales_Dev.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Client;

public class Index : PageModel
{
    private readonly ClientRepository _repository;

    public Index(ClientRepository repository)
    {
        _repository = repository;
    }

    public List<Models.Client> Clients { get; private set; } = [];

    public void OnGet()
    {
        Clients = _repository.GetAll();
    }
}