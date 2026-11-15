using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface IEmployeeRepository
{
    List<Employee> GetAll();
    List<Employee> GetByRole(string roleName);
    Employee? GetById(int id);
}
