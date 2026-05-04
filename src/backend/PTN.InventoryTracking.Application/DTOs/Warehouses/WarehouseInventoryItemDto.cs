namespace PTN.InventoryTracking.Application.DTOs.Warehouses;

public sealed record WarehouseInventoryItemDto(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string Unit,
    int Quantity);
