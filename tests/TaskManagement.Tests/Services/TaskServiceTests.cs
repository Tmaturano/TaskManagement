using TaskManagement.Application.Services;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using Xunit;
using AwesomeAssertions;

namespace TaskManagement.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateTask_ShouldReturnCreatedTask()
    {
        // Arrange
        var repo = new InMemoryTaskRepository();
        var service = new TaskService(repo);
        var request = new CreateTaskRequest("Test Task", "desc");

        // Act
        var result = await service.CreateTaskAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Title.Should().Be(request.Title);
        result.Description.Should().Be(request.Description);
        result.IsCompleted.Should().BeFalse();
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetAllTasks_ShouldReturnAll()
    {
        // Arrange
        var repo = new InMemoryTaskRepository();
        var service = new TaskService(repo);
        await service.CreateTaskAsync(new CreateTaskRequest("A", null), TestContext.Current.CancellationToken);
        await service.CreateTaskAsync(new CreateTaskRequest("B", null), TestContext.Current.CancellationToken);

        // Act
        var all = await service.GetAllTasksAsync(TestContext.Current.CancellationToken);

        // Assert
        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task ToggleTask_ShouldFlipCompleted()
    {
        // Arrange
        var repo = new InMemoryTaskRepository();
        var service = new TaskService(repo);
        var created = await service.CreateTaskAsync(new CreateTaskRequest("ToggleMe", null), TestContext.Current.CancellationToken);

        // Act
        var toggled = await service.ToggleTaskAsync(created.Id, TestContext.Current.CancellationToken);

        // Assert
        toggled.Should().NotBeNull();
        toggled!.IsCompleted.Should().BeTrue();

        // Act again
        var toggledBack = await service.ToggleTaskAsync(created.Id, TestContext.Current.CancellationToken);
        toggledBack!.IsCompleted.Should().BeFalse();
    }

    // Simple in-memory repo for service tests
    private class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _items = new();

        public Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((IReadOnlyList<TaskItem>)_items.OrderByDescending(t => t.CreatedAt).ToList());

        public Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            _items.Add(task);
            return Task.FromResult(task);
        }

        public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

        public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
        {
            var idx = _items.FindIndex(x => x.Id == task.Id);
            if (idx >= 0)
            {
                _items[idx] = task;
            }

            return Task.CompletedTask;
        }
    }
}
