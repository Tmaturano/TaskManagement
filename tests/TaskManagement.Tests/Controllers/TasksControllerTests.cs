using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Controllers;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using Xunit;

namespace TaskManagement.Tests.Controllers;

public class TasksControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        // Arrange
        var service = new FakeService();
        service.Tasks.Add(new TaskResponse(Guid.NewGuid(), "T1", null, false, DateTime.UtcNow));
        var controller = new TasksController(service);

        // Act
        var result = await controller.GetAll(TestContext.Current.CancellationToken);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IReadOnlyList<TaskResponse>>(ok.Value);
        Assert.Single(list);
    }

    [Fact]
    public async Task Create_ReturnsCreated()
    {
        // Arrange
        var service = new FakeService();
        var controller = new TasksController(service);
        var req = new CreateTaskRequest("New", null);

        // Act
        var result = await controller.Create(req, TestContext.Current.CancellationToken);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<TaskResponse>(created.Value!);
        Assert.Equal(req.Title, value.Title);
    }

    [Fact]
    public async Task Toggle_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var service = new FakeService();
        var controller = new TasksController(service);

        // Act
        var result = await controller.Toggle(Guid.NewGuid(), TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Toggle_ReturnsOk_WhenFound()
    {
        // Arrange
        var service = new FakeService();
        var id = Guid.NewGuid();
        service.Tasks.Add(new TaskResponse(id, "One", null, false, DateTime.UtcNow));
        var controller = new TasksController(service);

        // Act
        var result = await controller.Toggle(id, TestContext.Current.CancellationToken);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var resp = Assert.IsType<TaskResponse>(ok.Value!);
        Assert.True(resp.IsCompleted);
    }

    private class FakeService : ITaskService
    {
        public List<TaskResponse> Tasks { get; } = new();

        public Task<IReadOnlyList<TaskResponse>> GetAllTasksAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((IReadOnlyList<TaskResponse>)Tasks);

        public Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var t = new TaskResponse(Guid.NewGuid(), request.Title, request.Description, false, DateTime.UtcNow);
            Tasks.Add(t);
            return Task.FromResult(t);
        }

        public Task<TaskResponse?> ToggleTaskAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var idx = Tasks.FindIndex(x => x.Id == id);
            if (idx < 0) return Task.FromResult<TaskResponse?>(null);
            var old = Tasks[idx];
            var updated = old with { IsCompleted = !old.IsCompleted };
            Tasks[idx] = updated;
            return Task.FromResult<TaskResponse?>(updated);
        }
    }
}
