using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse?> ToggleTaskAsync(Guid id, CancellationToken cancellationToken = default);
}
