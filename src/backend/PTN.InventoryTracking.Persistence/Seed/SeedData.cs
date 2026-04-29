using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Application.Security;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace PTN.InventoryTracking.Persistence.Seed;

public static class SeedData
{
    private static readonly DateTime SeedCreatedAtUtc = new(2026, 4, 29, 7, 0, 0, DateTimeKind.Utc);

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppPermission>().HasData(
            CreatePermission(PermissionNames.ProductsRead, "Products Read", "Read product and stock summary endpoints"),
            CreatePermission(PermissionNames.ProductsCreate, "Products Create", "Create product records"),
            CreatePermission(PermissionNames.ProductsUpdate, "Products Update", "Update product records"),
            CreatePermission(PermissionNames.ProductsDelete, "Products Delete", "Delete product records"),
            CreatePermission(PermissionNames.WarehousesRead, "Warehouses Read", "Read warehouse records"),
            CreatePermission(PermissionNames.WarehousesCreate, "Warehouses Create", "Create warehouse records"),
            CreatePermission(PermissionNames.WarehousesUpdate, "Warehouses Update", "Update warehouse records"),
            CreatePermission(PermissionNames.WarehousesDelete, "Warehouses Delete", "Delete warehouse records"),
            CreatePermission(PermissionNames.VehiclesRead, "Vehicles Read", "Read vehicle and vehicle inventory endpoints"),
            CreatePermission(PermissionNames.VehiclesCreate, "Vehicles Create", "Create vehicle records"),
            CreatePermission(PermissionNames.VehiclesUpdate, "Vehicles Update", "Update vehicle records"),
            CreatePermission(PermissionNames.VehiclesDelete, "Vehicles Delete", "Delete vehicle records"),
            CreatePermission(PermissionNames.TasksRead, "Tasks Read", "Read task, task vehicle and task inventory endpoints"),
            CreatePermission(PermissionNames.TasksCreate, "Tasks Create", "Create task records"),
            CreatePermission(PermissionNames.TasksUpdate, "Tasks Update", "Update task records"),
            CreatePermission(PermissionNames.TasksDelete, "Tasks Delete", "Delete task records"),
            CreatePermission(PermissionNames.InventoryTransactionsRead, "Inventory Transactions Read", "Read inventory transaction history"),
            CreatePermission(PermissionNames.StockTransfersCreate, "Stock Transfers Create", "Perform stock transfer operations"));

        modelBuilder.Entity<AppRole>().HasData(
            new AppRole
            {
                Id = SeedIds.RoleAdmin,
                Code = "ROLE-ADMIN",
                Name = RoleNames.Admin,
                Description = "Full access role",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new AppRole
            {
                Id = SeedIds.RoleWarehouseOperator,
                Code = "ROLE-WAREHOUSE-OPERATOR",
                Name = RoleNames.WarehouseOperator,
                Description = "Warehouse and stock transfer operator role",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new AppRole
            {
                Id = SeedIds.RoleTaskManager,
                Code = "ROLE-TASK-MANAGER",
                Name = RoleNames.TaskManager,
                Description = "Task and vehicle management role",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = SeedIds.UserAdmin,
                Email = "admin@ptn.local",
                UserName = "admin",
                FullName = "System Administrator",
                PasswordHash = "pbkdf2$100000$adQyMFF/MEDyqS1mZ08ATA==$L4rcPrvAUy/+bShxsmO+6YvNZ4t+5hOBVnjdoROjk68=",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new AppUser
            {
                Id = SeedIds.UserWarehouse,
                Email = "warehouse@ptn.local",
                UserName = "warehouse.operator",
                FullName = "Warehouse Operator",
                PasswordHash = "pbkdf2$100000$fmXCFcONlxHDNILtMYT/tg==$l5v3quqi3kNjhqqcLC7jNEBW2RW0LNmHxwY2xAgI7Gs=",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new AppUser
            {
                Id = SeedIds.UserTaskManager,
                Email = "taskmanager@ptn.local",
                UserName = "task.manager",
                FullName = "Task Manager",
                PasswordHash = "pbkdf2$100000$mNGvVDSa8GdorMSan+8s/A==$Ht44IbT0Q6iij1kUu47u+DZ3IKTJTTCJdhk/n7fiZro=",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<AppUserRole>().HasData(
            CreateUserRole(SeedIds.UserAdmin, SeedIds.RoleAdmin),
            CreateUserRole(SeedIds.UserWarehouse, SeedIds.RoleWarehouseOperator),
            CreateUserRole(SeedIds.UserTaskManager, SeedIds.RoleTaskManager));

        modelBuilder.Entity<AppRolePermission>().HasData(
            PermissionNames.All.Select(permissionCode => CreateRolePermission(
                SeedIds.RoleAdmin,
                GetPermissionId(permissionCode))).ToArray());

        modelBuilder.Entity<AppRolePermission>().HasData(
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.ProductsRead)),
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.WarehousesRead)),
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.WarehousesUpdate)),
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.VehiclesRead)),
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.TasksRead)),
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.InventoryTransactionsRead)),
            CreateRolePermission(SeedIds.RoleWarehouseOperator, GetPermissionId(PermissionNames.StockTransfersCreate)));

        modelBuilder.Entity<AppRolePermission>().HasData(
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.ProductsRead)),
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.VehiclesRead)),
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.VehiclesUpdate)),
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.TasksRead)),
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.TasksCreate)),
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.TasksUpdate)),
            CreateRolePermission(SeedIds.RoleTaskManager, GetPermissionId(PermissionNames.InventoryTransactionsRead)));

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = SeedIds.ProductRadio,
                Code = "PRD-RADIO-001",
                Name = "Telsiz",
                Description = "Saha ekipleri icin el telsizi",
                Unit = "adet",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new Product
            {
                Id = SeedIds.ProductGenerator,
                Code = "PRD-GEN-001",
                Name = "Jenerator",
                Description = "Mobil enerji destegi icin jeneratör",
                Unit = "adet",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new Product
            {
                Id = SeedIds.ProductFlashlight,
                Code = "PRD-FLS-001",
                Name = "El Feneri",
                Description = "Acil durum aydinlatma ekipmani",
                Unit = "adet",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<Warehouse>().HasData(
            new Warehouse
            {
                Id = SeedIds.WarehouseMain,
                Code = "WH-ESK-001",
                Name = "Ana Depo",
                Region = "Eskisehir",
                Address = "Eskisehir Merkez Operasyon Kampusu",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new Warehouse
            {
                Id = SeedIds.WarehouseAnkara,
                Code = "WH-ANK-001",
                Name = "Ankara Destek Deposu",
                Region = "Ankara",
                Address = "Ankara Sincan Lojistik Alani",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                Id = SeedIds.VehicleIstanbul,
                Code = "VEH-001",
                LicensePlate = "34 ABC 123",
                VehicleType = "Panelvan",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new Vehicle
            {
                Id = SeedIds.VehicleAnkara,
                Code = "VEH-002",
                LicensePlate = "06 XYZ 789",
                VehicleType = "Kamyonet",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new Vehicle
            {
                Id = SeedIds.VehicleEskisehir,
                Code = "VEH-003",
                LicensePlate = "26 ES 001",
                VehicleType = "SUV",
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<InventoryTask>().HasData(
            new InventoryTask
            {
                Id = SeedIds.TaskIzmirSupport,
                Name = "Izmir Saha Destek Gorevi",
                Description = "Izmir bolgesindeki aktif saha ekiplerine ekipman destegi",
                Region = "Izmir",
                StartDate = new DateOnly(2026, 4, 28),
                EndDate = null,
                Status = InventoryTaskStatus.InProgress,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTask
            {
                Id = SeedIds.TaskAnkaraEnergy,
                Name = "Ankara Acil Enerji Destegi",
                Description = "Enerji kesintisi yasayan noktalar icin mobil jeneratör destegi",
                Region = "Ankara",
                StartDate = new DateOnly(2026, 4, 20),
                EndDate = new DateOnly(2026, 4, 23),
                Status = InventoryTaskStatus.Completed,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTask
            {
                Id = SeedIds.TaskBursaPrep,
                Name = "Bursa Hazirlik Gorevi",
                Description = "Planlanan saha operasyonu icin hazirlik kaydi",
                Region = "Bursa",
                StartDate = new DateOnly(2026, 5, 2),
                EndDate = null,
                Status = InventoryTaskStatus.Draft,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<VehicleTask>().HasData(
            new VehicleTask
            {
                Id = SeedIds.VehicleTaskIstanbulIzmir,
                VehicleId = SeedIds.VehicleIstanbul,
                TaskId = SeedIds.TaskIzmirSupport,
                AssignedAtUtc = new DateTime(2026, 4, 28, 7, 30, 0, DateTimeKind.Utc),
                ReleasedAtUtc = null,
                AssignmentNote = "Izmir gorevi icin ilk arac atamasi",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new VehicleTask
            {
                Id = SeedIds.VehicleTaskAnkaraIzmir,
                VehicleId = SeedIds.VehicleAnkara,
                TaskId = SeedIds.TaskIzmirSupport,
                AssignedAtUtc = new DateTime(2026, 4, 28, 8, 0, 0, DateTimeKind.Utc),
                ReleasedAtUtc = null,
                AssignmentNote = "Telsiz destegi icin ikinci arac",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new VehicleTask
            {
                Id = SeedIds.VehicleTaskEskisehirAnkara,
                VehicleId = SeedIds.VehicleEskisehir,
                TaskId = SeedIds.TaskAnkaraEnergy,
                AssignedAtUtc = new DateTime(2026, 4, 20, 6, 45, 0, DateTimeKind.Utc),
                ReleasedAtUtc = new DateTime(2026, 4, 23, 18, 15, 0, DateTimeKind.Utc),
                AssignmentNote = "Tamamlanan enerji gorevi kaydi",
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<StockLocation>().HasData(
            new StockLocation
            {
                Id = SeedIds.StockLocationMainWarehouse,
                Name = "Ana Depo Stok Lokasyonu",
                LocationType = StockLocationType.Warehouse,
                WarehouseId = SeedIds.WarehouseMain,
                VehicleId = null,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockLocation
            {
                Id = SeedIds.StockLocationAnkaraWarehouse,
                Name = "Ankara Destek Deposu Stok Lokasyonu",
                LocationType = StockLocationType.Warehouse,
                WarehouseId = SeedIds.WarehouseAnkara,
                VehicleId = null,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockLocation
            {
                Id = SeedIds.StockLocationVehicleIstanbul,
                Name = "34 ABC 123 Arac Lokasyonu",
                LocationType = StockLocationType.Vehicle,
                WarehouseId = null,
                VehicleId = SeedIds.VehicleIstanbul,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockLocation
            {
                Id = SeedIds.StockLocationVehicleAnkara,
                Name = "06 XYZ 789 Arac Lokasyonu",
                LocationType = StockLocationType.Vehicle,
                WarehouseId = null,
                VehicleId = SeedIds.VehicleAnkara,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockLocation
            {
                Id = SeedIds.StockLocationVehicleEskisehir,
                Name = "26 ES 001 Arac Lokasyonu",
                LocationType = StockLocationType.Vehicle,
                WarehouseId = null,
                VehicleId = SeedIds.VehicleEskisehir,
                IsActive = true,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<StockBalance>().HasData(
            new StockBalance
            {
                Id = SeedIds.StockBalanceMainWarehouseRadio,
                ProductId = SeedIds.ProductRadio,
                StockLocationId = SeedIds.StockLocationMainWarehouse,
                Quantity = 10,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockBalance
            {
                Id = SeedIds.StockBalanceVehicleIstanbulRadio,
                ProductId = SeedIds.ProductRadio,
                StockLocationId = SeedIds.StockLocationVehicleIstanbul,
                Quantity = 5,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockBalance
            {
                Id = SeedIds.StockBalanceVehicleAnkaraRadio,
                ProductId = SeedIds.ProductRadio,
                StockLocationId = SeedIds.StockLocationVehicleAnkara,
                Quantity = 5,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockBalance
            {
                Id = SeedIds.StockBalanceAnkaraWarehouseGenerator,
                ProductId = SeedIds.ProductGenerator,
                StockLocationId = SeedIds.StockLocationAnkaraWarehouse,
                Quantity = 2,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockBalance
            {
                Id = SeedIds.StockBalanceVehicleIstanbulGenerator,
                ProductId = SeedIds.ProductGenerator,
                StockLocationId = SeedIds.StockLocationVehicleIstanbul,
                Quantity = 1,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockBalance
            {
                Id = SeedIds.StockBalanceMainWarehouseFlashlight,
                ProductId = SeedIds.ProductFlashlight,
                StockLocationId = SeedIds.StockLocationMainWarehouse,
                Quantity = 12,
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new StockBalance
            {
                Id = SeedIds.StockBalanceVehicleEskisehirFlashlight,
                ProductId = SeedIds.ProductFlashlight,
                StockLocationId = SeedIds.StockLocationVehicleEskisehir,
                Quantity = 4,
                CreatedAtUtc = SeedCreatedAtUtc
            });

        modelBuilder.Entity<InventoryTransaction>().HasData(
            new InventoryTransaction
            {
                Id = SeedIds.TransactionInitialRadioLoad,
                ProductId = SeedIds.ProductRadio,
                SourceStockLocationId = null,
                DestinationStockLocationId = SeedIds.StockLocationMainWarehouse,
                TaskId = null,
                TransactionType = InventoryTransactionType.InitialLoad,
                Quantity = 20,
                PerformedAtUtc = new DateTime(2026, 4, 27, 9, 0, 0, DateTimeKind.Utc),
                ReferenceNote = "Telsiz ilk depo girisi",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTransaction
            {
                Id = SeedIds.TransactionRadioToVehicleIstanbul,
                ProductId = SeedIds.ProductRadio,
                SourceStockLocationId = SeedIds.StockLocationMainWarehouse,
                DestinationStockLocationId = SeedIds.StockLocationVehicleIstanbul,
                TaskId = SeedIds.TaskIzmirSupport,
                TransactionType = InventoryTransactionType.WarehouseToVehicle,
                Quantity = 5,
                PerformedAtUtc = new DateTime(2026, 4, 28, 8, 15, 0, DateTimeKind.Utc),
                ReferenceNote = "Izmir gorevi icin araca yukleme",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTransaction
            {
                Id = SeedIds.TransactionRadioToVehicleAnkara,
                ProductId = SeedIds.ProductRadio,
                SourceStockLocationId = SeedIds.StockLocationMainWarehouse,
                DestinationStockLocationId = SeedIds.StockLocationVehicleAnkara,
                TaskId = SeedIds.TaskIzmirSupport,
                TransactionType = InventoryTransactionType.WarehouseToVehicle,
                Quantity = 5,
                PerformedAtUtc = new DateTime(2026, 4, 28, 8, 45, 0, DateTimeKind.Utc),
                ReferenceNote = "Izmir gorevi icin ikinci arac yuklemesi",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTransaction
            {
                Id = SeedIds.TransactionInitialGeneratorLoad,
                ProductId = SeedIds.ProductGenerator,
                SourceStockLocationId = null,
                DestinationStockLocationId = SeedIds.StockLocationAnkaraWarehouse,
                TaskId = null,
                TransactionType = InventoryTransactionType.InitialLoad,
                Quantity = 3,
                PerformedAtUtc = new DateTime(2026, 4, 19, 14, 0, 0, DateTimeKind.Utc),
                ReferenceNote = "Jenerator depo girisi",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTransaction
            {
                Id = SeedIds.TransactionGeneratorToVehicleIstanbul,
                ProductId = SeedIds.ProductGenerator,
                SourceStockLocationId = SeedIds.StockLocationAnkaraWarehouse,
                DestinationStockLocationId = SeedIds.StockLocationVehicleIstanbul,
                TaskId = SeedIds.TaskIzmirSupport,
                TransactionType = InventoryTransactionType.WarehouseToVehicle,
                Quantity = 1,
                PerformedAtUtc = new DateTime(2026, 4, 28, 9, 5, 0, DateTimeKind.Utc),
                ReferenceNote = "Izmir gorevi icin jenerator transferi",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTransaction
            {
                Id = SeedIds.TransactionInitialFlashlightLoad,
                ProductId = SeedIds.ProductFlashlight,
                SourceStockLocationId = null,
                DestinationStockLocationId = SeedIds.StockLocationMainWarehouse,
                TaskId = null,
                TransactionType = InventoryTransactionType.InitialLoad,
                Quantity = 16,
                PerformedAtUtc = new DateTime(2026, 4, 18, 11, 0, 0, DateTimeKind.Utc),
                ReferenceNote = "El feneri depo girisi",
                CreatedAtUtc = SeedCreatedAtUtc
            },
            new InventoryTransaction
            {
                Id = SeedIds.TransactionFlashlightToVehicleEskisehir,
                ProductId = SeedIds.ProductFlashlight,
                SourceStockLocationId = SeedIds.StockLocationMainWarehouse,
                DestinationStockLocationId = SeedIds.StockLocationVehicleEskisehir,
                TaskId = SeedIds.TaskAnkaraEnergy,
                TransactionType = InventoryTransactionType.WarehouseToVehicle,
                Quantity = 4,
                PerformedAtUtc = new DateTime(2026, 4, 20, 7, 5, 0, DateTimeKind.Utc),
                ReferenceNote = "Ankara gorevi icin el feneri sevkiyati",
                CreatedAtUtc = SeedCreatedAtUtc
            });
    }

    private static AppPermission CreatePermission(string code, string name, string description) =>
        new()
        {
            Id = GetPermissionId(code),
            Code = code,
            Name = name,
            Description = description,
            IsActive = true,
            CreatedAtUtc = SeedCreatedAtUtc
        };

    private static AppUserRole CreateUserRole(Guid userId, Guid roleId) =>
        new()
        {
            Id = CreateDeterministicGuid($"user-role:{userId}:{roleId}"),
            UserId = userId,
            RoleId = roleId,
            CreatedAtUtc = SeedCreatedAtUtc
        };

    private static AppRolePermission CreateRolePermission(Guid roleId, Guid permissionId) =>
        new()
        {
            Id = CreateDeterministicGuid($"role-permission:{roleId}:{permissionId}"),
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAtUtc = SeedCreatedAtUtc
        };

    private static Guid GetPermissionId(string permissionCode) =>
        CreateDeterministicGuid($"permission:{permissionCode}");

    private static Guid CreateDeterministicGuid(string value)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(value));
        return new Guid(bytes);
    }
}
