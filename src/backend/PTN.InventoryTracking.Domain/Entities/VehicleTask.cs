using PTN.InventoryTracking.Domain.Common;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class VehicleTask : BaseEntity
{
    public Guid VehicleId { get; set; }
    public Guid TaskId { get; set; }
    public DateTime AssignedAtUtc { get; set; }
    public DateTime? ReleasedAtUtc { get; set; }
    public string? AssignmentNote { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
    public InventoryTask Task { get; set; } = null!;
}
