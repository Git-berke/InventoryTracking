using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PTN.InventoryTracking.Infrastructure.Realtime;

[Authorize]
public sealed class InventoryEventsHub : Hub
{
    public const string Route = "/hubs/inventory-events";
    public const string ReceiveMethod = "inventory-event";
}
