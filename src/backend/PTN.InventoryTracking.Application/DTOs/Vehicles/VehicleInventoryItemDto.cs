namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record VehicleInventoryItemDto(
    Guid ProductId,
    string ProductCode,
    string ProductName,
    string Unit,
    int Quantity);
