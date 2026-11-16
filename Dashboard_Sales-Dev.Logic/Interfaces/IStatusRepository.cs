using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IStatusRepository
{
    List<Status> GetAll();
    Status? GetById(int id);
    Status? GetByName(string name);
}
