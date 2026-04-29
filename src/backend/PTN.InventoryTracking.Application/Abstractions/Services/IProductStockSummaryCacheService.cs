using PTN.InventoryTracking.Application.DTOs.Products;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IProductStockSummaryCacheService
{
    Task<ProductStockSummaryDto?> GetAsync(Guid productId, CancellationToken cancellationToken = default);
    Task SetAsync(ProductStockSummaryDto summary, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid productId, CancellationToken cancellationToken = default);
}
