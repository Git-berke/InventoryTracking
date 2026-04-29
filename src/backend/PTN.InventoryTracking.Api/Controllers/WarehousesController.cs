using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Warehouses;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
public sealed class WarehousesController(
    IWarehouseManagementService warehouseManagementService) : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWarehouse(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await warehouseManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWarehouse(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await warehouseManagementService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
