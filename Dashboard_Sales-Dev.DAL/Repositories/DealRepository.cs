using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class DealRepository : RepositoryBase, IDealRepository
{
    public DealRepository(string connectionString) : base(connectionString)
    {
    }

    private const string SelectDeal = @"SELECT
    d.id,
    d.client_id,
    d.employee_id,
    d.status_id,
    d.title,
    d.requirement,
    d.estimatedDate,
    d.createdAt,
    s.name AS statusName,
    w.name AS waitingRoleName,
    c.companyName,
    c.firstName AS clientFirstName,
    c.lastName AS clientLastName,
    c.email AS clientEmail,
    c.phoneNumber AS clientPhoneNumber,
    e.firstName AS employeeFirstName,
    e.lastName AS employeeLastName
FROM deal d
INNER JOIN client c ON d.client_id = c.id
INNER JOIN employee e ON d.employee_id = e.id
INNER JOIN status s ON d.status_id = s.id
LEFT JOIN role w ON d.waiting_role_id = w.id";

    public List<Deal> GetAll()
    {
        List<Deal> deals = new List<Deal>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectDeal + " ORDER BY d.createdAt DESC;", connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            deals.Add(MapDeal(reader));
        }

        return deals;
    }

    public Deal? GetById(int id)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectDeal + " WHERE d.id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapDeal(reader) : null;
    }

    private static Deal MapDeal(MySqlDataReader reader)
    {
        return new Deal
        {
            Id = reader.GetInt32("id"),
            ClientId = reader.GetInt32("client_id"),
            EmployeeId = reader.GetInt32("employee_id"),
            StatusId = reader.GetInt32("status_id"),
            StatusName = reader.GetString("statusName"),
            WaitingOnRole = reader.IsDBNull(reader.GetOrdinal("waitingRoleName")) ? null : reader.GetString("waitingRoleName"),
            Title = reader.GetString("title"),
            Requirement = reader.GetString("requirement"),
            EstimatedDate = reader.GetDateTime("estimatedDate"),
            CreatedAt = reader.GetDateTime("createdAt"),
            ClientCompanyName = reader.GetString("companyName"),
            ClientName = reader.GetString("clientFirstName") + " " + reader.GetString("clientLastName"),
            ClientEmail = reader.GetString("clientEmail"),
            ClientPhoneNumber = reader.GetString("clientPhoneNumber"),
            EmployeeName = reader.GetString("employeeFirstName") + " " + reader.GetString("employeeLastName")
        };
    }

    public async Task<int> AddDealAsync(Deal deal)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var sql = @"INSERT INTO deal (client_id, employee_id, status_id, title, requirement,
                               estimatedDate, createdAt)
                       VALUES (@client_id, @employee_id, @status_id, @title, @requirement, @estimatedDate, @CreatedAt);
                    SELECT LAST_INSERT_ID();";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@client_id", deal.ClientId);
        cmd.Parameters.AddWithValue("@employee_id", deal.EmployeeId);
        cmd.Parameters.AddWithValue("@status_id", deal.StatusId);
        cmd.Parameters.AddWithValue("@title", deal.Title ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@requirement", deal.Requirement ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@estimatedDate", deal.EstimatedDate);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        object? result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task UpdateStatusAsync(int dealId, int statusId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("UPDATE deal SET status_id = @statusId, updatedAt = @updatedAt WHERE id = @dealId;", connection);
        command.Parameters.AddWithValue("@statusId", statusId);
        command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
        command.Parameters.AddWithValue("@dealId", dealId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateWaitingOnAsync(int dealId, string? roleName)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand(@"UPDATE deal
SET waiting_role_id = (SELECT id FROM role WHERE name = @roleName), updatedAt = @updatedAt
WHERE id = @dealId;", connection);
        command.Parameters.AddWithValue("@roleName", roleName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
        command.Parameters.AddWithValue("@dealId", dealId);

        await command.ExecuteNonQueryAsync();
    }

    public List<Module> GetModules(int dealId)
    {
        List<Module> modules = new List<Module>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = @"SELECT m.id, m.name, m.description, m.priceInCents, m.productionTimeInWeeks, m.createdAt
FROM deal_module dm
INNER JOIN module m ON dm.module_id = m.id
WHERE dm.deal_id = @dealId
ORDER BY m.name;";

        using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@dealId", dealId);
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

    public async Task AddModuleAsync(int dealId, int moduleId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("INSERT IGNORE INTO deal_module (deal_id, module_id) VALUES (@dealId, @moduleId);", connection);
        command.Parameters.AddWithValue("@dealId", dealId);
        command.Parameters.AddWithValue("@moduleId", moduleId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveModuleAsync(int dealId, int moduleId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("DELETE FROM deal_module WHERE deal_id = @dealId AND module_id = @moduleId;", connection);
        command.Parameters.AddWithValue("@dealId", dealId);
        command.Parameters.AddWithValue("@moduleId", moduleId);

        await command.ExecuteNonQueryAsync();
    }

    public List<Client> GetClients(int dealId)
    {
        List<Client> clients = new List<Client>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = @"SELECT c.id, c.companyName, c.firstName, c.lastName,
                            c.email, c.phoneNumber, c.createdAt, c.updatedAt
FROM deal_client dc
INNER JOIN client c ON dc.client_id = c.id
WHERE dc.deal_id = @dealId
ORDER BY c.lastName, c.firstName;";

        using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@dealId", dealId);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            clients.Add(new Client
            {
                Id = reader.GetInt32("id"),
                CompanyName = reader.GetString("companyName"),
                FirstName = reader.GetString("firstName"),
                LastName = reader.GetString("lastName"),
                Email = reader.GetString("email"),
                PhoneNumber = reader.GetString("phoneNumber"),
                CreatedAt = reader.GetDateTime("createdAt"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updatedAt"))
                    ? null
                    : reader.GetDateTime("updatedAt")
            });
        }

        return clients;
    }

    public async Task AddClientAsync(int dealId, int clientId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("INSERT IGNORE INTO deal_client (deal_id, client_id) VALUES (@dealId, @clientId);", connection);
        command.Parameters.AddWithValue("@dealId", dealId);
        command.Parameters.AddWithValue("@clientId", clientId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveClientAsync(int dealId, int clientId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("DELETE FROM deal_client WHERE deal_id = @dealId AND client_id = @clientId;", connection);
        command.Parameters.AddWithValue("@dealId", dealId);
        command.Parameters.AddWithValue("@clientId", clientId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task ReplacePrimaryClientAsync(int dealId, int newClientId)
    {
        await using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        const string sql = @"
            UPDATE deal SET client_id = @newClientId, updatedAt = @updatedAt WHERE id = @dealId;
            DELETE FROM deal_client WHERE deal_id = @dealId AND client_id = @newClientId;";

        await using MySqlCommand command = new MySqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@dealId", dealId);
        command.Parameters.AddWithValue("@newClientId", newClientId);
        command.Parameters.AddWithValue("@updatedAt", DateTime.Now);

        await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }
}