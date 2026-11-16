using Dashboard_Sales_Dev.Logic.Exceptions;
using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class ClientRepository(string connectionString)
    : RepositoryBase(connectionString), IClientRepository
{
    private const string SelectColumns =
        "id, companyName, firstName, lastName, email, phoneNumber, createdAt, updatedAt";

    public List<Client> GetAll()
    {
        List<Client> clients = [];

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = $"SELECT {SelectColumns} FROM client";

        using MySqlCommand command = new MySqlCommand(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            clients.Add(MapClient(reader));
        }

        return clients;
    }

    public Client? GetById(int id)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = $"SELECT {SelectColumns} FROM client WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapClient(reader) : null;
    }

    public async Task<int> AddClientAsync(Client client)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            INSERT INTO client
                (firstName, lastName, companyName, email, phoneNumber, createdAt)
            VALUES
                (@firstName, @lastName, @companyName, @email, @phoneNumber, @createdAt);

            SELECT LAST_INSERT_ID();";

        await using var cmd = new MySqlCommand(sql, connection);

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
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = @"
            UPDATE client
            SET firstName = @firstName,
                lastName = @lastName,
                companyName = @companyName,
                email = @email,
                phoneNumber = @phoneNumber,
                updatedAt = @updatedAt
            WHERE id = @id";

        await using var cmd = new MySqlCommand(sql, connection);

        cmd.Parameters.AddWithValue("@id", client.Id);
        cmd.Parameters.AddWithValue("@firstName", client.FirstName);
        cmd.Parameters.AddWithValue("@lastName", client.LastName);
        cmd.Parameters.AddWithValue("@companyName", client.CompanyName);
        cmd.Parameters.AddWithValue("@email", client.Email);
        cmd.Parameters.AddWithValue("@phoneNumber", client.PhoneNumber);
        cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);

        int affectedRows = await cmd.ExecuteNonQueryAsync();
        return affectedRows > 0;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        const string sql = "DELETE FROM client WHERE id = @id";

        await using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        try
        {
            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.RowIsReferenced2)
        {
            throw new ClientInUseException(id, exception);
        }
    }

    private static Client MapClient(MySqlDataReader reader)
    {
        return new Client
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
    }
}