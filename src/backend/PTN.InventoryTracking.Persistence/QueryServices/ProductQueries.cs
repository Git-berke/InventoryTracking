using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.Products;
using PTN.InventoryTracking.Domain.Enums;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.QueryServices;

public sealed class ProductQueries(InventoryTrackingDbContext dbContext) : IProductQueries
{
    public async Task<PagedResult<ProductListItemDto>> GetProductsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);

        var query = dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductListItemDto(
                x.Id,
                x.Code,
                x.Name,
                x.Unit,
                x.IsActive))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<ProductStockSummaryDto?> GetStockSummaryAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(x => x.Id == productId)
            .Select(x => new
            {
                x.Id,
                x.Code,
                x.Name,
                x.Unit
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return null;
        }

        var distribution = await dbContext.StockBalances
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.Quantity)
            .ThenBy(x => x.StockLocation.Name)
            .Select(x => new StockLocationQuantityDto(
                x.StockLocationId,
                x.StockLocation.Name,
                x.StockLocation.LocationType == StockLocationType.Warehouse ? "warehouse" : "vehicle",
                x.StockLocation.Warehouse != null ? x.StockLocation.Warehouse.Name : null,
                x.StockLocation.Vehicle != null ? x.StockLocation.Vehicle.LicensePlate : null,
                x.Quantity))
            .ToListAsync(cancellationToken);

        var totalQuantity = distribution.Sum(x => x.Quantity);
        var warehouseQuantity = distribution
            .Where(x => x.LocationType == "warehouse")
            .Sum(x => x.Quantity);
        var vehicleQuantity = distribution
            .Where(x => x.LocationType == "vehicle")
            .Sum(x => x.Quantity);

        return new ProductStockSummaryDto(
            product.Id,
            product.Code,
            product.Name,
            product.Unit,
            totalQuantity,
            warehouseQuantity,
            vehicleQuantity,
            distribution);
    }

    private static int NormalizePage(int page) => page < 1 ? 1 : page;
    private static int NormalizePageSize(int pageSize) => pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
}
