namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record CreateWarehouseRequestDto(
    string Code,
    string Name,
    string Region,
    string? Address);
