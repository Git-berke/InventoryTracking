namespace PTN.InventoryTracking.Application.DTOs.Realtime;

public sealed record InventoryRealtimeEventDto(
    string EventType,
    string Message,
    DateTime OccurredAtUtc,
    IReadOnlyDictionary<string, string?> Context);
