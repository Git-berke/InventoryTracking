namespace PTN.InventoryTracking.Application.Abstractions.Persistence;

public interface IInventoryTrackingDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
