using System.ComponentModel.DataAnnotations;

namespace PTN.InventoryTracking.Application.DTOs.StockTransfers;

public sealed record TransferWarehouseToVehicleRequestDto
{
    public Guid ProductId { get; init; }
    public Guid SourceWarehouseId { get; init; }
    public Guid DestinationVehicleId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    public Guid TaskId { get; init; }

    [StringLength(500)]
    public string? ReferenceNote { get; init; }
}
