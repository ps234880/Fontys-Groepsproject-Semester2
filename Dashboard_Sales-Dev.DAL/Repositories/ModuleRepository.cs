using Dashboard_Sales_Dev.Logic.Exceptions;
using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class ModuleRepository : RepositoryBase, IModuleRepository
{
    public ModuleRepository(string connectionString) : base(connectionString)
    {
    }

    public List<Module> GetAll()
    {
        List<Module> modules = new List<Module>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = @"SELECT id, name, description, priceInCents, 
                productionTimeInWeeks, createdAt, updatedAt 
        FROM module";

        using MySqlCommand command = new MySqlCommand(query, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Module module = new Module
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Description = reader.GetString("description"),
                PriceInCents = reader.GetInt32("priceInCents"),
                ProductionTimeInWeeks = reader.GetInt32("productionTimeInWeeks"),
                CreatedAt = reader.GetDateTime("createdAt")
            };

            modules.Add(module);
        }

        return modules;
    }

    public async Task AddModuleAsync(Module module)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var sql = @"INSERT INTO module (name, description, 
                           priceInCents, productionTimeInWeeks, createdAt) 
                   VALUES (@name, @description, @priceInCents, @productionTimeInWeeks, @CreatedAt)";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@name", module.Name);
        cmd.Parameters.AddWithValue("@description", module.Description);
        cmd.Parameters.AddWithValue("@priceInCents", module.PriceInCents);
        cmd.Parameters.AddWithValue("@productionTimeInWeeks", module.ProductionTimeInWeeks);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        await cmd.ExecuteNonQueryAsync();
    }

    public Module? GetById(int id)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        string query = @"SELECT id, name, description, priceInCents,
                productionTimeInWeeks, createdAt, updatedAt
        FROM module
        WHERE id = @id";

        using MySqlCommand command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        using MySqlDataReader reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new Module
        {
            Id = reader.GetInt32("id"),
            Name = reader.GetString("name"),
            Description = reader.GetString("description"),
            PriceInCents = reader.GetInt32("priceInCents"),
            ProductionTimeInWeeks = reader.GetInt32("productionTimeInWeeks"),
            CreatedAt = reader.GetDateTime("createdAt"),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updatedAt"))
                ? null
                : reader.GetDateTime("updatedAt")
        };
    }

    public async Task<bool> UpdateModuleAsync(Module module)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var sql = @"UPDATE module
                   SET name = @name,
                       description = @description,
                       priceInCents = @priceInCents,
                       productionTimeInWeeks = @productionTimeInWeeks,
                       updatedAt = @updatedAt
                   WHERE id = @id";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", module.Id);
        cmd.Parameters.AddWithValue("@name", module.Name);
        cmd.Parameters.AddWithValue("@description", module.Description);
        cmd.Parameters.AddWithValue("@priceInCents", module.PriceInCents);
        cmd.Parameters.AddWithValue("@productionTimeInWeeks", module.ProductionTimeInWeeks);
        cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);

        int affectedRows = await cmd.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }

    public async Task<bool> DeleteModuleAsync(int id)
    {
        using var connection = CreateConnection();
        await connection.OpenAsync();

        var sql = "DELETE FROM module WHERE id = @id";

        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        try
        {
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            return affectedRows > 0;
        }
        catch (MySqlException exception) when (exception.ErrorCode == MySqlErrorCode.RowIsReferenced2)
        {
            throw new ModuleInUseException(id, exception);
        }
    }
}
