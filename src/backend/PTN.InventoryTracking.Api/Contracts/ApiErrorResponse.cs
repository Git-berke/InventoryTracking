namespace PTN.InventoryTracking.Api.Contracts;

public sealed record ApiErrorResponse(
    bool Success,
    string Code,
    string Message,
    string TraceId,
    IDictionary<string, string[]>? Errors = null);
