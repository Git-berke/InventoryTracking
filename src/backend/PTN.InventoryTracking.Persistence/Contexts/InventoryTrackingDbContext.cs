using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Persistence;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Persistence.Seed;

namespace PTN.InventoryTracking.Persistence.Contexts;

public sealed class InventoryTrackingDbContext(DbContextOptions<InventoryTrackingDbContext> options) : DbContext(options), IInventoryTrackingDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<AppRole> AppRoles => Set<AppRole>();
    public DbSet<AppPermission> AppPermissions => Set<AppPermission>();
    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();
    public DbSet<AppRolePermission> AppRolePermissions => Set<AppRolePermission>();
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
        SeedData.Apply(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
