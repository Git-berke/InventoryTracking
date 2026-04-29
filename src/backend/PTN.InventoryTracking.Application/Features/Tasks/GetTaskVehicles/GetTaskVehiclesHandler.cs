using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Tasks;

namespace PTN.InventoryTracking.Application.Features.Tasks.GetTaskVehicles;

public sealed class GetTaskVehiclesHandler(ITaskQueries taskQueries)
{
    public Task<IReadOnlyCollection<TaskVehicleDto>> HandleAsync(
        GetTaskVehiclesQuery query,
        CancellationToken cancellationToken = default)
    {
        return taskQueries.GetTaskVehiclesAsync(query.TaskId, cancellationToken);
    }
}
