namespace PTN.InventoryTracking.Application.DTOs.StockTransfers;

public sealed record TransferWarehouseToVehicleRequestDto(
    Guid ProductId,
    Guid SourceWarehouseId,
    Guid DestinationVehicleId,
    int Quantity,
    Guid TaskId,
    string? ReferenceNote);
