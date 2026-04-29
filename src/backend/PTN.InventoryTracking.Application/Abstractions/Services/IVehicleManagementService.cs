using PTN.InventoryTracking.Application.DTOs.Vehicles;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IVehicleManagementService
{
    Task<VehicleDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VehicleDetailDto> CreateAsync(CreateVehicleRequestDto request, CancellationToken cancellationToken = default);
    Task<VehicleDetailDto?> UpdateAsync(Guid id, UpdateVehicleRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
