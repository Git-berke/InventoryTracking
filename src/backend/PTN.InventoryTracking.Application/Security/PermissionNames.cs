namespace PTN.InventoryTracking.Application.Security;

public static class PermissionNames
{
    public const string ProductsRead = "products.read";
    public const string ProductsCreate = "products.create";
    public const string ProductsUpdate = "products.update";
    public const string ProductsDelete = "products.delete";

    public const string WarehousesRead = "warehouses.read";
    public const string WarehousesCreate = "warehouses.create";
    public const string WarehousesUpdate = "warehouses.update";
    public const string WarehousesDelete = "warehouses.delete";

    public const string VehiclesRead = "vehicles.read";
    public const string VehiclesCreate = "vehicles.create";
    public const string VehiclesUpdate = "vehicles.update";
    public const string VehiclesDelete = "vehicles.delete";

    public const string TasksRead = "tasks.read";
    public const string TasksCreate = "tasks.create";
    public const string TasksUpdate = "tasks.update";
    public const string TasksDelete = "tasks.delete";

    public const string InventoryTransactionsRead = "inventory-transactions.read";
    public const string StockTransfersCreate = "stock-transfers.create";

    public static IReadOnlyCollection<string> All { get; } =
    [
        ProductsRead,
        ProductsCreate,
        ProductsUpdate,
        ProductsDelete,
        WarehousesRead,
        WarehousesCreate,
        WarehousesUpdate,
        WarehousesDelete,
        VehiclesRead,
        VehiclesCreate,
        VehiclesUpdate,
        VehiclesDelete,
        TasksRead,
        TasksCreate,
        TasksUpdate,
        TasksDelete,
        InventoryTransactionsRead,
        StockTransfersCreate
    ];
}
