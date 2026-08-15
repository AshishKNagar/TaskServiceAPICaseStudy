using TaskService.Api.Domain.Entities;

namespace TaskService.Api.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskItem> CreateAsync(TaskItem task, CancellationToken cancellationToken);
    Task<TaskItem> UpdateAsync(TaskItem task, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
