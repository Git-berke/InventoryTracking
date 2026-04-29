using System.ComponentModel.DataAnnotations;

namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record CreateProductRequestDto
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Code { get; init; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    [Required, StringLength(30, MinimumLength = 1)]
    public string Unit { get; init; } = string.Empty;
}
