using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Tasks;

namespace PTN.InventoryTracking.Application.Features.Tasks.GetTaskInventory;

public sealed class GetTaskInventoryHandler(ITaskQueries taskQueries)
{
    public Task<IReadOnlyCollection<TaskInventoryItemDto>> HandleAsync(
        GetTaskInventoryQuery query,
        CancellationToken cancellationToken = default)
    {
        return taskQueries.GetTaskInventoryAsync(query.TaskId, cancellationToken);
    }
}
