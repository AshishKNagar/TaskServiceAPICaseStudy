using TaskService.Api.Domain.Enums;

namespace TaskService.Api.Application.DTOs;

public sealed record TaskResponse(
    string Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    decimal OriginalEstimatedWork,
    decimal RemainingWork,
    decimal CompletedWork,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
