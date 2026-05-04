namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record WarehouseInventoryDetailDto(
    Guid WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    string Region,
    IReadOnlyCollection<WarehouseInventoryItemDto> Inventories);
