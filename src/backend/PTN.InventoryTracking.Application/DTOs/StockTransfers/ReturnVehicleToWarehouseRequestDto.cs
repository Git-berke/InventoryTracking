using System.ComponentModel.DataAnnotations;

namespace PTN.InventoryTracking.Application.DTOs.StockTransfers;

public sealed record ReturnVehicleToWarehouseRequestDto
{
    public Guid ProductId { get; init; }
    public Guid SourceVehicleId { get; init; }
    public Guid DestinationWarehouseId { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [StringLength(500)]
    public string? ReferenceNote { get; init; }
}
