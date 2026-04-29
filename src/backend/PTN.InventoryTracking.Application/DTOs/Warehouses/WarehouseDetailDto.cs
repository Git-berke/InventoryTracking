namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record WarehouseDetailDto(
    Guid Id,
    string Code,
    string Name,
    string Region,
    string? Address,
    bool IsActive);
