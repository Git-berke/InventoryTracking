using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Vehicles;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IVehicleQueries
{
    Task<PagedResult<VehicleListItemDto>> GetVehiclesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<VehicleInventoryDetailDto?> GetVehicleInventoriesAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}
