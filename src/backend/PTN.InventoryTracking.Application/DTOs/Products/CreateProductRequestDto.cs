namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record CreateProductRequestDto(
    string Code,
    string Name,
    string? Description,
    string Unit);
