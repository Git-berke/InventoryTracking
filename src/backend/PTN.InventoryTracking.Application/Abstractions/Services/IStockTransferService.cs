namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IStockTransferService
{
    Task TransferWarehouseToVehicleAsync(
        Guid productId,
        Guid sourceWarehouseId,
        Guid destinationVehicleId,
        int quantity,
        Guid taskId,
        string? referenceNote,
        CancellationToken cancellationToken = default);

    Task ReturnVehicleToWarehouseAsync(
        Guid productId,
        Guid sourceVehicleId,
        Guid destinationWarehouseId,
        int quantity,
        string? referenceNote,
        CancellationToken cancellationToken = default);
}
