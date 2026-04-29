using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Application.DTOs.Tasks;

public sealed record TaskDetailDto(
    Guid Id,
    string Name,
    string? Description,
    string Region,
    DateOnly StartDate,
    DateOnly? EndDate,
    InventoryTaskStatus Status);
