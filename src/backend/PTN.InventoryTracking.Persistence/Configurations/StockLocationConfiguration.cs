using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Persistence.Configurations;

public sealed class StockLocationConfiguration : IEntityTypeConfiguration<StockLocation>
{
    public void Configure(EntityTypeBuilder<StockLocation> builder)
    {
        builder.ToTable("stock_locations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.LocationType)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.Warehouse)
            .WithOne(x => x.StockLocation)
            .HasForeignKey<StockLocation>(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vehicle)
            .WithOne(x => x.StockLocation)
            .HasForeignKey<StockLocation>(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.WarehouseId).IsUnique();
        builder.HasIndex(x => x.VehicleId).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "ck_stock_locations_owner",
                $"""
                (
                    "location_type" = {(int)StockLocationType.Warehouse}
                    AND "warehouse_id" IS NOT NULL
                    AND "vehicle_id" IS NULL
                )
                OR
                (
                    "location_type" = {(int)StockLocationType.Vehicle}
                    AND "vehicle_id" IS NOT NULL
                    AND "warehouse_id" IS NULL
                )
                """);
        });
    }
}
