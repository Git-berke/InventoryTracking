using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.StockTransfers;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/stock-transfers")]
public sealed class StockTransfersController(
    IStockTransferService stockTransferService) : ApiControllerBase
{
    [HttpPost("warehouse-to-vehicle")]
    public async Task<IActionResult> TransferWarehouseToVehicle(
        [FromBody] TransferWarehouseToVehicleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await stockTransferService.TransferWarehouseToVehicleAsync(
                request.ProductId,
                request.SourceWarehouseId,
                request.DestinationVehicleId,
                request.Quantity,
                request.TaskId,
                request.ReferenceNote,
                cancellationToken);

            return NoContent();
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return ValidationProblemResponse(exception);
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

    [HttpPost("vehicle-to-warehouse")]
    public async Task<IActionResult> ReturnVehicleToWarehouse(
        [FromBody] ReturnVehicleToWarehouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await stockTransferService.ReturnVehicleToWarehouseAsync(
                request.ProductId,
                request.SourceVehicleId,
                request.DestinationWarehouseId,
                request.Quantity,
                request.ReferenceNote,
                cancellationToken);

            return NoContent();
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return ValidationProblemResponse(exception);
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
}
