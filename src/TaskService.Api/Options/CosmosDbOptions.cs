namespace TaskService.Api.Options;

/// <summary>
///     Represents the configuration options for connecting to a Cosmos DB instance.
/// </summary>
public sealed class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";
    public string Endpoint { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = "TaskDb";
    public string ContainerName { get; init; } = "Tasks";
}
