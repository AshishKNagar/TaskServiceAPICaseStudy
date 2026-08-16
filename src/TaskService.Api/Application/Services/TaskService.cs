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
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="TaskCreateFailException"></exception>
    
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

        var created = await repository.CreateAsync(task, cancellationToken)
            ?? throw new TaskCreateFailException(task.Title);
        logger.LogInformation("Created task {Title}", task?.Title);
        return Map(created);
    }

    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="TaskNotFoundException"></exception>
    
    public async Task<TaskResponse> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);
        return Map(task);
    }

    /// <summary>
    /// Gets all tasks.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="TaskListNotFoundException"></exception>
   
    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tasks = await repository.GetAllAsync(cancellationToken)
             ?? throw new TaskListNotFoundException(); ;
        return tasks.Select(Map).ToList();
    }

    /// <summary>
    /// Updates a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="TaskNotFoundException"></exception>
    
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

        var updated = await repository.UpdateAsync(task, cancellationToken)
            ?? throw new TaskNotFoundException(id);
        logger.LogInformation("Updated task {TaskId}", id);
        return Map(updated);
    }

    /// <summary>
    ///     Deletes a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="TaskNotFoundException"></exception>
    
    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var task = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new TaskNotFoundException(id);

        await repository.DeleteAsync(task.Id, cancellationToken);
        logger.LogInformation("Deleted task {TaskId}", id);
    }

    /// <summary>
    /// Maps a TaskItem entity to a TaskResponse DTO.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    
    private static TaskResponse Map(TaskItem task) =>
        new(task.Id, task.Title, task.Description, task.Status,
            task.OriginalEstimatedWork, task.RemainingWork,
            task.CompletedWork, task.CreatedAt, task.UpdatedAt);
}
