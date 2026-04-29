namespace PTN.InventoryTracking.Application.DTOs.Products;

public sealed record ProductStockSummaryDto(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string Unit,
    int TotalQuantity,
    int WarehouseQuantity,
    int VehicleQuantity,
    IReadOnlyCollection<StockLocationQuantityDto> StockDistribution);
