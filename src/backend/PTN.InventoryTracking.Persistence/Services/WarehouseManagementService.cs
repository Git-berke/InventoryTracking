using PTN.InventoryTracking.Application.Abstractions.Persistence;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Warehouses;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Services;

public sealed class WarehouseManagementService(
    IWarehouseRepository warehouseRepository,
    IInventoryTrackingDbContext dbContext) : IWarehouseManagementService
{
    public async Task<WarehouseDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await warehouseRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<WarehouseDetailDto> CreateAsync(CreateWarehouseRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedCode = NormalizeRequired(request.Code, nameof(request.Code));
        if (await warehouseRepository.ExistsByCodeAsync(normalizedCode, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A warehouse with the same code already exists.");
        }

        var entity = new Warehouse
        {
            Code = normalizedCode,
            Name = NormalizeRequired(request.Name, nameof(request.Name)),
            Region = NormalizeRequired(request.Region, nameof(request.Region)),
            Address = NormalizeOptional(request.Address),
            IsActive = true
        };

        await warehouseRepository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<WarehouseDetailDto?> UpdateAsync(Guid id, UpdateWarehouseRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await warehouseRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var normalizedCode = NormalizeRequired(request.Code, nameof(request.Code));
        if (await warehouseRepository.ExistsByCodeAsync(normalizedCode, id, cancellationToken))
        {
            throw new InvalidOperationException("A warehouse with the same code already exists.");
        }

        entity.Code = normalizedCode;
        entity.Name = NormalizeRequired(request.Name, nameof(request.Name));
        entity.Region = NormalizeRequired(request.Region, nameof(request.Region));
        entity.Address = NormalizeOptional(request.Address);
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await warehouseRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static WarehouseDetailDto Map(Warehouse entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Region, entity.Address, entity.IsActive);

    private static string NormalizeRequired(string value, string paramName)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized)
            ? throw new ArgumentException("Value is required.", paramName)
            : normalized;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
