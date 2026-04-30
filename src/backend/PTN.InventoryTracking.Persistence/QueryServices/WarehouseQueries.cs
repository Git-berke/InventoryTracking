using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Warehouses;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.QueryServices;

public sealed class WarehouseQueries(InventoryTrackingDbContext dbContext) : IWarehouseQueries
{
    public async Task<PagedResult<WarehouseListItemDto>> GetWarehousesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var query = dbContext.Warehouses
            .AsNoTracking()
            .OrderBy(x => x.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new WarehouseListItemDto(x.Id, x.Code, x.Name, x.Region, x.IsActive))
            .ToListAsync(cancellationToken);

        return new PagedResult<WarehouseListItemDto>(items, totalCount, page, pageSize);
    }
}
