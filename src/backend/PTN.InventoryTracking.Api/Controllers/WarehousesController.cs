using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Warehouses;
using PTN.InventoryTracking.Application.Security;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
public sealed class WarehousesController(
    IWarehouseManagementService warehouseManagementService) : ApiControllerBase
{
    [Authorize(Policy = PermissionNames.WarehousesRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWarehouse(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await warehouseManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Policy = PermissionNames.WarehousesCreate)]
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse(
        [FromBody] CreateWarehouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await warehouseManagementService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetWarehouse), new { id = result.Id }, result);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblemResponse(exception);
        }
        catch (InvalidOperationException exception)
        {
            return ValidationProblemResponse(exception);
        }
    }

    [Authorize(Policy = PermissionNames.WarehousesUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWarehouse(
        Guid id,
        [FromBody] UpdateWarehouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await warehouseManagementService.UpdateAsync(id, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblemResponse(exception);
        }
        catch (InvalidOperationException exception)
        {
            return ValidationProblemResponse(exception);
        }
    }

    [Authorize(Policy = PermissionNames.WarehousesDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWarehouse(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await warehouseManagementService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
