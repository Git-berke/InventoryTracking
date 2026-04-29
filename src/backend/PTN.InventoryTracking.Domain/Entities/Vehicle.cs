using PTN.InventoryTracking.Domain.Common;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class Vehicle : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public StockLocation? StockLocation { get; set; }
    public ICollection<VehicleTask> VehicleTasks { get; set; } = new List<VehicleTask>();
}
