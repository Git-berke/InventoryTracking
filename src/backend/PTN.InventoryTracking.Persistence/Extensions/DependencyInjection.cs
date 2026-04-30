using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTN.InventoryTracking.Application.Abstractions.Persistence;
using PTN.InventoryTracking.Application.Abstractions.Persistence.Repositories;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Persistence.Contexts;
using PTN.InventoryTracking.Persistence.QueryServices;
using PTN.InventoryTracking.Persistence.Repositories;
using PTN.InventoryTracking.Persistence.Services;

namespace PTN.InventoryTracking.Persistence.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("InventoryTracking")
            ?? Environment.GetEnvironmentVariable("PTN_INVENTORY_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=ptn_inventory_tracking;Username=postgres;Password=postgres";

        services.AddDbContext<InventoryTrackingDbContext>(options =>
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IInventoryTrackingDbContext>(provider => provider.GetRequiredService<InventoryTrackingDbContext>());
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductQueries, ProductQueries>();
        services.AddScoped<IWarehouseQueries, WarehouseQueries>();
        services.AddScoped<IVehicleQueries, VehicleQueries>();
        services.AddScoped<ITaskQueries, TaskQueries>();
        services.AddScoped<IInventoryTransactionQueries, InventoryTransactionQueries>();
        services.AddScoped<IStockTransferService, StockTransferService>();
        services.AddScoped<IProductManagementService, ProductManagementService>();
        services.AddScoped<IWarehouseManagementService, WarehouseManagementService>();
        services.AddScoped<IVehicleManagementService, VehicleManagementService>();
        services.AddScoped<ITaskManagementService, TaskManagementService>();

        return services;
    }
}
