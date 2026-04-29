using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.Repositories;

public sealed class TaskRepository(InventoryTrackingDbContext dbContext) : ITaskRepository
{
    public Task<InventoryTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Tasks.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddAsync(InventoryTask task, CancellationToken cancellationToken = default) =>
        dbContext.Tasks.AddAsync(task, cancellationToken).AsTask();

    public void Remove(InventoryTask task) => dbContext.Tasks.Remove(task);

    public Task<bool> HasRelationsAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.VehicleTasks.AnyAsync(x => x.TaskId == id, cancellationToken)
        .ContinueWith(
            async vehicleTaskResult => vehicleTaskResult.Result ||
                await dbContext.InventoryTransactions.AnyAsync(x => x.TaskId == id, cancellationToken),
            cancellationToken)
        .Unwrap();
}
