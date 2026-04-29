using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByLicensePlateAsync(string licensePlate, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    void Remove(Vehicle vehicle);
}
