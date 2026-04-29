namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record VehicleListItemDto(
    Guid Id,
    string Code,
    string LicensePlate,
    string VehicleType,
    bool IsActive,
    string? ActiveTaskName);
