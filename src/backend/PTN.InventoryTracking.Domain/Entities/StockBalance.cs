using PTN.InventoryTracking.Domain.Common;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class StockBalance : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid StockLocationId { get; set; }
    public int Quantity { get; set; }

    public Product Product { get; set; } = null!;
    public StockLocation StockLocation { get; set; } = null!;
}
