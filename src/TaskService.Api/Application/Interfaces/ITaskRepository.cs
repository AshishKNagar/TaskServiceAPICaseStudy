using TaskService.Api.Domain.Entities;

namespace TaskService.Api.Application.Interfaces;

public interface ITaskRepository
{
    /// <summary>
    /// Gets a task by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<TaskItem?> GetByIdAsync(string id, CancellationToken cancellationToken);
   
    /// <summary>
    /// Gets all tasks.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<IReadOnlyList<TaskItem>?> GetAllAsync(CancellationToken cancellationToken);
   
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<TaskItem?> CreateAsync(TaskItem task, CancellationToken cancellationToken);
    
    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<TaskItem?> UpdateAsync(TaskItem task, CancellationToken cancellationToken);
    
    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
