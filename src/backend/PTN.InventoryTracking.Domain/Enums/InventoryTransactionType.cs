namespace PTN.InventoryTracking.Domain.Enums;

public enum InventoryTransactionType
{
    InitialLoad = 1,
    WarehouseToVehicle = 2,
    VehicleToWarehouse = 3,
    WarehouseToWarehouse = 4,
    VehicleToVehicle = 5,
    AdjustmentIn = 6,
    AdjustmentOut = 7
}
