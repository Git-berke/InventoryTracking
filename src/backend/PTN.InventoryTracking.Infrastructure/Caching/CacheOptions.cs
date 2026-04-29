namespace PTN.InventoryTracking.Infrastructure.Caching;

public sealed class CacheOptions
{
    public const string SectionName = "Cache";

    public bool UseRedis { get; set; }
    public string? RedisConnectionString { get; set; }
    public int StockSummaryTtlMinutes { get; set; } = 10;
}
