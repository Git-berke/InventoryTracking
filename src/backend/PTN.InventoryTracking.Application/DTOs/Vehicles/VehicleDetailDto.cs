namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record VehicleDetailDto(
    Guid Id,
    string Code,
    string LicensePlate,
    string VehicleType,
    bool IsActive);
