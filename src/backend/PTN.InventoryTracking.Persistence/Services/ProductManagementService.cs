using PTN.InventoryTracking.Application.Abstractions.Persistence;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Products;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Services;

public sealed class ProductManagementService(
    IProductRepository productRepository,
    IInventoryTrackingDbContext dbContext) : IProductManagementService
{
    public async Task<ProductDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await productRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<ProductDetailDto> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedCode = NormalizeRequired(request.Code, nameof(request.Code));
        var normalizedName = NormalizeRequired(request.Name, nameof(request.Name));
        var normalizedUnit = NormalizeRequired(request.Unit, nameof(request.Unit));

        if (await productRepository.ExistsByCodeAsync(normalizedCode, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A product with the same code already exists.");
        }

        var entity = new Product
        {
            Code = normalizedCode,
            Name = normalizedName,
            Description = NormalizeOptional(request.Description),
            Unit = normalizedUnit,
            IsActive = true
        };

        await productRepository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Map(entity);
    }

    public async Task<ProductDetailDto?> UpdateAsync(Guid id, UpdateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await productRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var normalizedCode = NormalizeRequired(request.Code, nameof(request.Code));
        if (await productRepository.ExistsByCodeAsync(normalizedCode, id, cancellationToken))
        {
            throw new InvalidOperationException("A product with the same code already exists.");
        }

        entity.Code = normalizedCode;
        entity.Name = NormalizeRequired(request.Name, nameof(request.Name));
        entity.Description = NormalizeOptional(request.Description);
        entity.Unit = NormalizeRequired(request.Unit, nameof(request.Unit));
        entity.IsActive = request.IsActive;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await productRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsActive = false;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductDetailDto Map(Product entity) =>
        new(entity.Id, entity.Code, entity.Name, entity.Description, entity.Unit, entity.IsActive);

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
