using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Warehouses;

namespace PTN.InventoryTracking.Application.Features.Warehouses.GetWarehouseInventories;

public sealed class GetWarehouseInventoriesHandler(IWarehouseQueries warehouseQueries)
{
    public Task<WarehouseInventoryDetailDto?> HandleAsync(
        GetWarehouseInventoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        return warehouseQueries.GetWarehouseInventoriesAsync(
            query.WarehouseId,
            cancellationToken);
    }
}
