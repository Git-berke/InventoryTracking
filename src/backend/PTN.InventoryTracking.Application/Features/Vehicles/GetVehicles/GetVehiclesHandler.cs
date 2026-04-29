using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Vehicles;

namespace PTN.InventoryTracking.Application.Features.Vehicles.GetVehicles;

public sealed class GetVehiclesHandler(IVehicleQueries vehicleQueries)
{
    public Task<PagedResult<VehicleListItemDto>> HandleAsync(
        GetVehiclesQuery query,
        CancellationToken cancellationToken = default)
    {
        return vehicleQueries.GetVehiclesAsync(query.Page, query.PageSize, cancellationToken);
    }
}
