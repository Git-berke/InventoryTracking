using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PTN.InventoryTracking.Api.Contracts;

namespace PTN.InventoryTracking.Api.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred while processing request {Path}.", context.Request.Path);
            await WriteErrorResponseAsync(context, exception);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var (statusCode, code, message) = MapException(exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse(
            false,
            code,
            message,
            context.TraceIdentifier);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, SerializerOptions));
    }

    private static (int StatusCode, string Code, string Message) MapException(Exception exception) =>
        exception switch
        {
            ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, "validation_error", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "validation_error", exception.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "business_rule_violation", exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "unauthorized", exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "internal_server_error", "An unexpected error occurred.")
        };
}
