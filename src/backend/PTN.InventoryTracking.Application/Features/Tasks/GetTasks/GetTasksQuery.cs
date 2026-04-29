namespace PTN.InventoryTracking.Application.Features.Tasks.GetTasks;

public sealed record GetTasksQuery(int Page = 1, int PageSize = 20);
