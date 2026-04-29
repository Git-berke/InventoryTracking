using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;

public interface ITaskRepository
{
    Task<InventoryTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryTask task, CancellationToken cancellationToken = default);
    void Remove(InventoryTask task);
    Task<bool> HasRelationsAsync(Guid id, CancellationToken cancellationToken = default);
}
