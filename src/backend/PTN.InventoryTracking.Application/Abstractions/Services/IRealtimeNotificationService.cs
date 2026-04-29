using PTN.InventoryTracking.Application.DTOs.Realtime;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IRealtimeNotificationService
{
    Task PublishInventoryEventAsync(
        InventoryRealtimeEventDto notification,
        CancellationToken cancellationToken = default);
}
