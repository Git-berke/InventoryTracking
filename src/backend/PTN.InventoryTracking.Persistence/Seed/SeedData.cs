using Microsoft.EntityFrameworkCore;
using PTN.InventoryTracking.Domain.Entities;
using PTN.InventoryTracking.Domain.Enums;

namespace PTN.InventoryTracking.Persistence.Seed;

public static class SeedData
{
    private static readonly DateTime SeedCreatedAtUtc = new(2026, 4, 29, 7, 0, 0, DateTimeKind.Utc);

    public static void Apply(ModelBuilder modelBuilder)
    {
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
}
