using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.Migrations
{
    /// <inheritdoc />
    public partial class AddComponentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Specs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComponentTypeId = table.Column<int>(type: "int", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PCBuildItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PCBuildId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    ComponentTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PCBuildItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PCBuildItems_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PCBuildItems_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PCBuildItems_PCBuilds_PCBuildId",
                        column: x => x.PCBuildId,
                        principalTable: "PCBuilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25,
                column: "Description",
                value: "16 nhân (8P+8E)/24 luồng, Boost 5.4GHz, L3 30MB, TDP 125W, LGA1700");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26,
                column: "Description",
                value: "6 nhân/12 luồng, Boost 5.3GHz, L3 32MB, TDP 105W, PCIe 5.0, AM5");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "DLSS 3, Ray Tracing, 1440p/4K gaming", "https://images.unsplash.com/photo-1580927752452-89d86da3fa0a?w=400&q=80", "VGA NVIDIA RTX 4070 12GB" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "LGA1700, DDR5, PCIe 5.0, WiFi 6E", "https://images.unsplash.com/photo-1520659649-1b4f85f5cfc0?w=400&q=80", "Mainboard MSI B760M Mortar WiFi" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Modular, ATX 3.0, 10y warranty", "https://images.unsplash.com/photo-1560732488-6b0df240254a?w=400&q=80", "PSU Corsair RM850x 850W 80+ Gold" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 31, 5, "14 nhân (6P+8E)/20 luồng, Boost 5.1GHz, LGA1700, DDR5", "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=400&q=80", "CPU Intel Core i5-13600K", 7290000m },
                    { 32, 5, "8 nhân/16 luồng, Boost 5.4GHz, AM5, DDR5, PCIe 5.0", "https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400&q=80", "CPU AMD Ryzen 7 7700X", 8990000m },
                    { 33, 5, "4 nhân/8 luồng, Budget gaming/office, LGA1700", "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&q=80", "CPU Intel Core i3-13100F", 2790000m },
                    { 34, 5, "16GB (2x8GB) CL16, RGB optional, Intel/AMD", "https://images.unsplash.com/photo-1541029071515-84cc54f84dc5?w=400&q=80", "RAM DDR4 16GB Corsair Vengeance 3200MHz", 1590000m },
                    { 35, 5, "32GB (2x16GB) CL36, RGB, XMP 3.0", "https://images.unsplash.com/photo-1611186871525-b6a9b7b3a4f4?w=400&q=80", "RAM DDR5 32GB G.Skill Trident Z5 6000MHz", 3490000m },
                    { 36, 5, "Budget 8GB single stick, CL16", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400&q=80", "RAM DDR4 8GB Kingston HyperX Fury 3200MHz", 890000m },
                    { 37, 5, "1080p/1440p 144Hz, DLSS 3", "https://images.unsplash.com/photo-1547082299-de196ea013d6?w=400&q=80", "VGA NVIDIA RTX 4060 Ti 8GB", 11990000m },
                    { 38, 5, "FSR 3, 1080p/1440p high FPS", "https://images.unsplash.com/photo-1587202372634-32705e3bf49c?w=400&q=80", "VGA AMD RX 7600 8GB", 8990000m },
                    { 39, 5, "AM5, DDR5, PCIe 5.0, 3x M.2", "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400&q=80", "Mainboard ASUS PRIME B650-PLUS", 4490000m },
                    { 40, 5, "AM4, DDR4, PCIe 4.0, budget gaming", "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?w=400&q=80", "Mainboard Gigabyte B550 AORUS Elite", 3290000m },
                    { 41, 5, "Fully modular, 10y warranty", "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&q=80", "PSU Seasonic Focus GX-650 650W 80+ Gold", 2390000m },
                    { 42, 5, "Budget 550W, non-modular", "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&q=80", "PSU Cooler Master MWE 550 Bronze V2", 1290000m },
                    { 43, 5, "M-ATX, mesh front, RGB fans, tempered glass", "https://images.unsplash.com/photo-1586210579191-33b45e38fa2c?w=400&q=80", "Case NZXT H5 Flow RGB", 2490000m },
                    { 44, 5, "ATX, high airflow, front I/O USB-C", "https://images.unsplash.com/photo-1551645120-d70bfe84c826?w=400&q=80", "Case Corsair 4000D Airflow", 2790000m },
                    { 45, 5, "M-ATX, mesh design, compact", "https://images.unsplash.com/photo-1616763355548-1b606f439f86?w=400&q=80", "Case Fractal Design Meshify C", 3090000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Components_ComponentTypeId",
                table: "Components",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuildItems_ComponentId",
                table: "PCBuildItems",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuildItems_ComponentTypeId",
                table: "PCBuildItems",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PCBuildItems_PCBuildId",
                table: "PCBuildItems",
                column: "PCBuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PCBuildItems");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "ComponentTypes");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25,
                column: "Description",
                value: "16 nhân (8P+8E) / 24 luồng, Boost 5.4GHz, L3 30MB, TDP 125W, socket LGA1700");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26,
                column: "Description",
                value: "6 nhân / 12 luồng, Boost 5.3GHz, L3 32MB, TDP 105W, PCIe 5.0, AM5");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "12GB GDDR6X, 192-bit, DLSS 3, Ray Tracing, 2505MHz Boost, 2x HDMI 2.1, 3x DP", "https://images.unsplash.com/photo-1587202372775-e229f172b9d7?w=400&q=80", "VGA ASUS GeForce RTX 4070 12GB DUAL" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Intel B760, LGA1700, DDR5, PCIe 4.0, WiFi 6E, BT 5.3, 2x M.2, mATX", "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400&q=80", "Mainboard MSI MAG B760M Mortar WiFi" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "850W, 80+ Gold, Full Modular, 10 năm bảo hành, Zero RPM fan, ATX 3.0", "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?w=400&q=80", "Nguồn Corsair RM850x 850W 80+ Gold" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[] { 30, 5, "Radiator 240mm, 2x fan 120mm PWM, LCD màn hình hiển thị, ARGB, socket AM5/1700", "https://images.unsplash.com/photo-1560732488-6b0df240254a?w=400&q=80", "Tản nhiệt nước AIO NZXT Kraken 240", 2690000m });
        }
    }
}
