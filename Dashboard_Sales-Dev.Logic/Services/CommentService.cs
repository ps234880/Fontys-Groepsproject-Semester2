using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class CommentService
{
    public const int MaxMessageLength = 2000;

    private readonly ICommentRepository _commentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public CommentService(ICommentRepository commentRepository, IEmployeeRepository employeeRepository)
    {
        _commentRepository = commentRepository;
        _employeeRepository = employeeRepository;
    }

    public List<Comment> GetForDeal(int dealId)
    {
        return _commentRepository.GetByDeal(dealId);
    }

    public List<Comment> GetLatest(int count)
    {
        return _commentRepository.GetLatest(count);
    }

    public async Task AddAsync(int dealId, int employeeId, string? message)
    {
        string text = (message ?? string.Empty).Trim();

        if (text.Length == 0)
        {
            throw new ArgumentException("Write a comment before you post it.");
        }

        if (text.Length > MaxMessageLength)
        {
            throw new ArgumentException($"A comment can be at most {MaxMessageLength} characters.");
        }

        if (_employeeRepository.GetById(employeeId) == null)
        {
            throw new ArgumentException("Choose who is writing the comment.");
        }

        await _commentRepository.AddCommentAsync(new Comment
        {
            DealId = dealId,
            EmployeeId = employeeId,
            Message = text
        });
    }
}
