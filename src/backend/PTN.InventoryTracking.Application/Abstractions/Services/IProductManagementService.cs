using PTN.InventoryTracking.Application.DTOs.Products;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IProductManagementService
{
    Task<ProductDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDetailDto> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> UpdateAsync(Guid id, UpdateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
