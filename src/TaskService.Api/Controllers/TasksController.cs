using Microsoft.AspNetCore.Mvc;
using TaskService.Api.Application.DTOs;
using TaskService.Api.Services;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(ITaskService service) : ControllerBase
{

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id, CancellationToken cancellationToken)
        => Ok(await service.GetByIdAsync(id, cancellationToken));

    /// <summary>
    /// Gets all tasks.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await service.GetAllAsync(cancellationToken));

    /// <summary>
    /// Updates an existing task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id, UpdateTaskRequest request, CancellationToken cancellationToken)
        => Ok(await service.UpdateAsync(id, request, cancellationToken));

    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
