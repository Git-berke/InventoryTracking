namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record CreateVehicleRequestDto(
    string Code,
    string LicensePlate,
    string VehicleType);
