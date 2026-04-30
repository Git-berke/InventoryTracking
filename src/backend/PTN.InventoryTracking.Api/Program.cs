using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
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
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy
            .SetIsOriginAllowed(origin =>
                new Uri(origin).Host == "localhost")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PTN Inventory Tracking API",
        Version = "v1",
        Description = "Inventory tracking API. Use POST /api/v1/auth/login to obtain a JWT, then click 'Authorize' and paste the token (without the 'Bearer ' prefix)."
    });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Bearer token. Paste only the token value, the 'Bearer ' prefix is added automatically.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "PTN Inventory Tracking Swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<InventoryEventsHub>(InventoryEventsHub.Route);

app.Run();
