using MySqlConnector;
using Dashboard_Sales_Dev.Models;

namespace Dashboard_Sales_Dev.Repository;

public class ClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public List<Client> GetAll()
    {
        List<Client> clients = new List<Client>();

        using MySqlConnection connection = new MySqlConnection(_connectionString);
        connection.Open();

        string query =
            @"SELECT id, companyName, firstName, lastName, 
                    email, phoneNumber, createdAt, updatedAt 
            FROM client";

        using MySqlCommand command = new MySqlCommand(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Client client = new Client
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
            };
            clients.Add((client));
        }

        return clients;
    }

    public async Task<int> AddClientAsync(Client client)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var sql = @"
        INSERT INTO client
            (firstName, lastName, companyName, email, phoneNumber, createdAt)
        VALUES
            (@firstName, @lastName, @companyName, @email, @phoneNumber, @createdAt);

        SELECT LAST_INSERT_ID();
    ";

        using var cmd = new MySqlCommand(sql, connection);

        cmd.Parameters.AddWithValue("@firstName", client.FirstName);
        cmd.Parameters.AddWithValue("@lastName", client.LastName);
        cmd.Parameters.AddWithValue("@companyName", client.CompanyName);
        cmd.Parameters.AddWithValue("@email", client.Email);
        cmd.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
        cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

        object? result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}