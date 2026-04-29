using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    void Remove(Warehouse warehouse);
}
