using PTN.InventoryTracking.Domain.Common;
using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class InventoryTask : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Region { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public InventoryTaskStatus Status { get; set; } = InventoryTaskStatus.Draft;

    public ICollection<VehicleTask> VehicleTasks { get; set; } = new List<VehicleTask>();
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
