using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Remove(Product product);
}
