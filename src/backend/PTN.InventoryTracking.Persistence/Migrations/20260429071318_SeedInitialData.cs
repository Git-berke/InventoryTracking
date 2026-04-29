using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PTN.InventoryTracking.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "code", "created_at_utc", "description", "is_active", "name", "unit", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "PRD-RADIO-001", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Saha ekipleri icin el telsizi", true, "Telsiz", "adet", null },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "PRD-GEN-001", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Mobil enerji destegi icin jeneratör", true, "Jenerator", "adet", null },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "PRD-FLS-001", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Acil durum aydinlatma ekipmani", true, "El Feneri", "adet", null }
                });

            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "id", "created_at_utc", "description", "end_date", "name", "region", "start_date", "status", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("44444444-4444-4444-4444-444444444441"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Izmir bolgesindeki aktif saha ekiplerine ekipman destegi", null, "Izmir Saha Destek Gorevi", "Izmir", new DateOnly(2026, 4, 28), 2, null },
                    { new Guid("44444444-4444-4444-4444-444444444442"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Enerji kesintisi yasayan noktalar icin mobil jeneratör destegi", new DateOnly(2026, 4, 23), "Ankara Acil Enerji Destegi", "Ankara", new DateOnly(2026, 4, 20), 3, null },
                    { new Guid("44444444-4444-4444-4444-444444444443"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Planlanan saha operasyonu icin hazirlik kaydi", null, "Bursa Hazirlik Gorevi", "Bursa", new DateOnly(2026, 5, 2), 1, null }
                });

            migrationBuilder.InsertData(
                table: "vehicles",
                columns: new[] { "id", "code", "created_at_utc", "is_active", "license_plate", "updated_at_utc", "vehicle_type" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333331"), "VEH-001", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, "34 ABC 123", null, "Panelvan" },
                    { new Guid("33333333-3333-3333-3333-333333333332"), "VEH-002", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, "06 XYZ 789", null, "Kamyonet" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "VEH-003", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, "26 ES 001", null, "SUV" }
                });

            migrationBuilder.InsertData(
                table: "warehouses",
                columns: new[] { "id", "address", "code", "created_at_utc", "is_active", "name", "region", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222221"), "Eskisehir Merkez Operasyon Kampusu", "WH-ESK-001", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, "Ana Depo", "Eskisehir", null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Ankara Sincan Lojistik Alani", "WH-ANK-001", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, "Ankara Destek Deposu", "Ankara", null }
                });

            migrationBuilder.InsertData(
                table: "stock_locations",
                columns: new[] { "id", "created_at_utc", "is_active", "location_type", "name", "updated_at_utc", "vehicle_id", "warehouse_id" },
                values: new object[,]
                {
                    { new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, 1, "Ana Depo Stok Lokasyonu", null, null, new Guid("22222222-2222-2222-2222-222222222221") },
                    { new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, 1, "Ankara Destek Deposu Stok Lokasyonu", null, null, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("66666666-6666-6666-6666-666666666663"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, 2, "34 ABC 123 Arac Lokasyonu", null, new Guid("33333333-3333-3333-3333-333333333331"), null },
                    { new Guid("66666666-6666-6666-6666-666666666664"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, 2, "06 XYZ 789 Arac Lokasyonu", null, new Guid("33333333-3333-3333-3333-333333333332"), null },
                    { new Guid("66666666-6666-6666-6666-666666666665"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), true, 2, "26 ES 001 Arac Lokasyonu", null, new Guid("33333333-3333-3333-3333-333333333333"), null }
                });

            migrationBuilder.InsertData(
                table: "vehicle_tasks",
                columns: new[] { "id", "assigned_at_utc", "assignment_note", "created_at_utc", "released_at_utc", "task_id", "updated_at_utc", "vehicle_id" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555551"), new DateTime(2026, 4, 28, 7, 30, 0, 0, DateTimeKind.Utc), "Izmir gorevi icin ilk arac atamasi", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), null, new Guid("44444444-4444-4444-4444-444444444441"), null, new Guid("33333333-3333-3333-3333-333333333331") },
                    { new Guid("55555555-5555-5555-5555-555555555552"), new DateTime(2026, 4, 28, 8, 0, 0, 0, DateTimeKind.Utc), "Telsiz destegi icin ikinci arac", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), null, new Guid("44444444-4444-4444-4444-444444444441"), null, new Guid("33333333-3333-3333-3333-333333333332") },
                    { new Guid("55555555-5555-5555-5555-555555555553"), new DateTime(2026, 4, 20, 6, 45, 0, 0, DateTimeKind.Utc), "Tamamlanan enerji gorevi kaydi", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 23, 18, 15, 0, 0, DateTimeKind.Utc), new Guid("44444444-4444-4444-4444-444444444442"), null, new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "inventory_transactions",
                columns: new[] { "id", "created_at_utc", "destination_stock_location_id", "performed_at_utc", "product_id", "quantity", "reference_note", "source_stock_location_id", "task_id", "transaction_type", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("88888888-8888-8888-8888-888888888881"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 4, 27, 9, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 20, "Telsiz ilk depo girisi", null, null, 1, null },
                    { new Guid("88888888-8888-8888-8888-888888888882"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666663"), new DateTime(2026, 4, 28, 8, 15, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 5, "Izmir gorevi icin araca yukleme", new Guid("66666666-6666-6666-6666-666666666661"), new Guid("44444444-4444-4444-4444-444444444441"), 2, null },
                    { new Guid("88888888-8888-8888-8888-888888888883"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666664"), new DateTime(2026, 4, 28, 8, 45, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 5, "Izmir gorevi icin ikinci arac yuklemesi", new Guid("66666666-6666-6666-6666-666666666661"), new Guid("44444444-4444-4444-4444-444444444441"), 2, null },
                    { new Guid("88888888-8888-8888-8888-888888888884"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666662"), new DateTime(2026, 4, 19, 14, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111112"), 3, "Jenerator depo girisi", null, null, 1, null },
                    { new Guid("88888888-8888-8888-8888-888888888885"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666663"), new DateTime(2026, 4, 28, 9, 5, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111112"), 1, "Izmir gorevi icin jenerator transferi", new Guid("66666666-6666-6666-6666-666666666662"), new Guid("44444444-4444-4444-4444-444444444441"), 2, null },
                    { new Guid("88888888-8888-8888-8888-888888888886"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666661"), new DateTime(2026, 4, 18, 11, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111113"), 16, "El feneri depo girisi", null, null, 1, null },
                    { new Guid("88888888-8888-8888-8888-888888888887"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("66666666-6666-6666-6666-666666666665"), new DateTime(2026, 4, 20, 7, 5, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111113"), 4, "Ankara gorevi icin el feneri sevkiyati", new Guid("66666666-6666-6666-6666-666666666661"), new Guid("44444444-4444-4444-4444-444444444442"), 2, null }
                });

            migrationBuilder.InsertData(
                table: "stock_balances",
                columns: new[] { "id", "created_at_utc", "product_id", "quantity", "stock_location_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777771"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 10, new Guid("66666666-6666-6666-6666-666666666661"), null },
                    { new Guid("77777777-7777-7777-7777-777777777772"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 5, new Guid("66666666-6666-6666-6666-666666666663"), null },
                    { new Guid("77777777-7777-7777-7777-777777777773"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111"), 5, new Guid("66666666-6666-6666-6666-666666666664"), null },
                    { new Guid("77777777-7777-7777-7777-777777777774"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111112"), 2, new Guid("66666666-6666-6666-6666-666666666662"), null },
                    { new Guid("77777777-7777-7777-7777-777777777775"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111112"), 1, new Guid("66666666-6666-6666-6666-666666666663"), null },
                    { new Guid("77777777-7777-7777-7777-777777777776"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111113"), 12, new Guid("66666666-6666-6666-6666-666666666661"), null },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111113"), 4, new Guid("66666666-6666-6666-6666-666666666665"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888881"));

            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888882"));

            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888883"));

            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888884"));

            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888885"));

            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888886"));

            migrationBuilder.DeleteData(
                table: "inventory_transactions",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888887"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777773"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777774"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777775"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777776"));

            migrationBuilder.DeleteData(
                table: "stock_balances",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444443"));

            migrationBuilder.DeleteData(
                table: "vehicle_tasks",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"));

            migrationBuilder.DeleteData(
                table: "vehicle_tasks",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"));

            migrationBuilder.DeleteData(
                table: "vehicle_tasks",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555553"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "products",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "stock_locations",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666661"));

            migrationBuilder.DeleteData(
                table: "stock_locations",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666662"));

            migrationBuilder.DeleteData(
                table: "stock_locations",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666663"));

            migrationBuilder.DeleteData(
                table: "stock_locations",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666664"));

            migrationBuilder.DeleteData(
                table: "stock_locations",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666665"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"));

            migrationBuilder.DeleteData(
                table: "tasks",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"));

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333331"));

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333332"));

            migrationBuilder.DeleteData(
                table: "vehicles",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "warehouses",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "warehouses",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
