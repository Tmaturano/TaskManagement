using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.Tasks.Add(task);
        await context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync(cancellationToken);
    }
}
