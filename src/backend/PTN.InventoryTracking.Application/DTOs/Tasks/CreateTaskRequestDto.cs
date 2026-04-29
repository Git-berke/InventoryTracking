using System.ComponentModel.DataAnnotations;
using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Application.DTOs.Tasks;

public sealed record CreateTaskRequestDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Required, StringLength(100, MinimumLength = 2)]
    public string Region { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public InventoryTaskStatus Status { get; init; }
}
