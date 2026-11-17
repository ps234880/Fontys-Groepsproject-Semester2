using Dashboard_Sales_Dev.Logic.Interfaces;
using Dashboard_Sales_Dev.Logic.Models;

namespace Dashboard_Sales_Dev.Logic.Services;

public class SavedFilterService
{
    public const int MaxNameLength = 100;

    private readonly ISavedFilterRepository _savedFilterRepository;

    public SavedFilterService(ISavedFilterRepository savedFilterRepository)
    {
        _savedFilterRepository = savedFilterRepository;
    }

    public List<SavedFilter> GetForEmployee(int employeeId)
    {
        return _savedFilterRepository.GetForEmployee(employeeId);
    }

    public async Task SaveAsync(int employeeId, string? name, DealFilter filter)
    {
        string trimmed = (name ?? string.Empty).Trim();

        if (trimmed.Length == 0 || trimmed.Length > MaxNameLength)
        {
            throw new ArgumentException($"Give the filter a name of 1 to {MaxNameLength} characters.");
        }

        if (filter.IsEmpty)
        {
            throw new ArgumentException("Choose an owner or a stage before you save the filter.");
        }

        if (GetForEmployee(employeeId).Any(saved => string.Equals(saved.Name, trimmed, StringComparison.CurrentCultureIgnoreCase)))
        {
            throw new ArgumentException($"You already have a filter called '{trimmed}'. Choose another name.");
        }

        await _savedFilterRepository.AddAsync(new SavedFilter
        {
            EmployeeId = employeeId,
            Name = trimmed,
            Owner = string.IsNullOrEmpty(filter.Owner) ? null : filter.Owner,
            Stage = string.IsNullOrEmpty(filter.Stage) ? null : filter.Stage,
            CreatedAt = DateTime.Now
        });
    }

    public Task DeleteAsync(int employeeId, int filterId)
    {
        return _savedFilterRepository.DeleteAsync(employeeId, filterId);
    }
}
