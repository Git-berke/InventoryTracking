namespace PTN.InventoryTracking.Persistence.Seed;

public static class SeedIds
{
    public static readonly Guid RoleAdmin = Guid.Parse("10101010-1010-1010-1010-101010101011");
    public static readonly Guid RoleWarehouseOperator = Guid.Parse("10101010-1010-1010-1010-101010101012");
    public static readonly Guid RoleTaskManager = Guid.Parse("10101010-1010-1010-1010-101010101013");

    public static readonly Guid UserAdmin = Guid.Parse("20202020-2020-2020-2020-202020202021");
    public static readonly Guid UserWarehouse = Guid.Parse("20202020-2020-2020-2020-202020202022");
    public static readonly Guid UserTaskManager = Guid.Parse("20202020-2020-2020-2020-202020202023");

    public static readonly Guid ProductRadio = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ProductGenerator = Guid.Parse("11111111-1111-1111-1111-111111111112");
    public static readonly Guid ProductFlashlight = Guid.Parse("11111111-1111-1111-1111-111111111113");

    public static readonly Guid WarehouseMain = Guid.Parse("22222222-2222-2222-2222-222222222221");
    public static readonly Guid WarehouseAnkara = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static readonly Guid VehicleIstanbul = Guid.Parse("33333333-3333-3333-3333-333333333331");
    public static readonly Guid VehicleAnkara = Guid.Parse("33333333-3333-3333-3333-333333333332");
    public static readonly Guid VehicleEskisehir = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static readonly Guid TaskIzmirSupport = Guid.Parse("44444444-4444-4444-4444-444444444441");
    public static readonly Guid TaskAnkaraEnergy = Guid.Parse("44444444-4444-4444-4444-444444444442");
    public static readonly Guid TaskBursaPrep = Guid.Parse("44444444-4444-4444-4444-444444444443");

    public static readonly Guid VehicleTaskIstanbulIzmir = Guid.Parse("55555555-5555-5555-5555-555555555551");
    public static readonly Guid VehicleTaskAnkaraIzmir = Guid.Parse("55555555-5555-5555-5555-555555555552");
    public static readonly Guid VehicleTaskEskisehirAnkara = Guid.Parse("55555555-5555-5555-5555-555555555553");

    public static readonly Guid StockLocationMainWarehouse = Guid.Parse("66666666-6666-6666-6666-666666666661");
    public static readonly Guid StockLocationAnkaraWarehouse = Guid.Parse("66666666-6666-6666-6666-666666666662");
    public static readonly Guid StockLocationVehicleIstanbul = Guid.Parse("66666666-6666-6666-6666-666666666663");
    public static readonly Guid StockLocationVehicleAnkara = Guid.Parse("66666666-6666-6666-6666-666666666664");
    public static readonly Guid StockLocationVehicleEskisehir = Guid.Parse("66666666-6666-6666-6666-666666666665");

    public static readonly Guid StockBalanceMainWarehouseRadio = Guid.Parse("77777777-7777-7777-7777-777777777771");
    public static readonly Guid StockBalanceVehicleIstanbulRadio = Guid.Parse("77777777-7777-7777-7777-777777777772");
    public static readonly Guid StockBalanceVehicleAnkaraRadio = Guid.Parse("77777777-7777-7777-7777-777777777773");
    public static readonly Guid StockBalanceAnkaraWarehouseGenerator = Guid.Parse("77777777-7777-7777-7777-777777777774");
    public static readonly Guid StockBalanceVehicleIstanbulGenerator = Guid.Parse("77777777-7777-7777-7777-777777777775");
    public static readonly Guid StockBalanceMainWarehouseFlashlight = Guid.Parse("77777777-7777-7777-7777-777777777776");
    public static readonly Guid StockBalanceVehicleEskisehirFlashlight = Guid.Parse("77777777-7777-7777-7777-777777777777");

    public static readonly Guid TransactionInitialRadioLoad = Guid.Parse("88888888-8888-8888-8888-888888888881");
    public static readonly Guid TransactionRadioToVehicleIstanbul = Guid.Parse("88888888-8888-8888-8888-888888888882");
    public static readonly Guid TransactionRadioToVehicleAnkara = Guid.Parse("88888888-8888-8888-8888-888888888883");
    public static readonly Guid TransactionInitialGeneratorLoad = Guid.Parse("88888888-8888-8888-8888-888888888884");
    public static readonly Guid TransactionGeneratorToVehicleIstanbul = Guid.Parse("88888888-8888-8888-8888-888888888885");
    public static readonly Guid TransactionInitialFlashlightLoad = Guid.Parse("88888888-8888-8888-8888-888888888886");
    public static readonly Guid TransactionFlashlightToVehicleEskisehir = Guid.Parse("88888888-8888-8888-8888-888888888887");
}
