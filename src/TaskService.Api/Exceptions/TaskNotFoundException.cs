namespace TaskService.Api.Exceptions;

/// <summary>
/// Exception thrown when a task is not found.
/// </summary>
/// <param name="id"></param>
public sealed class TaskNotFoundException(string id)
    : Exception($"Task '{id}' was not found.");
