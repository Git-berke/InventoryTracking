using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Products;

namespace PTN.InventoryTracking.Infrastructure.Caching;

public sealed class RedisProductStockSummaryCacheService(
    IDistributedCache distributedCache,
    IOptions<CacheOptions> cacheOptions) : IProductStockSummaryCacheService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly CacheOptions _cacheOptions = cacheOptions.Value;

    public async Task<ProductStockSummaryDto?> GetAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var payload = await distributedCache.GetStringAsync(GetKey(productId), cancellationToken);
        return string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonSerializer.Deserialize<ProductStockSummaryDto>(payload, SerializerOptions);
    }

    public Task SetAsync(ProductStockSummaryDto summary, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(summary, SerializerOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Math.Max(1, _cacheOptions.StockSummaryTtlMinutes))
        };

        return distributedCache.SetStringAsync(GetKey(summary.ProductId), payload, options, cancellationToken);
    }

    public Task RemoveAsync(Guid productId, CancellationToken cancellationToken = default) =>
        distributedCache.RemoveAsync(GetKey(productId), cancellationToken);

    private static string GetKey(Guid productId) => $"product-stock-summary:{productId}";
}
