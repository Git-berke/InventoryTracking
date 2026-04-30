using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Warehouses;

namespace PTN.InventoryTracking.Application.Features.Warehouses.GetWarehouses;

public sealed class GetWarehousesHandler(IWarehouseQueries warehouseQueries)
{
    public Task<PagedResult<WarehouseListItemDto>> HandleAsync(
        GetWarehousesQuery query,
        CancellationToken cancellationToken = default)
    {
        return warehouseQueries.GetWarehousesAsync(query.Page, query.PageSize, cancellationToken);
    }
}
