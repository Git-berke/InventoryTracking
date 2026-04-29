using PTN.InventoryTracking.Domain.Common;
using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class StockLocation : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public StockLocationType LocationType { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? VehicleId { get; set; }
    public bool IsActive { get; set; } = true;

    public Warehouse? Warehouse { get; set; }
    public Vehicle? Vehicle { get; set; }
    public ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
    public ICollection<InventoryTransaction> SourceTransactions { get; set; } = new List<InventoryTransaction>();
    public ICollection<InventoryTransaction> DestinationTransactions { get; set; } = new List<InventoryTransaction>();
}
