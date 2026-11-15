using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;
using MySqlConnector;

namespace Dashboard_Sales_Dev.DAL.Repositories;

public class CommentRepository : RepositoryBase, ICommentRepository
{
    public CommentRepository(string connectionString) : base(connectionString)
    {
    }

    private const string SelectComment = @"SELECT
    cm.id,
    cm.quotation_id,
    cm.employee_id,
    cm.message,
    cm.createdAt,
    e.firstName,
    e.lastName,
    r.name AS roleName,
    q.title,
    c.companyName
FROM comment cm
INNER JOIN employee e ON cm.employee_id = e.id
INNER JOIN role r ON e.role_id = r.id
INNER JOIN quotation q ON cm.quotation_id = q.id
INNER JOIN client c ON q.client_id = c.id";

    public List<Comment> GetByQuotation(int quotationId)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectComment + " WHERE cm.quotation_id = @quotationId ORDER BY cm.createdAt, cm.id;", connection);
        command.Parameters.AddWithValue("@quotationId", quotationId);

        return ReadComments(command);
    }

    public List<Comment> GetLatest(int count)
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        using MySqlCommand command = new MySqlCommand(SelectComment + " ORDER BY cm.createdAt DESC, cm.id DESC LIMIT @count;", connection);
        command.Parameters.AddWithValue("@count", count);

        return ReadComments(command);
    }

    public async Task AddCommentAsync(Comment comment)
    {
        using MySqlConnection connection = CreateConnection();
        await connection.OpenAsync();

        using MySqlCommand command = new MySqlCommand("INSERT INTO comment (employee_id, quotation_id, message, createdAt) VALUES (@employeeId, @quotationId, @message, @createdAt);", connection);
        command.Parameters.AddWithValue("@employeeId", comment.EmployeeId);
        command.Parameters.AddWithValue("@quotationId", comment.QuotationId);
        command.Parameters.AddWithValue("@message", comment.Message);
        command.Parameters.AddWithValue("@createdAt", DateTime.Now);

        await command.ExecuteNonQueryAsync();
    }

    private static List<Comment> ReadComments(MySqlCommand command)
    {
        List<Comment> comments = new List<Comment>();
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            comments.Add(new Comment
            {
                Id = reader.GetInt32("id"),
                QuotationId = reader.GetInt32("quotation_id"),
                EmployeeId = reader.GetInt32("employee_id"),
                Message = reader.GetString("message"),
                CreatedAt = reader.GetDateTime("createdAt"),
                EmployeeName = reader.GetString("firstName") + " " + reader.GetString("lastName"),
                EmployeeRole = reader.GetString("roleName"),
                QuotationTitle = reader.GetString("title"),
                CompanyName = reader.GetString("companyName")
            });
        }

        return comments;
    }
}
