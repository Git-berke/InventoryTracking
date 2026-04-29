namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record StockLocationQuantityDto(
    Guid StockLocationId,
    string StockLocationName,
    string LocationType,
    string? WarehouseName,
    string? VehicleLicensePlate,
    int Quantity);
