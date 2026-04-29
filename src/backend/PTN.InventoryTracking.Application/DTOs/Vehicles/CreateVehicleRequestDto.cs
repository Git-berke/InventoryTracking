using System.ComponentModel.DataAnnotations;

namespace PTN.InventoryTracking.Application.DTOs.Vehicles;

public sealed record CreateVehicleRequestDto
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Code { get; init; } = string.Empty;

    [Required, StringLength(20, MinimumLength = 3)]
    public string LicensePlate { get; init; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 2)]
    public string VehicleType { get; init; } = string.Empty;
}
