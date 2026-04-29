using System.ComponentModel.DataAnnotations;

namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record CreateWarehouseRequestDto
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Code { get; init; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string Region { get; init; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; init; }
}
