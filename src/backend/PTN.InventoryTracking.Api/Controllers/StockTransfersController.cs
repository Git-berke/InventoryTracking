using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.StockTransfers;
using PTN.InventoryTracking.Application.Security;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/stock-transfers")]
public sealed class StockTransfersController(
    IStockTransferService stockTransferService) : ApiControllerBase
{
    [Authorize(Policy = PermissionNames.StockTransfersCreate)]
    [HttpPost("warehouse-to-vehicle")]
    public async Task<IActionResult> TransferWarehouseToVehicle(
        [FromBody] TransferWarehouseToVehicleRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await stockTransferService.TransferWarehouseToVehicleAsync(
            request.ProductId,
            request.SourceWarehouseId,
            request.DestinationVehicleId,
            request.Quantity,
            request.TaskId,
            request.ReferenceNote,
            cancellationToken);

        return OkResponse(new { completed = true }, "Stock transferred from warehouse to vehicle successfully.");
    }

    [Authorize(Policy = PermissionNames.StockTransfersCreate)]
    [HttpPost("vehicle-to-warehouse")]
    public async Task<IActionResult> ReturnVehicleToWarehouse(
        [FromBody] ReturnVehicleToWarehouseRequestDto request,
        CancellationToken cancellationToken = default)
    {
        await stockTransferService.ReturnVehicleToWarehouseAsync(
            request.ProductId,
            request.SourceVehicleId,
            request.DestinationWarehouseId,
            request.Quantity,
            request.ReferenceNote,
            cancellationToken);

        return OkResponse(new { completed = true }, "Stock returned from vehicle to warehouse successfully.");
    }
}
