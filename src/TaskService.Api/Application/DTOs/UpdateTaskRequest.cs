using System.ComponentModel.DataAnnotations;
using TaskService.Api.Domain.Enums;

namespace TaskService.Api.Application.DTOs;

public sealed class UpdateTaskRequest
{
    [Required, MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    public TaskItemStatus Status { get; init; }

    [Range(0, double.MaxValue)]
    public decimal OriginalEstimatedWork { get; init; }

    [Range(0, double.MaxValue)]
    public decimal RemainingWork { get; init; }

    [Range(0, double.MaxValue)]
    public decimal CompletedWork { get; init; }
}
