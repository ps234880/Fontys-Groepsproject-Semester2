using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class SavedFilterRepository : RepositoryBase, ISavedFilterRepository
{
    public SavedFilterRepository(string connectionString) : base(connectionString)
    {
    }

    public List<SavedFilter> GetForEmployee(int employeeId)
    {
        List<SavedFilter> filters = new List<SavedFilter>();

        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(@"SELECT id, employee_id, name, owner, stage, createdAt
FROM saved_filter
WHERE employee_id = @employeeId
ORDER BY name;", connection);
        command.Parameters.AddWithValue("@employeeId", employeeId);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            filters.Add(new SavedFilter
            {
                Id = reader.GetInt32("id"),
                EmployeeId = reader.GetInt32("employee_id"),
                Name = reader.GetString("name"),
                Owner = reader.IsDBNull(reader.GetOrdinal("owner")) ? null : reader.GetString("owner"),
                Stage = reader.IsDBNull(reader.GetOrdinal("stage")) ? null : reader.GetString("stage"),
                CreatedAt = reader.GetDateTime("createdAt")
            });
        }

        return filters;
    }

    public async Task AddAsync(SavedFilter filter)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand(@"INSERT INTO saved_filter (employee_id, name, owner, stage, createdAt)
VALUES (@employeeId, @name, @owner, @stage, @createdAt);", connection);
        command.Parameters.AddWithValue("@employeeId", filter.EmployeeId);
        command.Parameters.AddWithValue("@name", filter.Name);
        command.Parameters.AddWithValue("@owner", filter.Owner ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@stage", filter.Stage ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", filter.CreatedAt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int employeeId, int filterId)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("DELETE FROM saved_filter WHERE id = @id AND employee_id = @employeeId;", connection);
        command.Parameters.AddWithValue("@id", filterId);
        command.Parameters.AddWithValue("@employeeId", employeeId);

        await command.ExecuteNonQueryAsync();
    }
}
