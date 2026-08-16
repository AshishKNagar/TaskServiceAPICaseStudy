using System.ComponentModel.DataAnnotations;
using TaskService.Api.Domain.Enums;

namespace TaskService.Api.Application.DTOs;

/// <summary>
///     Represents a request to update an existing task item.
/// </summary>
public sealed class UpdateTaskRequest : IValidatableObject
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

    /// <summary>
    ///  validation for status  value by enum code 
    /// </summary>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (!Enum.IsDefined(typeof(TaskItemStatus), Status))
        {
            yield return new ValidationResult(
                $"Invalid status value: {(int)Status}. " +
                "Valid values are Todo, InProgress and Done.",
                new[] { nameof(Status) });
        }
    }
}
