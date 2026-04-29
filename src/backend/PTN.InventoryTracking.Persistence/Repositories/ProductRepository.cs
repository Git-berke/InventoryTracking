using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Persistence.Contexts;

namespace PTN.InventoryTracking.Persistence.Repositories;

public sealed class ProductRepository(InventoryTrackingDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Products.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Products.AnyAsync(x => x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        dbContext.Products.AddAsync(product, cancellationToken).AsTask();

    public void Remove(Product product) => dbContext.Products.Remove(product);
}
