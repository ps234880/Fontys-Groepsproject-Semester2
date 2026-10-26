using Dashboard_Sales_Dev.Models;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace Dashboard_Sales_Dev.Repository;

public class QuotationRepository
{
    private readonly string _connectionString;

    public QuotationRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public List<Quotation> GetAll()
    {
        List<Quotation> quotations = new List<Quotation>();

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query = @"SELECT
    q.id,
    q.title,
    q.description,
    q.estimatedDate,
    q.createdAt,
    c.companyName,
    c.firstName AS clientFirstName,
    c.lastName AS clientLastName,
    e.firstName AS employeeFirstName,
    e.lastName AS employeeLastName
FROM quotation q
INNER JOIN client c ON q.client_id = c.id
INNER JOIN employee e ON q.employee_id = e.id;";

        using MySqlCommand command = new MySqlCommand(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Quotation quotation = new Quotation
            {
                Id = reader.GetInt32("id"),
                Title = reader.GetString("title"),
                Description = reader.GetString("description"),
                EstimatedDate = reader.GetDateTime("estimatedDate"),
                ClientName = reader.GetString("clientFirstName") + " " +
                   reader.GetString("clientLastName"),
                EmployeeName = reader.GetString("employeeFirstName") + " " +
                     reader.GetString("employeeLastName"),
                CreatedAt = reader.GetDateTime("createdAt")
            };

            quotations.Add(quotation);
        }

        return quotations;
    }

    public async Task AddQuotationAsync(Quotation quotation)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"INSERT INTO quotation (client_id, employee_id, title, description, 
                                estimatedDate, createdAt) 
                        VALUES (@client_id, @employee_id, @title, @description, @estimatedDate, @CreatedAt)";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@client_id", quotation.ClientId);
        cmd.Parameters.AddWithValue("@employee_id", quotation.EmployeeId);
        cmd.Parameters.AddWithValue("@title", quotation.Title ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@description", quotation.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@estimatedDate", quotation.EstimatedDate);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        await cmd.ExecuteNonQueryAsync();
    }
}
