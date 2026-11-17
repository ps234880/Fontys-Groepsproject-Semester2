using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class EmployeeService
{
    public const string SalesRole = "Sales";
    public const string DevelopmentRole = "Development";

    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public List<Employee> GetAll()
    {
        return _employeeRepository.GetAll();
    }

    public List<Employee> GetSalesEmployees()
    {
        return _employeeRepository.GetByRole(SalesRole);
    }
}
