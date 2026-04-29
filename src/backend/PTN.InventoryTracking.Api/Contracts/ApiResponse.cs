namespace PTN.InventoryTracking.Api.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message,
    string TraceId);
