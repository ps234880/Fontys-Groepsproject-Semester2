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

        string query = @"SELECT id, client_id, employee_id, title, description, 
                                estimatedDate, createdAt, updatedAt 
                         FROM quotation";

        using MySqlCommand command = new MySqlCommand(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Quotation quotation = new Quotation
            {
                Id = reader.GetInt32("id"),
                ClientId = reader.GetInt32("client_id"),
                EmployeeId = reader.GetInt32("employee_id"),
                Title = reader.GetString("title"),
                // Controleer op NULL voor optionele velden
                Description = reader.IsDBNull(reader.GetOrdinal("description")) 
                    ? string.Empty 
                    : reader.GetString("description"),
                EstimatedDate = reader.GetDateTime("estimatedDate"),
                CreatedAt = reader.GetDateTime("createdAt"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updatedAt")) 
                    ? null 
                    : reader.GetDateTime("updatedAt")
            };

            quotations.Add(quotation);
        }

        return quotations;
    }
}