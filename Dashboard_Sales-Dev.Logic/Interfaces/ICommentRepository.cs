using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Interfaces;

public interface ICommentRepository
{
    List<Comment> GetByQuotation(int quotationId);
    List<Comment> GetLatest(int count);
    Task AddCommentAsync(Comment comment);
}
