using Dashboard_Sales_Dev.Logic.Exceptions;
using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class ClientRepository : RepositoryBase, IClientRepository
{
    public ClientRepository(string connectionString) : base(connectionString)
    {
    }

    public List<Client> GetAll()
    {
        List<Client> clients = new List<Client>();

        using MySqlConnection connection = CreateConnection();
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
        using var connection = CreateConnection();
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


    public async Task<bool> UpdateClientAsync(Client client)
    {
        using var connection = CreateConnection();
        connection.Open();

        var sql =
            @"UPDATE client set firstName = @firstName, lastName = @lastName, companyName = @companyName, email = @email, phoneNumber = @phoneNumber, updatedAt  = @updatedAt WHERE id = @id ";

        await using var cmd = new MySqlCommand(sql, connection);

        cmd.Parameters.AddWithValue("@firstName", client.FirstName);
        cmd.Parameters.AddWithValue("@lastName", client.LastName);
        cmd.Parameters.AddWithValue("@companyName", client.CompanyName);
        cmd.Parameters.AddWithValue("@email", client.Email);
        cmd.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
        cmd.Parameters.AddWithValue("@updateAt", DateTime.Now);

        int affectedRows = await cmd.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }


    public async Task<bool> DeleteClientAsync(int id)
    {
        var connection = CreateConnection();
        await connection.OpenAsync();

        var sql = "DELETE FROM client WHERE id = @id";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        try
        {
            int AffectedRows = await cmd.ExecuteNonQueryAsync();

            return AffectedRows > 0;
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.RowIsReferenced2)
        {
            throw new ModuleInUseException(id, exception);
        }
    }
}