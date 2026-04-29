using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.Repositories;

public sealed class WarehouseRepository(InventoryTrackingDbContext dbContext) : IWarehouseRepository
{
    public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Warehouses.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Warehouses.AnyAsync(x => x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) =>
        dbContext.Warehouses.AddAsync(warehouse, cancellationToken).AsTask();

    public void Remove(Warehouse warehouse) => dbContext.Warehouses.Remove(warehouse);
}
