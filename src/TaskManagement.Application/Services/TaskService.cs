using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public sealed class TaskService(ITaskRepository repository) : ITaskService
{
    public async Task<IReadOnlyList<TaskResponse>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await repository.GetAllAsync(cancellationToken);
        return tasks.Select(ToResponse).ToList();
    }

    public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description
        };

        var created = await repository.AddAsync(task, cancellationToken);
        return ToResponse(created);
    }

    public async Task<TaskResponse?> ToggleTaskAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.IsCompleted = !task.IsCompleted;
        await repository.UpdateAsync(task, cancellationToken);
        return ToResponse(task);
    }

    private static TaskResponse ToResponse(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.IsCompleted, task.CreatedAt);
}
