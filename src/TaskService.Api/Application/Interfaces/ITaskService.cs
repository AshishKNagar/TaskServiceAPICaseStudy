using TaskService.Api.Application.DTOs;

namespace TaskService.Api.Services;

public interface ITaskService
{
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);
    Task<TaskResponse> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskResponse> UpdateAsync(string id, UpdateTaskRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
