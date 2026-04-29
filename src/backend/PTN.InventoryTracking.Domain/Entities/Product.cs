using PTN.InventoryTracking.Domain.Common;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class Product : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
