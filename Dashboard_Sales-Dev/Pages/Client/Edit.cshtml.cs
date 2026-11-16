using Dashboard_Sales_Dev.Logic.Exceptions;
using Dashboard_Sales_Dev.Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dashboard_Sales_Dev.Pages.Client;

public class EditModel : PageModel
{
    private readonly ClientService _clientService;

    public EditModel(ClientService clientService)
    {
        _clientService = clientService;
    }

    [BindProperty]
    public Logic.Models.Client Client { get; set; } = default!;

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet(int id)
    {
        Logic.Models.Client? client = _clientService.GetById(id);

        if (client == null)
        {
            return NotFound();
        }

        Client = client;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Client.Id = id;

        bool updated = await _clientService.UpdateClientAsync(Client);

        if (!updated)
        {
            return NotFound();
        }

        return RedirectToPage("/Client/Index");
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            bool deleted = await _clientService.DeleteClientAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToPage("/Client/Index");
        }
        catch (ClientInUseException)
        {
            Logic.Models.Client? client = _clientService.GetById(id);

            if (client == null)
            {
                return NotFound();
            }

            Client = client;
            ErrorMessage = "This customer has deals and can't be deleted.";

            return Page();
        }
    }
}