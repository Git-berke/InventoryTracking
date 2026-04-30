using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Warehouses;
using PTN.InventoryTracking.Application.Features.Warehouses.GetWarehouses;
using PTN.InventoryTracking.Application.Security;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
public sealed class WarehousesController(
    GetWarehousesHandler getWarehousesHandler,
    IWarehouseManagementService warehouseManagementService) : ApiControllerBase
{
    [Authorize(Policy = PermissionNames.WarehousesRead)]
    [HttpGet]
    public async Task<IActionResult> GetWarehouses(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getWarehousesHandler.HandleAsync(
            new GetWarehousesQuery(page, pageSize),
            cancellationToken);
        return OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.WarehousesRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWarehouse(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await warehouseManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFoundResponse("Warehouse could not be found.") : OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.WarehousesCreate)]
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(
        [FromBody] CreateWarehouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await warehouseManagementService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetWarehouse), new { id = result.Id }, result, "Warehouse created successfully.");
    }

    [Authorize(Policy = PermissionNames.WarehousesUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWarehouse(
        Guid id,
        [FromBody] UpdateWarehouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await warehouseManagementService.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFoundResponse("Warehouse could not be found.") : OkResponse(result, "Warehouse updated successfully.");
    }

    [Authorize(Policy = PermissionNames.WarehousesDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWarehouse(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await warehouseManagementService.DeleteAsync(id, cancellationToken);
        return deleted
            ? OkResponse(new { deleted = true }, "Warehouse deleted successfully.")
            : NotFoundResponse("Warehouse could not be found.");
    }
}
