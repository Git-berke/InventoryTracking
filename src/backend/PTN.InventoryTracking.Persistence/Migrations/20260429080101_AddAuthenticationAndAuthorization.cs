using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PTN.InventoryTracking.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationAndAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "app_role_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_role_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_app_role_permissions_app_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "app_permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_app_role_permissions_app_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "app_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "app_user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_app_user_roles_app_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "app_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_app_user_roles_app_users_user_id",
                        column: x => x.user_id,
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "app_permissions",
                columns: new[] { "id", "code", "created_at_utc", "description", "is_active", "name", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("056d3dde-7c9b-4f80-2bef-448ec032cc47"), "products.update", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Update product records", true, "Products Update", null },
                    { new Guid("06c66756-de69-c309-5904-1e03f5cb802f"), "warehouses.delete", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Delete warehouse records", true, "Warehouses Delete", null },
                    { new Guid("1905b4b2-0ee4-c765-90be-7d0f61c1a31e"), "stock-transfers.create", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Perform stock transfer operations", true, "Stock Transfers Create", null },
                    { new Guid("44cbfd49-9071-bbe9-cbe1-b40f9915db7f"), "vehicles.delete", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Delete vehicle records", true, "Vehicles Delete", null },
                    { new Guid("5340d184-3527-50e2-a178-7ccc44e9fb4c"), "tasks.update", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Update task records", true, "Tasks Update", null },
                    { new Guid("5c1bfe4d-52d9-909c-3527-9b921ecaf1f0"), "products.delete", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Delete product records", true, "Products Delete", null },
                    { new Guid("5d3e17a6-ee56-63d8-cf19-69f31904c9e5"), "vehicles.create", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Create vehicle records", true, "Vehicles Create", null },
                    { new Guid("627e24d4-7e9b-df99-ff41-417c379f2e7f"), "warehouses.read", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Read warehouse records", true, "Warehouses Read", null },
                    { new Guid("6d123192-989d-d078-c886-2afe2dac17d9"), "vehicles.read", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Read vehicle and vehicle inventory endpoints", true, "Vehicles Read", null },
                    { new Guid("709622e0-c98c-a0e1-3d14-cd8a7df9140f"), "tasks.delete", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Delete task records", true, "Tasks Delete", null },
                    { new Guid("826a169d-c216-c921-de5c-537103eb0ac8"), "tasks.read", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Read task, task vehicle and task inventory endpoints", true, "Tasks Read", null },
                    { new Guid("b0dd0060-ecc3-d5f9-85ff-dfa878fa3b7c"), "warehouses.update", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Update warehouse records", true, "Warehouses Update", null },
                    { new Guid("bb167ee6-8c7a-e6b8-7e81-c06fc2e8fb62"), "warehouses.create", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Create warehouse records", true, "Warehouses Create", null },
                    { new Guid("e1d455fb-78d0-80ff-11e2-939128d9dfe8"), "vehicles.update", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Update vehicle records", true, "Vehicles Update", null },
                    { new Guid("f6844f62-5b8f-415a-6297-e885cdc667da"), "products.read", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Read product and stock summary endpoints", true, "Products Read", null },
                    { new Guid("f83c6850-a850-3ba5-81d9-a8a04bcd18d2"), "tasks.create", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Create task records", true, "Tasks Create", null },
                    { new Guid("fbbb5c2d-039a-8762-baab-4bd664154506"), "inventory-transactions.read", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Read inventory transaction history", true, "Inventory Transactions Read", null },
                    { new Guid("febe00cf-cd8d-f7f0-fbae-9f9b2015c29c"), "products.create", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Create product records", true, "Products Create", null }
                });

            migrationBuilder.InsertData(
                table: "app_roles",
                columns: new[] { "id", "code", "created_at_utc", "description", "is_active", "name", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("10101010-1010-1010-1010-101010101011"), "ROLE-ADMIN", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Full access role", true, "Admin", null },
                    { new Guid("10101010-1010-1010-1010-101010101012"), "ROLE-WAREHOUSE-OPERATOR", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Warehouse and stock transfer operator role", true, "WarehouseOperator", null },
                    { new Guid("10101010-1010-1010-1010-101010101013"), "ROLE-TASK-MANAGER", new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "Task and vehicle management role", true, "TaskManager", null }
                });

            migrationBuilder.InsertData(
                table: "app_users",
                columns: new[] { "id", "created_at_utc", "email", "full_name", "is_active", "password_hash", "updated_at_utc", "user_name" },
                values: new object[,]
                {
                    { new Guid("20202020-2020-2020-2020-202020202021"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "admin@ptn.local", "System Administrator", true, "pbkdf2$100000$adQyMFF/MEDyqS1mZ08ATA==$L4rcPrvAUy/+bShxsmO+6YvNZ4t+5hOBVnjdoROjk68=", null, "admin" },
                    { new Guid("20202020-2020-2020-2020-202020202022"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "warehouse@ptn.local", "Warehouse Operator", true, "pbkdf2$100000$fmXCFcONlxHDNILtMYT/tg==$l5v3quqi3kNjhqqcLC7jNEBW2RW0LNmHxwY2xAgI7Gs=", null, "warehouse.operator" },
                    { new Guid("20202020-2020-2020-2020-202020202023"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), "taskmanager@ptn.local", "Task Manager", true, "pbkdf2$100000$mNGvVDSa8GdorMSan+8s/A==$Ht44IbT0Q6iij1kUu47u+DZ3IKTJTTCJdhk/n7fiZro=", null, "task.manager" }
                });

            migrationBuilder.InsertData(
                table: "app_role_permissions",
                columns: new[] { "id", "created_at_utc", "permission_id", "role_id", "updated_at_utc" },
                values: new object[,]
                {
                    { new Guid("069a136f-33c5-34c2-25e0-7a8239d2292a"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("5c1bfe4d-52d9-909c-3527-9b921ecaf1f0"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("08460690-6de7-7b35-6f89-88be26257faa"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("6d123192-989d-d078-c886-2afe2dac17d9"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("108823c6-69c6-0454-60bf-5a859d8892fe"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("febe00cf-cd8d-f7f0-fbae-9f9b2015c29c"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("127ae5a4-49f8-ce16-c6e5-f82c84ff067c"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("627e24d4-7e9b-df99-ff41-417c379f2e7f"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("195ad4ea-d0c1-0c05-c4f4-4390335ebfc1"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("fbbb5c2d-039a-8762-baab-4bd664154506"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("2cf3ee7f-8b2c-c17d-b087-3c6d7af72af6"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("e1d455fb-78d0-80ff-11e2-939128d9dfe8"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("2eb1c131-dd8d-13d6-20c0-e53505695f48"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("709622e0-c98c-a0e1-3d14-cd8a7df9140f"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("301988b3-0ca8-8cfb-57de-641928ef4f24"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("fbbb5c2d-039a-8762-baab-4bd664154506"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("3531b09c-614a-5439-c96c-2049fac385e2"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("f6844f62-5b8f-415a-6297-e885cdc667da"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("44baeb4e-5729-cad9-a343-73886c0bcc17"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("e1d455fb-78d0-80ff-11e2-939128d9dfe8"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("4dc22b53-7691-e86e-b9cf-54e8bf9a5e70"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("f6844f62-5b8f-415a-6297-e885cdc667da"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("56677704-ae28-4b27-b468-8cb0a98c2da5"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("826a169d-c216-c921-de5c-537103eb0ac8"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("6c982b3f-1881-855d-4a10-0bdb9012b155"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("b0dd0060-ecc3-d5f9-85ff-dfa878fa3b7c"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("7ad826b8-66c1-defc-eef7-a8e04335b91b"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("f83c6850-a850-3ba5-81d9-a8a04bcd18d2"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("7d463a9d-e7c8-ae0a-6216-1ae08b39add8"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("826a169d-c216-c921-de5c-537103eb0ac8"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("7da1ba1e-f1d6-d931-1a45-a5786eb41944"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("6d123192-989d-d078-c886-2afe2dac17d9"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("8a3526a3-6db6-4456-c946-842b155809be"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("056d3dde-7c9b-4f80-2bef-448ec032cc47"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("938ce833-fe87-c5b6-435a-7db155ac5aba"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("6d123192-989d-d078-c886-2afe2dac17d9"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("93a24b5a-0322-c6dd-74ee-aec1b6408b73"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("1905b4b2-0ee4-c765-90be-7d0f61c1a31e"), new Guid("10101010-1010-1010-1010-101010101012"), null },
                    { new Guid("a6d08d84-b0bf-856f-8c01-1a3be9adcd32"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("b0dd0060-ecc3-d5f9-85ff-dfa878fa3b7c"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("bf18a65c-871b-6044-0a4e-30016137ef93"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("fbbb5c2d-039a-8762-baab-4bd664154506"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("c18534c7-a8a8-105d-e513-48609fcc4114"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("5340d184-3527-50e2-a178-7ccc44e9fb4c"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("c9a9e48a-0cba-c624-2c3c-9323421939c2"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("44cbfd49-9071-bbe9-cbe1-b40f9915db7f"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("cd2f5c7d-ae8c-4361-2d1e-ee682fd1b11d"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("826a169d-c216-c921-de5c-537103eb0ac8"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("d40c3516-48a4-8ee2-8ebf-4721e03fce31"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("bb167ee6-8c7a-e6b8-7e81-c06fc2e8fb62"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("d4d16e85-3add-013f-779d-92d013126ac2"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("627e24d4-7e9b-df99-ff41-417c379f2e7f"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("db060b28-0443-d45e-01a7-88e00449689f"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("5d3e17a6-ee56-63d8-cf19-69f31904c9e5"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("e128286c-298e-b23d-9438-2aa4816b3951"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("5340d184-3527-50e2-a178-7ccc44e9fb4c"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("ede9ec13-5fd3-ba9c-a904-17008a9e6365"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("f6844f62-5b8f-415a-6297-e885cdc667da"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("f1f901e0-6410-2cdf-f884-458adead476d"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("f83c6850-a850-3ba5-81d9-a8a04bcd18d2"), new Guid("10101010-1010-1010-1010-101010101013"), null },
                    { new Guid("f361505c-eac2-247a-5fae-61ce7079ca9e"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("1905b4b2-0ee4-c765-90be-7d0f61c1a31e"), new Guid("10101010-1010-1010-1010-101010101011"), null },
                    { new Guid("f87bd99a-679f-a322-f404-215a90408228"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("06c66756-de69-c309-5904-1e03f5cb802f"), new Guid("10101010-1010-1010-1010-101010101011"), null }
                });

            migrationBuilder.InsertData(
                table: "app_user_roles",
                columns: new[] { "id", "created_at_utc", "role_id", "updated_at_utc", "user_id" },
                values: new object[,]
                {
                    { new Guid("474a135a-6bcc-9198-89b6-ffdd370c199f"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("10101010-1010-1010-1010-101010101011"), null, new Guid("20202020-2020-2020-2020-202020202021") },
                    { new Guid("83cf1153-daf6-ab65-9e0e-9b2542cdfce9"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("10101010-1010-1010-1010-101010101012"), null, new Guid("20202020-2020-2020-2020-202020202022") },
                    { new Guid("99ec6872-8ad3-50d3-06b3-90356645b868"), new DateTime(2026, 4, 29, 7, 0, 0, 0, DateTimeKind.Utc), new Guid("10101010-1010-1010-1010-101010101013"), null, new Guid("20202020-2020-2020-2020-202020202023") }
                });

            migrationBuilder.CreateIndex(
                name: "ix_app_permissions_code",
                table: "app_permissions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_permissions_name",
                table: "app_permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_role_permissions_permission_id",
                table: "app_role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_app_role_permissions_role_id_permission_id",
                table: "app_role_permissions",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_roles_code",
                table: "app_roles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_roles_name",
                table: "app_roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_user_roles_role_id",
                table: "app_user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_app_user_roles_user_id_role_id",
                table: "app_user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_email",
                table: "app_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_user_name",
                table: "app_users",
                column: "user_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_role_permissions");

            migrationBuilder.DropTable(
                name: "app_user_roles");

            migrationBuilder.DropTable(
                name: "app_permissions");

            migrationBuilder.DropTable(
                name: "app_roles");

            migrationBuilder.DropTable(
                name: "app_users");
        }
    }
}
