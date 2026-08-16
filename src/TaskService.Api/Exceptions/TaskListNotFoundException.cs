namespace TaskService.Api.Exceptions
{
    /// <summary>
    /// Exception thrown when a task list is not found.
    /// </summary>
    public sealed class TaskListNotFoundException()
    : Exception($"Task list not found.");

}
