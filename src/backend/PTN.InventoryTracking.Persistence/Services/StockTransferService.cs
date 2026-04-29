using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Domain.Enums;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.Services;

public sealed class StockTransferService(InventoryTrackingDbContext dbContext) : IStockTransferService
{
    public async Task TransferWarehouseToVehicleAsync(
        Guid productId,
        Guid sourceWarehouseId,
        Guid destinationVehicleId,
        int quantity,
        Guid taskId,
        string? referenceNote,
        CancellationToken cancellationToken = default)
    {
        ValidateQuantity(quantity);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var productExists = await dbContext.Products
            .AnyAsync(x => x.Id == productId && x.IsActive, cancellationToken);

        if (!productExists)
        {
            throw new InvalidOperationException("Active product could not be found.");
        }

        var sourceLocation = await dbContext.StockLocations
            .SingleOrDefaultAsync(x => x.WarehouseId == sourceWarehouseId && x.IsActive, cancellationToken);

        if (sourceLocation is null)
        {
            throw new InvalidOperationException("Source warehouse stock location could not be found.");
        }

        var destinationLocation = await dbContext.StockLocations
            .SingleOrDefaultAsync(x => x.VehicleId == destinationVehicleId && x.IsActive, cancellationToken);

        if (destinationLocation is null)
        {
            throw new InvalidOperationException("Destination vehicle stock location could not be found.");
        }

        var task = await dbContext.Tasks
            .SingleOrDefaultAsync(x => x.Id == taskId, cancellationToken);

        if (task is null)
        {
            throw new InvalidOperationException("Task could not be found.");
        }

        if (task.Status != InventoryTaskStatus.InProgress)
        {
            throw new InvalidOperationException("Inventory can only be transferred to a vehicle for an in-progress task.");
        }

        var vehicleAssignedToTask = await dbContext.VehicleTasks
            .AnyAsync(x =>
                x.TaskId == taskId &&
                x.VehicleId == destinationVehicleId &&
                x.ReleasedAtUtc == null,
                cancellationToken);

        if (!vehicleAssignedToTask)
        {
            throw new InvalidOperationException("Vehicle is not actively assigned to the given task.");
        }

        var sourceBalance = await dbContext.StockBalances
            .SingleOrDefaultAsync(
                x => x.ProductId == productId && x.StockLocationId == sourceLocation.Id,
                cancellationToken);

        if (sourceBalance is null || sourceBalance.Quantity < quantity)
        {
            throw new InvalidOperationException("Insufficient warehouse stock for this transfer.");
        }

        var destinationBalance = await GetOrCreateStockBalanceAsync(
            productId,
            destinationLocation.Id,
            cancellationToken);

        sourceBalance.Quantity -= quantity;
        sourceBalance.UpdatedAtUtc = DateTime.UtcNow;

        destinationBalance.Quantity += quantity;
        destinationBalance.UpdatedAtUtc = DateTime.UtcNow;

        dbContext.InventoryTransactions.Add(new InventoryTransaction
        {
            ProductId = productId,
            SourceStockLocationId = sourceLocation.Id,
            DestinationStockLocationId = destinationLocation.Id,
            TaskId = taskId,
            TransactionType = InventoryTransactionType.WarehouseToVehicle,
            Quantity = quantity,
            PerformedAtUtc = DateTime.UtcNow,
            ReferenceNote = referenceNote
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ReturnVehicleToWarehouseAsync(
        Guid productId,
        Guid sourceVehicleId,
        Guid destinationWarehouseId,
        int quantity,
        string? referenceNote,
        CancellationToken cancellationToken = default)
    {
        ValidateQuantity(quantity);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var sourceLocation = await dbContext.StockLocations
            .SingleOrDefaultAsync(x => x.VehicleId == sourceVehicleId && x.IsActive, cancellationToken);

        if (sourceLocation is null)
        {
            throw new InvalidOperationException("Source vehicle stock location could not be found.");
        }

        var destinationLocation = await dbContext.StockLocations
            .SingleOrDefaultAsync(x => x.WarehouseId == destinationWarehouseId && x.IsActive, cancellationToken);

        if (destinationLocation is null)
        {
            throw new InvalidOperationException("Destination warehouse stock location could not be found.");
        }

        var sourceBalance = await dbContext.StockBalances
            .SingleOrDefaultAsync(
                x => x.ProductId == productId && x.StockLocationId == sourceLocation.Id,
                cancellationToken);

        if (sourceBalance is null || sourceBalance.Quantity < quantity)
        {
            throw new InvalidOperationException("Insufficient vehicle stock for this return.");
        }

        var destinationBalance = await GetOrCreateStockBalanceAsync(
            productId,
            destinationLocation.Id,
            cancellationToken);

        var activeTaskId = await dbContext.VehicleTasks
            .Where(x => x.VehicleId == sourceVehicleId && x.ReleasedAtUtc == null)
            .OrderByDescending(x => x.AssignedAtUtc)
            .Select(x => (Guid?)x.TaskId)
            .FirstOrDefaultAsync(cancellationToken);

        sourceBalance.Quantity -= quantity;
        sourceBalance.UpdatedAtUtc = DateTime.UtcNow;

        destinationBalance.Quantity += quantity;
        destinationBalance.UpdatedAtUtc = DateTime.UtcNow;

        dbContext.InventoryTransactions.Add(new InventoryTransaction
        {
            ProductId = productId,
            SourceStockLocationId = sourceLocation.Id,
            DestinationStockLocationId = destinationLocation.Id,
            TaskId = activeTaskId,
            TransactionType = InventoryTransactionType.VehicleToWarehouse,
            Quantity = quantity,
            PerformedAtUtc = DateTime.UtcNow,
            ReferenceNote = referenceNote
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task<StockBalance> GetOrCreateStockBalanceAsync(
        Guid productId,
        Guid stockLocationId,
        CancellationToken cancellationToken)
    {
        var balance = await dbContext.StockBalances
            .SingleOrDefaultAsync(
                x => x.ProductId == productId && x.StockLocationId == stockLocationId,
                cancellationToken);

        if (balance is not null)
        {
            return balance;
        }

        balance = new StockBalance
        {
            ProductId = productId,
            StockLocationId = stockLocationId,
            Quantity = 0
        };

        dbContext.StockBalances.Add(balance);
        return balance;
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        }
    }
}
