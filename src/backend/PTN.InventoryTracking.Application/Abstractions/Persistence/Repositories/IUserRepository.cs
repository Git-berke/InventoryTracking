using PTN.InventoryTracking.Domain.Entities;

namespace PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
