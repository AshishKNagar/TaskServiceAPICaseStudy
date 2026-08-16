using TaskService.Api.Application.DTOs;

namespace TaskService.Api.Services;

public interface ITaskService
{
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<TaskResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
   
    /// <summary>
    ///  Gets all tasks.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken);
    
    /// <summary>
    ///     Updates an existing task.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<TaskResponse> UpdateAsync(string id, UpdateTaskRequest request, CancellationToken cancellationToken);
    
    /// <summary>
    ///     Deletes a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
