using Dashboard_Sales_Dev.Models;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace Dashboard_Sales_Dev.Repository;

public class ClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection"!);
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
}