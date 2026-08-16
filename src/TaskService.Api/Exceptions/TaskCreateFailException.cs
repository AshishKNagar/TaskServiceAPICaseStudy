namespace TaskService.Api.Exceptions
{
    /// <summary>
    /// Exception thrown when a task creation fails.
    /// </summary>
    /// <param name="title"></param>
    public sealed class TaskCreateFailException(string title)
    : Exception($"Task '{title}' creation failed.");

}
