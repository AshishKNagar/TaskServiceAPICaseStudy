using TaskService.Api.Domain.Enums;

namespace TaskService.Api.Application.DTOs;

/// <summary>
/// Represents a response containing task item details.
/// </summary>
/// <param name="Id"></param>
/// <param name="Title"></param>
/// <param name="Description"></param>
/// <param name="Status"></param>
/// <param name="OriginalEstimatedWork"></param>
/// <param name="RemainingWork"></param>
/// <param name="CompletedWork"></param>
/// <param name="CreatedAt"></param>
/// <param name="UpdatedAt"></param>
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
