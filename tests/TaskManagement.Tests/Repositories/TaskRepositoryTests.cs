using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Domain.Entities;
using Xunit;
using AwesomeAssertions;

namespace TaskManagement.Tests.Repositories;

public class TaskRepositoryTests
{
    [Fact]
    public async Task AddAndGetAll_ShouldPersistAndReturn()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var ctx = new AppDbContext(options);
        var repo = new TaskRepository(ctx);

        var task = new TaskItem { Title = "RepoTask", Description = "d" };

        // Act
        var added = await repo.AddAsync(task, TestContext.Current.CancellationToken);
        var all = await repo.GetAllAsync(TestContext.Current.CancellationToken);

        // Assert
        all.Should().HaveCount(1);
        all[0].Id.Should().Be(added.Id);
    }

    [Fact]
    public async Task Update_ShouldPersistChanges()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var ctx = new AppDbContext(options);
        var repo = new TaskRepository(ctx);

        var task = new TaskItem { Title = "ToToggle" };
        await repo.AddAsync(task, TestContext.Current.CancellationToken);

        // Act
        var fetched = await repo.GetByIdAsync(task.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(fetched);
        fetched!.IsCompleted = true;
        await repo.UpdateAsync(fetched, TestContext.Current.CancellationToken);

        var reloaded = await repo.GetByIdAsync(task.Id, TestContext.Current.CancellationToken);

        // Assert
        reloaded!.IsCompleted.Should().BeTrue();
    }
}
