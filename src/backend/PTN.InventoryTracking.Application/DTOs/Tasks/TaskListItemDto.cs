namespace PTN.InventoryTracking.Application.DTOs.Tasks;

public sealed record TaskListItemDto(
    Guid Id,
    string Name,
    string Region,
    string Status,
    DateOnly StartDate,
    DateOnly? EndDate);
