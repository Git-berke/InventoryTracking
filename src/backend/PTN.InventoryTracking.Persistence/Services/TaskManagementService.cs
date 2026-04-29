using PTN.InventoryTracking.Application.Abstractions.Persistence;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Realtime;
using PTN.InventoryTracking.Application.DTOs.Tasks;
using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Persistence.Services;

public sealed class TaskManagementService(
    ITaskRepository taskRepository,
    IInventoryTrackingDbContext dbContext,
    IRealtimeNotificationService realtimeNotificationService) : ITaskManagementService
{
    public async Task<TaskDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await taskRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<TaskDetailDto> CreateAsync(CreateTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        ValidateDates(request.StartDate, request.EndDate);

        var entity = new InventoryTask
        {
            Name = NormalizeRequired(request.Name, nameof(request.Name)),
            Description = NormalizeOptional(request.Description),
            Region = NormalizeRequired(request.Region, nameof(request.Region)),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = request.Status
        };

        await taskRepository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await realtimeNotificationService.PublishInventoryEventAsync(
            BuildTaskNotification(
                "task.created",
                "Task created successfully.",
                entity),
            cancellationToken);

        return Map(entity);
    }

    public async Task<TaskDetailDto?> UpdateAsync(Guid id, UpdateTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        var entity = await taskRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        ValidateDates(request.StartDate, request.EndDate);

        entity.Name = NormalizeRequired(request.Name, nameof(request.Name));
        entity.Description = NormalizeOptional(request.Description);
        entity.Region = NormalizeRequired(request.Region, nameof(request.Region));
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.Status = request.Status;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        await realtimeNotificationService.PublishInventoryEventAsync(
            BuildTaskNotification(
                "task.updated",
                "Task updated successfully.",
                entity),
            cancellationToken);

        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await taskRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        if (await taskRepository.HasRelationsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException("Task cannot be deleted because it has related vehicle assignments or inventory transactions.");
        }

        taskRepository.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TaskDetailDto Map(InventoryTask entity) =>
        new(entity.Id, entity.Name, entity.Description, entity.Region, entity.StartDate, entity.EndDate, entity.Status);

    private static InventoryRealtimeEventDto BuildTaskNotification(
        string eventType,
        string message,
        InventoryTask task) =>
        new(
            eventType,
            message,
            DateTime.UtcNow,
            new Dictionary<string, string?>
            {
                ["taskId"] = task.Id.ToString(),
                ["taskName"] = task.Name,
                ["region"] = task.Region,
                ["status"] = task.Status.ToString(),
                ["startDate"] = task.StartDate.ToString("yyyy-MM-dd"),
                ["endDate"] = task.EndDate?.ToString("yyyy-MM-dd")
            });

    private static void ValidateDates(DateOnly startDate, DateOnly? endDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new InvalidOperationException("End date cannot be earlier than start date.");
        }
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized)
            ? throw new ArgumentException("Value is required.", paramName)
            : normalized;
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
