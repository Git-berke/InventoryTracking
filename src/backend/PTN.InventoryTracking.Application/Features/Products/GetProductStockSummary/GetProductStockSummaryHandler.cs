using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Products;

namespace PTN.InventoryTracking.Application.Features.Products.GetProductStockSummary;

public sealed class GetProductStockSummaryHandler(IProductQueries productQueries)
{
    public Task<ProductStockSummaryDto?> HandleAsync(
        GetProductStockSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        return productQueries.GetStockSummaryAsync(query.ProductId, cancellationToken);
    }
}
