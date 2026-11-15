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

    public List<Module> GetModules(int quotationId)
    {
        List<Module> modules = new List<Module>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = @"SELECT m.id, m.name, m.description, m.priceInCents, m.productionTimeInWeeks, m.createdAt
FROM quotation_module qm
INNER JOIN module m ON qm.module_id = m.id
WHERE qm.quotation_id = @quotationId
ORDER BY m.name;";

        using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@quotationId", quotationId);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            modules.Add(new Module
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description"),
                PriceInCents = reader.GetInt32("priceInCents"),
                ProductionTimeInWeeks = reader.GetInt32("productionTimeInWeeks"),
                CreatedAt = reader.GetDateTime("createdAt")
            });
        }

        return modules;
    }

    public async Task AddModuleAsync(int quotationId, int moduleId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("INSERT IGNORE INTO quotation_module (quotation_id, module_id) VALUES (@quotationId, @moduleId);", connection);
        command.Parameters.AddWithValue("@quotationId", quotationId);
        command.Parameters.AddWithValue("@moduleId", moduleId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveModuleAsync(int quotationId, int moduleId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("DELETE FROM quotation_module WHERE quotation_id = @quotationId AND module_id = @moduleId;", connection);
        command.Parameters.AddWithValue("@quotationId", quotationId);
        command.Parameters.AddWithValue("@moduleId", moduleId);

        await command.ExecuteNonQueryAsync();
    }
}
