using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PTN.InventoryTracking.Persistence.Contexts;

public sealed class InventoryTrackingDbContextFactory : IDesignTimeDbContextFactory<InventoryTrackingDbContext>
{
    public InventoryTrackingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryTrackingDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("PTN_INVENTORY_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=postgres";

        optionsBuilder
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();

        return new InventoryTrackingDbContext(optionsBuilder.Options);
    }
}
