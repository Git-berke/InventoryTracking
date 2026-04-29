using PTN.InventoryTracking.Application.DTOs.Warehouses;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface IWarehouseManagementService
{
    Task<WarehouseDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WarehouseDetailDto> CreateAsync(CreateWarehouseRequestDto request, CancellationToken cancellationToken = default);
    Task<WarehouseDetailDto?> UpdateAsync(Guid id, UpdateWarehouseRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
