using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Configurations;

public sealed class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("inventory_transactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ReferenceNote)
            .HasMaxLength(500);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.InventoryTransactions)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SourceStockLocation)
            .WithMany(x => x.SourceTransactions)
            .HasForeignKey(x => x.SourceStockLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DestinationStockLocation)
            .WithMany(x => x.DestinationTransactions)
            .HasForeignKey(x => x.DestinationStockLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Task)
            .WithMany(x => x.InventoryTransactions)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.PerformedAtUtc);
        builder.HasIndex(x => new { x.ProductId, x.PerformedAtUtc });
        builder.HasIndex(x => x.TaskId);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_inventory_transactions_quantity_positive", "\"quantity\" > 0");
            t.HasCheckConstraint(
                "ck_inventory_transactions_endpoint_presence",
                "\"source_stock_location_id\" IS NOT NULL OR \"destination_stock_location_id\" IS NOT NULL");
        });
    }
}
