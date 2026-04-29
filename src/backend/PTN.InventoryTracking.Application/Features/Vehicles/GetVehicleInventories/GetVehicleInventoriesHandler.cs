using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Vehicles;

namespace PTN.InventoryTracking.Application.Features.Vehicles.GetVehicleInventories;

public sealed class GetVehicleInventoriesHandler(IVehicleQueries vehicleQueries)
{
    public Task<VehicleInventoryDetailDto?> HandleAsync(
        GetVehicleInventoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        return vehicleQueries.GetVehicleInventoriesAsync(query.VehicleId, cancellationToken);
    }
}
