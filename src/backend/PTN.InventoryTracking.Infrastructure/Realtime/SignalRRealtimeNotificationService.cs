using Microsoft.AspNetCore.SignalR;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Realtime;

namespace PTN.InventoryTracking.Infrastructure.Realtime;

public sealed class SignalRRealtimeNotificationService(
    IHubContext<InventoryEventsHub> hubContext) : IRealtimeNotificationService
{
    public Task PublishInventoryEventAsync(
        InventoryRealtimeEventDto notification,
        CancellationToken cancellationToken = default) =>
        hubContext.Clients.All.SendAsync(
            InventoryEventsHub.ReceiveMethod,
            notification,
            cancellationToken);
}
