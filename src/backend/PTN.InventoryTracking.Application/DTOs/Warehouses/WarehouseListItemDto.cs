namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record WarehouseListItemDto(
    Guid Id,
    string Code,
    string Name,
    string Region,
    bool IsActive);
