using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Api.Contracts;

namespace PTN.InventoryTracking.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult OkResponse<T>(T data, string? message = null) =>
        Ok(new ApiResponse<T>(true, data, message, HttpContext.TraceIdentifier));

    protected IActionResult CreatedResponse<T>(
        string actionName,
        object routeValues,
        T data,
        string? message = null) =>
        CreatedAtAction(
            actionName,
            routeValues,
            new ApiResponse<T>(true, data, message, HttpContext.TraceIdentifier));

    protected IActionResult AcceptedResponse<T>(T data, string? message = null) =>
        Accepted(new ApiResponse<T>(true, data, message, HttpContext.TraceIdentifier));

    protected IActionResult NotFoundResponse(string message = "Resource not found.") =>
        NotFound(new ApiErrorResponse(false, "not_found", message, HttpContext.TraceIdentifier));

    protected IActionResult UnauthorizedResponse(string message = "Unauthorized.") =>
        Unauthorized(new ApiErrorResponse(false, "unauthorized", message, HttpContext.TraceIdentifier));
}
