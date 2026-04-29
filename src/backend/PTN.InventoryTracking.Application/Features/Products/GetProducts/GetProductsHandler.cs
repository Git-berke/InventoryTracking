using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Products;

namespace PTN.InventoryTracking.Application.Features.Products.GetProducts;

public sealed class GetProductsHandler(IProductQueries productQueries)
{
    public Task<PagedResult<ProductListItemDto>> HandleAsync(
        GetProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        return productQueries.GetProductsAsync(query.Page, query.PageSize, cancellationToken);
    }
}
