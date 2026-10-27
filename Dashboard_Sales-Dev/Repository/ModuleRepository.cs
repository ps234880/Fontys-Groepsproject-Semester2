using Dashboard_Sales_Dev.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.Repository
{
    public class ModuleRepository
    {
        private readonly string _connectionString;

        public ModuleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Module> GetAll()
        {
            List<Module> modules = new List<Module>();

            using MySqlConnection connection = new MySqlConnection(_connectionString);
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
            using var connection = new MySqlConnection(_connectionString);
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
    }
}
