namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record ProductListItemDto(
    Guid Id,
    string Code,
    string Name,
    string Unit,
    bool IsActive);
