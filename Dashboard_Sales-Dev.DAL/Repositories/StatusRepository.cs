using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class StatusRepository : RepositoryBase, IStatusRepository
{
    public StatusRepository(string connectionString) : base(connectionString)
    {
    }

    public List<Status> GetAll()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand("SELECT id, name FROM status ORDER BY id;", connection);

        return ReadStatuses(command);
    }

    public Status? GetById(int id)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand("SELECT id, name FROM status WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);

        return ReadStatuses(command).FirstOrDefault();
    }

    public Status? GetByName(string name)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand("SELECT id, name FROM status WHERE name = @name;", connection);
        command.Parameters.AddWithValue("@name", name);

        return ReadStatuses(command).FirstOrDefault();
    }

    private static List<Status> ReadStatuses(MySqlCommand command)
    {
        List<Status> statuses = new List<Status>();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            statuses.Add(new Status
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name")
            });
        }

        return statuses;
    }
}
