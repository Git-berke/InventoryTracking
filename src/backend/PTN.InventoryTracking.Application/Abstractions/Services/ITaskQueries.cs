using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Tasks;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface ITaskQueries
{
    Task<PagedResult<TaskListItemDto>> GetTasksAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaskVehicleDto>> GetTaskVehiclesAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaskInventoryItemDto>> GetTaskInventoryAsync(Guid taskId, CancellationToken cancellationToken = default);
}
