using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IQuotationRepository
{
    List<Quotation> GetAll();
    Quotation? GetById(int id);
    Task AddQuotationAsync(Quotation quotation);
    List<Module> GetModules(int quotationId);
    Task AddModuleAsync(int quotationId, int moduleId);
    Task RemoveModuleAsync(int quotationId, int moduleId);
}
