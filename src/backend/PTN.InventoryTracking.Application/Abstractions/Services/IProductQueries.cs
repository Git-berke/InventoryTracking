using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Products;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IProductQueries
{
    Task<PagedResult<ProductListItemDto>> GetProductsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<ProductStockSummaryDto?> GetStockSummaryAsync(Guid productId, CancellationToken cancellationToken = default);
}
