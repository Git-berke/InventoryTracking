using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Common;
using PTN.InventoryTracking.Application.DTOs.InventoryTransactions;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.QueryServices;

public sealed class InventoryTransactionQueries(InventoryTrackingDbContext dbContext) : IInventoryTransactionQueries
{
    public async Task<PagedResult<InventoryTransactionListItemDto>> GetTransactionsAsync(
        int page,
        int pageSize,
        Guid? productId,
        Guid? taskId,
        CancellationToken cancellationToken = default)
    {
        page = NormalizePage(page);
        pageSize = NormalizePageSize(pageSize);

        var query = dbContext.InventoryTransactions
            .AsNoTracking()
            .AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == productId.Value);
        }

        if (taskId.HasValue)
        {
            query = query.Where(x => x.TaskId == taskId.Value);
        }

        query = query
            .OrderByDescending(x => x.PerformedAtUtc)
            .ThenByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new InventoryTransactionListItemDto(
                x.Id,
                x.ProductId,
                x.Product.Code,
                x.Product.Name,
                x.TransactionType.ToString(),
                x.Quantity,
                x.PerformedAtUtc,
                x.SourceStockLocation != null ? x.SourceStockLocation.Name : null,
                x.DestinationStockLocation != null ? x.DestinationStockLocation.Name : null,
                x.Task != null ? x.Task.Name : null,
                x.ReferenceNote))
            .ToListAsync(cancellationToken);

        return new PagedResult<InventoryTransactionListItemDto>(items, totalCount, page, pageSize);
    }

    private static int NormalizePage(int page) => page < 1 ? 1 : page;
    private static int NormalizePageSize(int pageSize) => pageSize <= 0 ? 20 : Math.Min(pageSize, 100);
}
