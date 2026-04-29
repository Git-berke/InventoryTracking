using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.Repositories;

public sealed class VehicleRepository(InventoryTrackingDbContext dbContext) : IVehicleRepository
{
    public Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Vehicles.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Vehicles.AnyAsync(x => x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public Task<bool> ExistsByLicensePlateAsync(string licensePlate, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Vehicles.AnyAsync(x => x.LicensePlate == licensePlate && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default) =>
        dbContext.Vehicles.AddAsync(vehicle, cancellationToken).AsTask();

    public void Remove(Vehicle vehicle) => dbContext.Vehicles.Remove(vehicle);
}
