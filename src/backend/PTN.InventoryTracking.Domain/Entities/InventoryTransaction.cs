using PTN.InventoryTracking.Domain.Common;
using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class InventoryTransaction : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid? SourceStockLocationId { get; set; }
    public Guid? DestinationStockLocationId { get; set; }
    public Guid? TaskId { get; set; }
    public InventoryTransactionType TransactionType { get; set; }
    public int Quantity { get; set; }
    public DateTime PerformedAtUtc { get; set; }
    public string? ReferenceNote { get; set; }

    public Product Product { get; set; } = null!;
    public StockLocation? SourceStockLocation { get; set; }
    public StockLocation? DestinationStockLocation { get; set; }
    public InventoryTask? Task { get; set; }
}
