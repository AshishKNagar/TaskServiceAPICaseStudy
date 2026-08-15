using Newtonsoft.Json;
using System.Text.Json.Serialization;
using TaskService.Api.Domain.Enums;

namespace TaskService.Api.Domain.Entities;

public sealed class TaskItem
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "description")]
    public string? Description { get; set; }

    [JsonProperty(PropertyName = "status")]
    public TaskItemStatus Status { get; set; }

    [JsonProperty(PropertyName = "originalEstimatedWork")]
    public decimal OriginalEstimatedWork { get; set; }

    [JsonProperty(PropertyName = "remainingWork")]
    public decimal RemainingWork { get; set; }

    [JsonProperty(PropertyName = "completedWork")]
    public decimal CompletedWork { get; set; }

    [JsonProperty(PropertyName = "createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty(PropertyName = "updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}
