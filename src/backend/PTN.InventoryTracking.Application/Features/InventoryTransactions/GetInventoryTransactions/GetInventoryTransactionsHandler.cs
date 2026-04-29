using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.InventoryTransactions;

namespace PTN.InventoryTracking.Application.Features.InventoryTransactions.GetInventoryTransactions;

public sealed class GetInventoryTransactionsHandler(IInventoryTransactionQueries inventoryTransactionQueries)
{
    public Task<PagedResult<InventoryTransactionListItemDto>> HandleAsync(
        GetInventoryTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        return inventoryTransactionQueries.GetTransactionsAsync(
            query.Page,
            query.PageSize,
            query.ProductId,
            query.TaskId,
            cancellationToken);
    }
}
