namespace TaskManagement.Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
