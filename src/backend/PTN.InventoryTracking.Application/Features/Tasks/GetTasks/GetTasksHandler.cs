using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Tasks;

namespace PTN.InventoryTracking.Application.Features.Tasks.GetTasks;

public sealed class GetTasksHandler(ITaskQueries taskQueries)
{
    public Task<PagedResult<TaskListItemDto>> HandleAsync(
        GetTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        return taskQueries.GetTasksAsync(query.Page, query.PageSize, cancellationToken);
    }
}
