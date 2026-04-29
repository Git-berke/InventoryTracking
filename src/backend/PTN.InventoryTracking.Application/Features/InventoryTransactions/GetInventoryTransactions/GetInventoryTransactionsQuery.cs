namespace PTN.InventoryTracking.Application.Features.InventoryTransactions.GetInventoryTransactions;

public sealed record GetInventoryTransactionsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? ProductId = null,
    Guid? TaskId = null);
