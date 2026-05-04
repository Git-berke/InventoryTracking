using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Warehouses;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IWarehouseQueries
{
    Task<PagedResult<WarehouseListItemDto>> GetWarehousesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<WarehouseInventoryDetailDto?> GetWarehouseInventoriesAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default);
}
