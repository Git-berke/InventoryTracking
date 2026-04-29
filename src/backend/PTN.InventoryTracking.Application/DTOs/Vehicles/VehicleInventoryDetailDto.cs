namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record VehicleInventoryDetailDto(
    Guid VehicleId,
    string VehicleCode,
    string LicensePlate,
    string VehicleType,
    string? ActiveTaskName,
    IReadOnlyCollection<VehicleInventoryItemDto> Inventories);
