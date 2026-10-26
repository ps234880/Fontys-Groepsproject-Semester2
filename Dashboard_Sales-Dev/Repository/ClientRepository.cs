using MySqlConnector;
using Dashboard_Sales_Dev.Models;

namespace Dashboard_Sales_Dev.Repository
{
    public class ClientRepository
    {
        private readonly string _connectionString;

        public ClientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task AddClientAsync(Client client)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"INSERT INTO client (FirstName, LastName, CompanyName, Email, PhoneNumber, CreatedAt) 
                        VALUES (@FirstName, @LastName, @CompanyName, @Email, @PhoneNumber, @CreatedAt)";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", client.LastName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CompanyName", client.CompanyName);
            cmd.Parameters.AddWithValue("@Email", client.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}