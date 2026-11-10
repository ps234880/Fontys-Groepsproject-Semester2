using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class QuotationRepository : RepositoryBase, IQuotationRepository
{
    public QuotationRepository(string connectionString) : base(connectionString)
    {
    }

    private const string SelectQuotation = @"SELECT
    q.id,
    q.title,
    q.description,
    q.estimatedDate,
    q.createdAt,
    c.companyName,
    c.firstName AS clientFirstName,
    c.lastName AS clientLastName,
    c.email AS clientEmail,
    c.phoneNumber AS clientPhoneNumber,
    e.firstName AS employeeFirstName,
    e.lastName AS employeeLastName
FROM quotation q
INNER JOIN client c ON q.client_id = c.id
INNER JOIN employee e ON q.employee_id = e.id";

    public List<Quotation> GetAll()
    {
        List<Quotation> quotations = new List<Quotation>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectQuotation + " ORDER BY q.createdAt DESC;", connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            quotations.Add(MapQuotation(reader));
        }

        return quotations;
    }

    public Quotation? GetById(int id)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectQuotation + " WHERE q.id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapQuotation(reader) : null;
    }

    private static Quotation MapQuotation(MySqlDataReader reader)
    {
        return new Quotation
        {
            Id = reader.GetInt32("id"),
            Title = reader.GetString("title"),
            Description = reader.GetString("description"),
            EstimatedDate = reader.GetDateTime("estimatedDate"),
            CreatedAt = reader.GetDateTime("createdAt"),
            ClientCompanyName = reader.GetString("companyName"),
            ClientName = reader.GetString("clientFirstName") + " " + reader.GetString("clientLastName"),
            ClientEmail = reader.GetString("clientEmail"),
            ClientPhoneNumber = reader.GetString("clientPhoneNumber"),
            EmployeeName = reader.GetString("employeeFirstName") + " " + reader.GetString("employeeLastName")
        };
    }

    public async Task AddQuotationAsync(Quotation quotation)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var sql = @"INSERT INTO quotation (client_id, employee_id, title, description, 
                               estimatedDate, createdAt) 
                       VALUES (@client_id, @employee_id, @title, @description, @estimatedDate, @CreatedAt)";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@client_id", quotation.ClientId) ;
        cmd.Parameters.AddWithValue("@employee_id", quotation.EmployeeId);
        cmd.Parameters.AddWithValue("@title", quotation.Title ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@description", quotation.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@estimatedDate", quotation.EstimatedDate);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        await cmd.ExecuteNonQueryAsync();
    }
}
