using PTN.InventoryTracking.Domain.Common;

namespace PTN.InventoryTracking.Domain.Entities;

public sealed class AppRolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    public AppRole Role { get; set; } = null!;
    public AppPermission Permission { get; set; } = null!;
}
