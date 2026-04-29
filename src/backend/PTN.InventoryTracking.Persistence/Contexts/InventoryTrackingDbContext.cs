using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Contexts;

public sealed class InventoryTrackingDbContext(DbContextOptions<InventoryTrackingDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<InventoryTask> Tasks => Set<InventoryTask>();
    public DbSet<VehicleTask> VehicleTasks => Set<VehicleTask>();
    public DbSet<StockLocation> StockLocations => Set<StockLocation>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryTrackingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
