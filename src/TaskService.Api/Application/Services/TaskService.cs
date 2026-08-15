using TaskService.Api.Application.DTOs;
using TaskService.Api.Application.Interfaces;
using TaskService.Api.Domain.Entities;
using TaskService.Api.Exceptions;
using TaskService.Api.Services;

namespace TaskService.Api.Application.Services;

public sealed class TaskService(
    ITaskRepository repository,
    ILogger<TaskService> logger) : ITaskService
{
    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var task = new TaskItem
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Status = request.Status,
            OriginalEstimatedWork = request.OriginalEstimatedWork,
            RemainingWork = request.RemainingWork,
            CompletedWork = request.CompletedWork,
            CreatedAt = now,
            UpdatedAt = now
        };

        var created = await repository.CreateAsync(task, cancellationToken);
        logger.LogInformation("Created task {TaskId}", created.Id);
        return Map(created);
    }

    public async Task<TaskResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);
        return Map(task);
    }

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tasks = await repository.GetAllAsync(cancellationToken);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskResponse> UpdateAsync(string id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Status = request.Status;
        task.OriginalEstimatedWork = request.OriginalEstimatedWork;
        task.RemainingWork = request.RemainingWork;
        task.CompletedWork = request.CompletedWork;
        task.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await repository.UpdateAsync(task, cancellationToken);
        logger.LogInformation("Updated task {TaskId}", id);
        return Map(updated);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        await repository.DeleteAsync(task.Id, cancellationToken);
        logger.LogInformation("Deleted task {TaskId}", id);
    }

    private static TaskResponse Map(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.Status,
            task.OriginalEstimatedWork, task.RemainingWork,
            task.CompletedWork, task.CreatedAt, task.UpdatedAt);
}
