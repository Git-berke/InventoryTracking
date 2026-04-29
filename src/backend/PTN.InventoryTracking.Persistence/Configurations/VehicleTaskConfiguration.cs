using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Configurations;

public sealed class VehicleTaskConfiguration : IEntityTypeConfiguration<VehicleTask>
{
    public void Configure(EntityTypeBuilder<VehicleTask> builder)
    {
        builder.ToTable("vehicle_tasks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AssignmentNote)
            .HasMaxLength(300);

        builder.HasOne(x => x.Vehicle)
            .WithMany(x => x.VehicleTasks)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Task)
            .WithMany(x => x.VehicleTasks)
            .HasForeignKey(x => x.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.VehicleId, x.AssignedAtUtc });
        builder.HasIndex(x => new { x.TaskId, x.AssignedAtUtc });

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_vehicle_tasks_release_after_assign", "\"released_at_utc\" IS NULL OR \"released_at_utc\" >= \"assigned_at_utc\"");
        });
    }
}
