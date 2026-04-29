namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record UpdateProductRequestDto(
    string Code,
    string Name,
    string? Description,
    string Unit,
    bool IsActive);
