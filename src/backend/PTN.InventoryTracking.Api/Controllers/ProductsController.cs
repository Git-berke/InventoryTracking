using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Products;
using PTN.InventoryTracking.Application.Features.Products.GetProducts;
using PTN.InventoryTracking.Application.Features.Products.GetProductStockSummary;
using PTN.InventoryTracking.Application.Security;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController(
    GetProductsHandler getProductsHandler,
    GetProductStockSummaryHandler getProductStockSummaryHandler,
    IProductManagementService productManagementService) : ApiControllerBase
{
    [Authorize(Policy = PermissionNames.ProductsRead)]
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

    [Authorize(Policy = PermissionNames.ProductsRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await productManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Policy = PermissionNames.ProductsRead)]
    [HttpGet("{id:guid}/stock-summary")]
    public async Task<IActionResult> GetStockSummary(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getProductStockSummaryHandler.HandleAsync(
            new GetProductStockSummaryQuery(id),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Policy = PermissionNames.ProductsCreate)]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await productManagementService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
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

    [Authorize(Policy = PermissionNames.ProductsUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await productManagementService.UpdateAsync(id, request, cancellationToken);
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

    [Authorize(Policy = PermissionNames.ProductsDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await productManagementService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
