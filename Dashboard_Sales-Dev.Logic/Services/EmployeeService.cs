using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using Dashboard_Sales_Dev.Logic.Security;

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

    public Employee? Authenticate(string? email, string? password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        (Employee Employee, string PasswordHash)? login = _employeeRepository.GetLoginByEmail(email.Trim());

        if (login == null || !PasswordHasher.Verify(password, login.Value.PasswordHash))
        {
            return null;
        }

        return login.Value.Employee;
    }

    public List<Employee> GetSalesEmployees()
    {
        return _employeeRepository.GetByRole(SalesRole);
    }
}
