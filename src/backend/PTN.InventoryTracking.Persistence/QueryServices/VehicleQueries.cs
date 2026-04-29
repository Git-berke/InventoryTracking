using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Vehicles;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.QueryServices;

public sealed class VehicleQueries(InventoryTrackingDbContext dbContext) : IVehicleQueries
{
    public async Task<PagedResult<VehicleListItemDto>> GetVehiclesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);

        var baseQuery = dbContext.Vehicles
            .AsNoTracking()
            .OrderBy(x => x.LicensePlate);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(vehicle => new VehicleListItemDto(
                vehicle.Id,
                vehicle.Code,
                vehicle.LicensePlate,
                vehicle.VehicleType,
                vehicle.IsActive,
                vehicle.VehicleTasks
                    .Where(vt => vt.ReleasedAtUtc == null)
                    .OrderByDescending(vt => vt.AssignedAtUtc)
                    .Select(vt => vt.Task.Name)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<VehicleListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<VehicleInventoryDetailDto?> GetVehicleInventoriesAsync(
        Guid vehicleId,
        CancellationToken cancellationToken = default)
    {
        var vehicle = await dbContext.Vehicles
            .AsNoTracking()
            .Where(x => x.Id == vehicleId)
            .Select(vehicle => new
            {
                vehicle.Id,
                vehicle.Code,
                vehicle.LicensePlate,
                vehicle.VehicleType,
                ActiveTaskName = vehicle.VehicleTasks
                    .Where(vt => vt.ReleasedAtUtc == null)
                    .OrderByDescending(vt => vt.AssignedAtUtc)
                    .Select(vt => vt.Task.Name)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (vehicle is null)
        {
            return null;
        }

        var inventories = await dbContext.StockBalances
            .AsNoTracking()
            .Where(x => x.StockLocation.VehicleId == vehicleId)
            .OrderBy(x => x.Product.Name)
            .Select(x => new VehicleInventoryItemDto(
                x.ProductId,
                x.Product.Code,
                x.Product.Name,
                x.Product.Unit,
                x.Quantity))
            .ToListAsync(cancellationToken);

        return new VehicleInventoryDetailDto(
            vehicle.Id,
            vehicle.Code,
            vehicle.LicensePlate,
            vehicle.VehicleType,
            vehicle.ActiveTaskName,
            inventories);
    }

    private static int NormalizePage(int page) => page < 1 ? 1 : page;
    private static int NormalizePageSize(int pageSize) => pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
}
