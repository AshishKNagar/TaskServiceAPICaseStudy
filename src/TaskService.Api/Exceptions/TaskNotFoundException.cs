namespace TaskService.Api.Exceptions;

public sealed class TaskNotFoundException(string id)
    : Exception($"Task '{id}' was not found.");
