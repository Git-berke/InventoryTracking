namespace PTN.InventoryTracking.Application.DTOs.Tasks;

public sealed record TaskInventoryItemDto(
    Guid VehicleId,
    string LicensePlate,
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string Unit,
    int Quantity);
