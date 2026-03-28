using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TechStore.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiscountPercent = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WishlistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishlistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishlistItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WishlistItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Máy tính xách tay", "Laptop" },
                    { 2, "Máy tính để bàn", "PC / Desktop" },
                    { 3, "Monitor các loại", "Màn hình" },
                    { 4, "Bàn phím, chuột, tai nghe", "Phụ kiện" },
                    { 5, "CPU, RAM, SSD, VGA", "Linh kiện" }
                });

            migrationBuilder.InsertData(
                table: "Vouchers",
                columns: new[] { "Id", "Code", "DiscountPercent", "ExpiryDate", "IsActive", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { 1, "TECHSTORE10", 10, new DateTime(2027, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 1000, 0 },
                    { 2, "SALE20", 20, new DateTime(2027, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 100, 0 },
                    { 3, "NEWUSER15", 15, new DateTime(2027, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 500, 0 },
                    { 4, "GAMING30", 30, new DateTime(2027, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 50, 0 },
                    { 5, "LINH5", 5, new DateTime(2027, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 999, 0 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Core i5-1235U, RAM 8GB DDR4, SSD 512GB NVMe, 15.6\" FHD IPS, Iris Xe, Win 11", "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=400&q=80", "Laptop ASUS VivoBook 15 X1504ZA", 14990000m },
                    { 2, 1, "Core i5-1235U, RAM 16GB DDR4, SSD 512GB, 15.6\" FHD, Win 11 Home", "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&q=80", "Laptop Dell Inspiron 15 3520", 16490000m },
                    { 3, 1, "Core i7-1355U, RAM 16GB, SSD 512GB, GeForce RTX 2050 4GB, 15.6\" FHD IPS", "https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400&q=80", "Laptop HP Pavilion 15-eg3097TX", 18990000m },
                    { 4, 1, "Ryzen 7 6800H, RAM 16GB, SSD 512GB, Radeon RX 6600M, 16\" 2.5K 120Hz", "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&q=80", "Laptop Lenovo IdeaPad 5 Pro 16ARH7", 21490000m },
                    { 5, 1, "Apple M2 8-core, RAM 8GB Unified, SSD 256GB, 13.6\" Liquid Retina, macOS", "https://images.unsplash.com/photo-1611186871525-b6a9b7b3a4f4?w=400&q=80", "Laptop MacBook Air M2 13\"", 27990000m },
                    { 6, 1, "Core i9-14900HX, RTX 4070 8GB, RAM 32GB DDR5, SSD 1TB, 16\" QHD 240Hz", "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400&q=80", "Laptop ASUS ROG Strix G16 2024", 42990000m },
                    { 7, 1, "Core i5-12500H, RTX 3050 4GB, RAM 8GB, SSD 512GB, 15.6\" FHD 144Hz", "https://images.unsplash.com/photo-1580927752452-89d86da3fa0a?w=400&q=80", "Laptop Acer Nitro 5 AN515-58", 19990000m },
                    { 8, 1, "Core i7-12650H, RTX 4050 6GB, RAM 16GB, SSD 512GB, 15.6\" FHD 144Hz", "https://images.unsplash.com/photo-1547082299-de196ea013d6?w=400&q=80", "Laptop MSI Thin GF63 12VE", 22490000m },
                    { 9, 2, "Core i7-13700F, RTX 4060 8GB, RAM 16GB DDR5, SSD 1TB NVMe, Case ATX Full RGB", "https://images.unsplash.com/photo-1587202372634-32705e3bf49c?w=400&q=80", "PC Gaming RGB i7 RTX 4060", 28900000m },
                    { 10, 2, "Ryzen 9 7900X, RTX 4070 Ti 12GB, RAM 32GB DDR5, SSD 2TB, Cooler Noctua NH-D15", "https://images.unsplash.com/photo-1520659649-1b4f85f5cfc0?w=400&q=80", "PC Workstation Ryzen 9 Pro", 38500000m },
                    { 11, 2, "Core i5-1340P, RAM 16GB LPDDR5, SSD 512GB, Thunderbolt 4, WiFi 6E, BT 5.3", "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400&q=80", "Mini PC Intel NUC 13 Pro", 11990000m },
                    { 12, 2, "Core i3-12100, RAM 8GB DDR4, SSD 256GB, Case mATX, Tặng bàn phím + chuột", "https://images.unsplash.com/photo-1593640408182-31c228f1e97e?w=400&q=80", "PC Văn phòng Intel Core i3 Budget", 8490000m },
                    { 13, 3, "27\", 4K UHD 3840x2160, IPS, 60Hz, HDR400, USB-C 60W, sRGB 99%", "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&q=80", "Màn hình LG 27\" 4K IPS UltraFine", 8990000m },
                    { 14, 3, "32\", QHD 2560x1440, VA Curved, 165Hz, 1ms MPRT, FreeSync Premium", "https://images.unsplash.com/photo-1586210579191-33b45e38fa2c?w=400&q=80", "Màn hình Samsung 32\" Odyssey G5", 7490000m },
                    { 15, 3, "27\", FHD IPS, 240Hz, 1ms GTG, G-Sync Compatible, HDR10, USB Hub", "https://images.unsplash.com/photo-1551645120-d70bfe84c826?w=400&q=80", "Màn hình ASUS ROG Swift 27\" 240Hz", 12990000m },
                    { 16, 3, "24\", FHD 1920x1080, IPS, 75Hz, 4ms, FreeSync, VGA+HDMI, Frameless", "https://images.unsplash.com/photo-1616763355548-1b606f439f86?w=400&q=80", "Màn hình AOC 24\" FHD 75Hz", 2990000m },
                    { 17, 4, "75%, Hot-swap, RGB backlit, Gateron Brown switch, Bluetooth 5.1 + USB-C", "https://images.unsplash.com/photo-1609081219090-a6d81d3085bf?w=400&q=80", "Bàn phím cơ Keychron K2 V2", 2290000m },
                    { 18, 4, "LIGHTFORCE hybrid switch, HERO 25K sensor, 25,600 DPI, 13 nút, sạc không dây", "https://images.unsplash.com/photo-1527814050087-3793815479db?w=400&q=80", "Chuột gaming Logitech G502 X Plus", 2890000m },
                    { 19, 4, "Chống ồn ANC hàng đầu, 30h pin, LDAC Hi-Res, Multipoint, gập gọn", "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400&q=80", "Tai nghe Sony WH-1000XM5", 8990000m },
                    { 20, 4, "Driver kép 50mm, mic tháo rời, âm thanh vòm 7.1, tương thích PC/PS/Xbox", "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&q=80", "Tai nghe gaming HyperX Cloud Alpha", 1990000m },
                    { 21, 4, "Full HD 1080p/30fps, autofocus, mic kép, tích hợp, clip gắn màn hình", "https://images.unsplash.com/photo-1587826080692-f439cd0b70da?w=400&q=80", "Webcam Logitech C920 HD Pro", 1690000m },
                    { 22, 4, "HDMI 4K, USB-A 3.0 x2, USB-C PD 100W, SD/microSD, tương thích MacBook/Win", "https://images.unsplash.com/photo-1625948515291-69613efd103f?w=400&q=80", "Bộ hub USB-C 7-in-1 Anker", 890000m },
                    { 23, 5, "PCIe 4.0 x4, Read 7,000MB/s, Write 5,000MB/s, TLC V-NAND, tản nhiệt", "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&q=80", "SSD Samsung 980 Pro 1TB NVMe M.2", 2790000m },
                    { 24, 5, "DDR5-5200MHz, 32GB (2x16GB), CL40, Intel XMP 3.0 & AMD EXPO, tản RGB", "https://images.unsplash.com/photo-1541029071515-84cc54f84dc5?w=400&q=80", "RAM DDR5 Kingston Fury Beast 32GB", 2490000m },
                    { 25, 5, "16 nhân (8P+8E) / 24 luồng, Boost 5.4GHz, L3 30MB, TDP 125W, socket LGA1700", "https://images.unsplash.com/photo-1591799265444-d66432b91588?w=400&q=80", "CPU Intel Core i7-13700K", 8990000m },
                    { 26, 5, "6 nhân / 12 luồng, Boost 5.3GHz, L3 32MB, TDP 105W, PCIe 5.0, AM5", "https://images.unsplash.com/photo-1555617981-dac3880eac6e?w=400&q=80", "CPU AMD Ryzen 5 7600X", 5490000m },
                    { 27, 5, "12GB GDDR6X, 192-bit, DLSS 3, Ray Tracing, 2505MHz Boost, 2x HDMI 2.1, 3x DP", "https://images.unsplash.com/photo-1587202372775-e229f172b9d7?w=400&q=80", "VGA ASUS GeForce RTX 4070 12GB DUAL", 16990000m },
                    { 28, 5, "Intel B760, LGA1700, DDR5, PCIe 4.0, WiFi 6E, BT 5.3, 2x M.2, mATX", "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400&q=80", "Mainboard MSI MAG B760M Mortar WiFi", 3990000m },
                    { 29, 5, "850W, 80+ Gold, Full Modular, 10 năm bảo hành, Zero RPM fan, ATX 3.0", "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?w=400&q=80", "Nguồn Corsair RM850x 850W 80+ Gold", 2990000m },
                    { 30, 5, "Radiator 240mm, 2x fan 120mm PWM, LCD màn hình hiển thị, ARGB, socket AM5/1700", "https://images.unsplash.com/photo-1560732488-6b0df240254a?w=400&q=80", "Tản nhiệt nước AIO NZXT Kraken 240", 2690000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_UserId",
                table: "WishlistItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "WishlistItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
