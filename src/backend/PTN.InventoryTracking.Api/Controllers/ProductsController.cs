using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Features.Products.GetProducts;
using PTN.InventoryTracking.Application.Features.Products.GetProductStockSummary;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController(
    GetProductsHandler getProductsHandler,
    GetProductStockSummaryHandler getProductStockSummaryHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getProductsHandler.HandleAsync(
            new GetProductsQuery(page, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}/stock-summary")]
    public async Task<IActionResult> GetStockSummary(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getProductStockSummaryHandler.HandleAsync(
            new GetProductStockSummaryQuery(id),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
