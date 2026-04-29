namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record ProductDetailDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    string Unit,
    bool IsActive);
