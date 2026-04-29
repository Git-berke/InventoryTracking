using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Configurations;

public sealed class TaskConfiguration : IEntityTypeConfiguration<InventoryTask>
{
    public void Configure(EntityTypeBuilder<InventoryTask> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Region)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => new { x.Status, x.StartDate });
        builder.ToTable(t => t.HasCheckConstraint("ck_tasks_date_range", "\"end_date\" IS NULL OR \"end_date\" >= \"start_date\""));
    }
}
