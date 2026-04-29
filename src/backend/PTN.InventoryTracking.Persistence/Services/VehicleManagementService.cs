using PTN.InventoryTracking.Application.Abstractions.Persistence;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Vehicles;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Services;

public sealed class VehicleManagementService(
    IVehicleRepository vehicleRepository,
    IInventoryTrackingDbContext dbContext) : IVehicleManagementService
{
    public async Task<VehicleDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await vehicleRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<VehicleDetailDto> CreateAsync(CreateVehicleRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedCode = NormalizeRequired(request.Code, nameof(request.Code));
        var normalizedPlate = NormalizeRequired(request.LicensePlate, nameof(request.LicensePlate));

        if (await vehicleRepository.ExistsByCodeAsync(normalizedCode, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A vehicle with the same code already exists.");
        }

        if (await vehicleRepository.ExistsByLicensePlateAsync(normalizedPlate, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A vehicle with the same license plate already exists.");
        }

        var entity = new Vehicle
        {
            Code = normalizedCode,
            LicensePlate = normalizedPlate,
            VehicleType = NormalizeRequired(request.VehicleType, nameof(request.VehicleType)),
            IsActive = true
        };

        await vehicleRepository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<VehicleDetailDto?> UpdateAsync(Guid id, UpdateVehicleRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await vehicleRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var normalizedCode = NormalizeRequired(request.Code, nameof(request.Code));
        var normalizedPlate = NormalizeRequired(request.LicensePlate, nameof(request.LicensePlate));

        if (await vehicleRepository.ExistsByCodeAsync(normalizedCode, id, cancellationToken))
        {
            throw new InvalidOperationException("A vehicle with the same code already exists.");
        }

        if (await vehicleRepository.ExistsByLicensePlateAsync(normalizedPlate, id, cancellationToken))
        {
            throw new InvalidOperationException("A vehicle with the same license plate already exists.");
        }

        entity.Code = normalizedCode;
        entity.LicensePlate = normalizedPlate;
        entity.VehicleType = NormalizeRequired(request.VehicleType, nameof(request.VehicleType));
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await vehicleRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static VehicleDetailDto Map(Vehicle entity) =>
        new(entity.Id, entity.Code, entity.LicensePlate, entity.VehicleType, entity.IsActive);

    private static string NormalizeRequired(string value, string paramName)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized)
            ? throw new ArgumentException("Value is required.", paramName)
            : normalized;
    }
}
