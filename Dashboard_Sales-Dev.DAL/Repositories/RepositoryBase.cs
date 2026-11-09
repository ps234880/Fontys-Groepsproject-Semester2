using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public abstract class RepositoryBase
{
    private readonly string _connectionString;

    protected RepositoryBase(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
