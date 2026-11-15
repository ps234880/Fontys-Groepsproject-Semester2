using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class EmployeeRepository : RepositoryBase, IEmployeeRepository
{
    public EmployeeRepository(string connectionString) : base(connectionString)
    {
    }

    private const string SelectEmployee = @"SELECT
    e.id,
    e.firstName,
    e.lastName,
    r.name AS roleName
FROM employee e
INNER JOIN role r ON e.role_id = r.id";

    public List<Employee> GetAll()
    {
        List<Employee> employees = new List<Employee>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectEmployee + " ORDER BY r.name DESC, e.firstName, e.lastName;", connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            employees.Add(MapEmployee(reader));
        }

        return employees;
    }

    public List<Employee> GetByRole(string roleName)
    {
        List<Employee> employees = new List<Employee>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectEmployee + " WHERE r.name = @roleName ORDER BY e.firstName, e.lastName;", connection);
        command.Parameters.AddWithValue("@roleName", roleName);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            employees.Add(MapEmployee(reader));
        }

        return employees;
    }

    public Employee? GetById(int id)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectEmployee + " WHERE e.id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapEmployee(reader) : null;
    }

    private static Employee MapEmployee(MySqlDataReader reader)
    {
        return new Employee
        {
            Id = reader.GetInt32("id"),
            FirstName = reader.GetString("firstName"),
            LastName = reader.GetString("lastName"),
            RoleName = reader.GetString("roleName")
        };
    }
}
