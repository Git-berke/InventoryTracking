namespace PTN.InventoryTracking.Application.DTOs.Tasks;

public sealed record TaskVehicleDto(
    Guid VehicleId,
    string VehicleCode,
    string LicensePlate,
    string VehicleType,
    DateTime AssignedAtUtc,
    DateTime? ReleasedAtUtc);
