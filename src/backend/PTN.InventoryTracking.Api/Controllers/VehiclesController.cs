using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Features.Vehicles.GetVehicleInventories;
using PTN.InventoryTracking.Application.Features.Vehicles.GetVehicles;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/vehicles")]
public sealed class VehiclesController(
    GetVehiclesHandler getVehiclesHandler,
    GetVehicleInventoriesHandler getVehicleInventoriesHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetVehicles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getVehiclesHandler.HandleAsync(
            new GetVehiclesQuery(page, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}/inventories")]
    public async Task<IActionResult> GetVehicleInventories(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getVehicleInventoriesHandler.HandleAsync(
            new GetVehicleInventoriesQuery(id),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
