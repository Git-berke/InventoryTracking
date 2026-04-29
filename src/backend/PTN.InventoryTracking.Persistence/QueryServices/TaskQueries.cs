using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Tasks;
using PTN.InventoryTracking.Domain.Enums;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.QueryServices;

public sealed class TaskQueries(InventoryTrackingDbContext dbContext) : ITaskQueries
{
    public async Task<PagedResult<TaskListItemDto>> GetTasksAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);

        var query = dbContext.Tasks
            .AsNoTracking()
            .OrderByDescending(x => x.StartDate)
            .ThenBy(x => x.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TaskListItemDto(
                x.Id,
                x.Name,
                x.Region,
                MapTaskStatus(x.Status),
                x.StartDate,
                x.EndDate))
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<IReadOnlyCollection<TaskVehicleDto>> GetTaskVehiclesAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.VehicleTasks
            .AsNoTracking()
            .Where(x => x.TaskId == taskId)
            .OrderByDescending(x => x.AssignedAtUtc)
            .Select(x => new TaskVehicleDto(
                x.VehicleId,
                x.Vehicle.Code,
                x.Vehicle.LicensePlate,
                x.Vehicle.VehicleType,
                x.AssignedAtUtc,
                x.ReleasedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaskInventoryItemDto>> GetTaskInventoryAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var vehicleIds = await dbContext.VehicleTasks
            .AsNoTracking()
            .Where(x => x.TaskId == taskId && x.ReleasedAtUtc == null)
            .Select(x => x.VehicleId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (vehicleIds.Count == 0)
        {
            return Array.Empty<TaskInventoryItemDto>();
        }

        return await dbContext.StockBalances
            .AsNoTracking()
            .Where(x => x.StockLocation.VehicleId != null && vehicleIds.Contains(x.StockLocation.VehicleId.Value))
            .OrderBy(x => x.StockLocation.Vehicle!.LicensePlate)
            .ThenBy(x => x.Product.Name)
            .Select(x => new TaskInventoryItemDto(
                x.StockLocation.Vehicle!.Id,
                x.StockLocation.Vehicle.LicensePlate,
                x.ProductId,
                x.Product.Code,
                x.Product.Name,
                x.Product.Unit,
                x.Quantity))
            .ToListAsync(cancellationToken);
    }

    private static int NormalizePage(int page) => page < 1 ? 1 : page;
    private static int NormalizePageSize(int pageSize) => pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

    private static string MapTaskStatus(InventoryTaskStatus status) => status switch
    {
        InventoryTaskStatus.Draft => "draft",
        InventoryTaskStatus.InProgress => "in_progress",
        InventoryTaskStatus.Completed => "completed",
        _ => "unknown"
    };
}
