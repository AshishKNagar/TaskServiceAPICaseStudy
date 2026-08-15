using Microsoft.Extensions.Logging;
using Moq;
using TaskService.Api.Application.DTOs;
using TaskService.Api.Application.Interfaces;
using TaskService.Api.Domain.Entities;
using TaskService.Api.Domain.Enums;
using TaskService.Api.Exceptions;
using Xunit;

namespace TaskService.Tests.Services;

public sealed class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repository = new();
    private readonly Mock<ILogger<Api.Application.Services.TaskService>> _logger = new();

    private Api.Application.Services.TaskService CreateService() => new(_repository.Object, _logger.Object);

    [Fact]
    public async Task CreateAsync_ShouldCreateTask()
    {
        var request = new CreateTaskRequest
        {
            Title = "  Test task  ",
            Description = "Test description",
            OriginalEstimatedWork = 8,
            RemainingWork = 8,
            CompletedWork = 0
        };

        _repository.Setup(x => x.CreateAsync(
                It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem task, CancellationToken _) => task);

        var result = await CreateService().CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Id);
        Assert.Equal("Test task", result.Title);
        Assert.Equal(8, result.OriginalEstimatedWork);
        _repository.Verify(x => x.CreateAsync(
            It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAndUpdatedAt()
    {
        _repository.Setup(x => x.CreateAsync(
                It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem task, CancellationToken _) => task);

        var result = await CreateService().CreateAsync(
            new CreateTaskRequest { Title = "Test" }, CancellationToken.None);

        Assert.NotEqual(default, result.CreatedAt);
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task CreateAsync_ShouldAllowIndependentWorkValues()
    {
        _repository.Setup(x => x.CreateAsync(
                It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem task, CancellationToken _) => task);

        var result = await CreateService().CreateAsync(new CreateTaskRequest
        {
            Title = "Work",
            OriginalEstimatedWork = 5,
            RemainingWork = 10,
            CompletedWork = 7
        }, CancellationToken.None);

        Assert.Equal(5, result.OriginalEstimatedWork);
        Assert.Equal(10, result.RemainingWork);
        Assert.Equal(7, result.CompletedWork);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask()
    {
        var task = new TaskItem { Id = "1", Title = "Existing" };
        _repository.Setup(x => x.GetByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var result = await CreateService().GetByIdAsync("1", CancellationToken.None);

        Assert.Equal("1", result.Id);
        Assert.Equal("Existing", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ShouldThrow()
    {
        _repository.Setup(x => x.GetByIdAsync(
                "missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            CreateService().GetByIdAsync("missing", CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedTasks()
    {
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new TaskItem { Id = "1", Title = "One" },
                new TaskItem { Id = "2", Title = "Two" }
            });

        var result = await CreateService().GetAllAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Equal("One", result[0].Title);
        Assert.Equal("Two", result[1].Title);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ShouldReturnEmpty()
    {
        _repository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TaskItem>());

        var result = await CreateService().GetAllAsync(CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFields()
    {
        var task = new TaskItem
        {
            Id = "1", Title = "Old", Status = TaskItemStatus.Todo,
            OriginalEstimatedWork = 5, RemainingWork = 5
        };

        _repository.Setup(x => x.GetByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repository.Setup(x => x.UpdateAsync(
                It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem value, CancellationToken _) => value);

        var result = await CreateService().UpdateAsync("1",
            new UpdateTaskRequest
            {
                Title = "Updated",
                Status = TaskItemStatus.InProgress,
                OriginalEstimatedWork = 10,
                RemainingWork = 3,
                CompletedWork = 7
            }, CancellationToken.None);

        Assert.Equal("Updated", result.Title);
        Assert.Equal(TaskItemStatus.InProgress, result.Status);
        Assert.Equal(10, result.OriginalEstimatedWork);
        Assert.Equal(3, result.RemainingWork);
        Assert.Equal(7, result.CompletedWork);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreserveCreatedAtAndChangeUpdatedAt()
    {
        var created = DateTimeOffset.UtcNow.AddHours(-1);
        var task = new TaskItem
        {
            Id = "1", Title = "Old", CreatedAt = created, UpdatedAt = created
        };

        _repository.Setup(x => x.GetByIdAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _repository.Setup(x => x.UpdateAsync(
                It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem value, CancellationToken _) => value);

        var result = await CreateService().UpdateAsync("1",
            new UpdateTaskRequest { Title = "Updated" }, CancellationToken.None);

        Assert.Equal(created, result.CreatedAt);
        Assert.True(result.UpdatedAt > created);
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ShouldThrowAndNotUpdate()
    {
        _repository.Setup(x => x.GetByIdAsync(
                "missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            CreateService().UpdateAsync("missing",
                new UpdateTaskRequest { Title = "Updated" }, CancellationToken.None));

        _repository.Verify(x => x.UpdateAsync(
            It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenExists_ShouldDelete()
    {
        _repository.Setup(x => x.GetByIdAsync(
                "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TaskItem { Id = "1", Title = "Delete" });

        await CreateService().DeleteAsync("1", CancellationToken.None);

        _repository.Verify(x => x.DeleteAsync(
            "1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ShouldThrowAndNotDelete()
    {
        _repository.Setup(x => x.GetByIdAsync(
                "missing", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        await Assert.ThrowsAsync<TaskNotFoundException>(() =>
            CreateService().DeleteAsync("missing", CancellationToken.None));

        _repository.Verify(x => x.DeleteAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
