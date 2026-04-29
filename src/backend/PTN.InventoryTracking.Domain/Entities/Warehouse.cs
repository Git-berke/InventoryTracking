using PTN.InventoryTracking.Domain.Common;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class Warehouse : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public StockLocation? StockLocation { get; set; }
}
