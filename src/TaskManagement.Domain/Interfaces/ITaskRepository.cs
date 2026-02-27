using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
}
