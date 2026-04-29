using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.InventoryTransactions;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IInventoryTransactionQueries
{
    Task<PagedResult<InventoryTransactionListItemDto>> GetTransactionsAsync(
        int page,
        int pageSize,
        Guid? productId,
        Guid? taskId,
        CancellationToken cancellationToken = default);
}
