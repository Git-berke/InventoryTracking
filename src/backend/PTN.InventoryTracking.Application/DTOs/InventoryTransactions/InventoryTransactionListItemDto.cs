namespace PTN.InventoryTracking.Application.DTOs.InventoryTransactions;

public sealed record InventoryTransactionListItemDto(
    Guid Id,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string TransactionType,
    int Quantity,
    DateTime PerformedAtUtc,
    string? SourceLocationName,
    string? DestinationLocationName,
    string? TaskName,
    string? ReferenceNote);
