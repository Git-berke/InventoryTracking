using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Vehicles;
using PTN.InventoryTracking.Application.Features.Vehicles.GetVehicleInventories;
using PTN.InventoryTracking.Application.Features.Vehicles.GetVehicles;
using PTN.InventoryTracking.Application.Security;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/vehicles")]
public sealed class VehiclesController(
    GetVehiclesHandler getVehiclesHandler,
    GetVehicleInventoriesHandler getVehicleInventoriesHandler,
    IVehicleManagementService vehicleManagementService) : ApiControllerBase
{
    [Authorize(Policy = PermissionNames.VehiclesRead)]
    [HttpGet]
    public async Task<IActionResult> GetVehicles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getVehiclesHandler.HandleAsync(
            new GetVehiclesQuery(page, pageSize),
            cancellationToken);

        return OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.VehiclesRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVehicle(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await vehicleManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFoundResponse("Vehicle could not be found.") : OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.VehiclesRead)]
    [HttpGet("{id:guid}/inventories")]
    public async Task<IActionResult> GetVehicleInventories(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getVehicleInventoriesHandler.HandleAsync(
            new GetVehicleInventoriesQuery(id),
            cancellationToken);

        return result is null ? NotFoundResponse("Vehicle inventory could not be found.") : OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.VehiclesCreate)]
    [HttpPost]
    public async Task<IActionResult> CreateVehicle(
        [FromBody] CreateVehicleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await vehicleManagementService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetVehicle), new { id = result.Id }, result, "Vehicle created successfully.");
    }

    [Authorize(Policy = PermissionNames.VehiclesUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateVehicle(
        Guid id,
        [FromBody] UpdateVehicleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await vehicleManagementService.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFoundResponse("Vehicle could not be found.") : OkResponse(result, "Vehicle updated successfully.");
    }

    [Authorize(Policy = PermissionNames.VehiclesDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteVehicle(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await vehicleManagementService.DeleteAsync(id, cancellationToken);
        return deleted
            ? OkResponse(new { deleted = true }, "Vehicle deleted successfully.")
            : NotFoundResponse("Vehicle could not be found.");
    }
}
