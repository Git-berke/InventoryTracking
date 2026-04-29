using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Features.InventoryTransactions.GetInventoryTransactions;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/inventory-transactions")]
public sealed class InventoryTransactionsController(GetInventoryTransactionsHandler getInventoryTransactionsHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? productId = null,
        [FromQuery] Guid? taskId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await getInventoryTransactionsHandler.HandleAsync(
            new GetInventoryTransactionsQuery(page, pageSize, productId, taskId),
            cancellationToken);

        return Ok(result);
    }
}
