namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record UpdateVehicleRequestDto(
    string Code,
    string LicensePlate,
    string VehicleType,
    bool IsActive);
