using PTN.InventoryTracking.Application.DTOs.Tasks;

namespace PTN.InventoryTracking.Application.Abstractions.Services;

public interface ITaskManagementService
{
    Task<TaskDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskDetailDto> CreateAsync(CreateTaskRequestDto request, CancellationToken cancellationToken = default);
    Task<TaskDetailDto?> UpdateAsync(Guid id, UpdateTaskRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
