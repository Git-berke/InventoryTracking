using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Api.Contracts;
using PTN.InventoryTracking.Api.Middleware;
using PTN.InventoryTracking.Application.Extensions;
using PTN.InventoryTracking.Infrastructure.Extensions;
using PTN.InventoryTracking.Infrastructure.Realtime;
using PTN.InventoryTracking.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(error =>
                    string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The value is invalid." : error.ErrorMessage).ToArray());

        return new BadRequestObjectResult(
            new ApiErrorResponse(
                false,
                "validation_error",
                "One or more validation errors occurred.",
                context.HttpContext.TraceIdentifier,
                errors));
    };
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<InventoryEventsHub>(InventoryEventsHub.Route);

app.Run();
