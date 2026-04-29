using Microsoft.Extensions.DependencyInjection;
using PTN.InventoryTracking.Application.Features.InventoryTransactions.GetInventoryTransactions;
using PTN.InventoryTracking.Application.Features.Products.GetProducts;
using PTN.InventoryTracking.Application.Features.Products.GetProductStockSummary;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskInventory;
using PTN.InventoryTracking.Application.Features.Tasks.GetTasks;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskVehicles;
using PTN.InventoryTracking.Application.Features.Vehicles.GetVehicleInventories;
using PTN.InventoryTracking.Application.Features.Vehicles.GetVehicles;

namespace PTN.InventoryTracking.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<GetProductsHandler>();
        services.AddTransient<GetProductStockSummaryHandler>();
        services.AddTransient<GetVehiclesHandler>();
        services.AddTransient<GetVehicleInventoriesHandler>();
        services.AddTransient<GetTasksHandler>();
        services.AddTransient<GetTaskVehiclesHandler>();
        services.AddTransient<GetTaskInventoryHandler>();
        services.AddTransient<GetInventoryTransactionsHandler>();

        return services;
    }
}
