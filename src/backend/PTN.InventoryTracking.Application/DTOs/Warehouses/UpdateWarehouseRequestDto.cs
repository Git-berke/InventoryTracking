namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record UpdateWarehouseRequestDto(
    string Code,
    string Name,
    string Region,
    string? Address,
    bool IsActive);
