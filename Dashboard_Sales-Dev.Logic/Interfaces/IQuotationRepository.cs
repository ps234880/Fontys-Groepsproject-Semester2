using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IQuotationRepository
{
    List<Quotation> GetAll();
    Quotation? GetById(int id);
    Task AddQuotationAsync(Quotation quotation);
}
