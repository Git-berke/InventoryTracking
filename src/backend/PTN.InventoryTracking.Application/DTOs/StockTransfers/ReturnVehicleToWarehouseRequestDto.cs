namespace PTN.InventoryTracking.Application.DTOs.StockTransfers;

public sealed record ReturnVehicleToWarehouseRequestDto(
    Guid ProductId,
    Guid SourceVehicleId,
    Guid DestinationWarehouseId,
    int Quantity,
    string? ReferenceNote);
