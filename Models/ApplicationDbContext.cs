using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<PCBuild> PCBuilds { get; set; }
        public DbSet<PCBuildItem> PCBuildItems { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComponentType> ComponentTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Laptop", Description = "Máy tính xách tay" },
                new Category { Id = 2, Name = "PC / Desktop", Description = "Máy tính để bàn" },
                new Category { Id = 3, Name = "Màn hình", Description = "Monitor các loại" },
                new Category { Id = 4, Name = "Phụ kiện", Description = "Bàn phím, chuột, tai nghe" },
                new Category { Id = 5, Name = "Linh kiện", Description = "CPU, RAM, SSD, VGA" }
            );

            modelBuilder.Entity<Product>().HasData(
                // ── LAPTOP ─────────────────────────────────────────────────────
                new Product { Id = 1, Name = "Laptop ASUS VivoBook 15 X1504ZA", Price = 14990000, CategoryId = 1, Description = "Core i5-1235U, RAM 8GB DDR4, SSD 512GB NVMe, 15.6\" FHD IPS, Iris Xe, Win 11", ImageUrl = "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=400&q=80" },
                new Product { Id = 2, Name = "Laptop Dell Inspiron 15 3520", Price = 16490000, CategoryId = 1, Description = "Core i5-1235U, RAM 16GB DDR4, SSD 512GB, 15.6\" FHD, Win 11 Home", ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=400&q=80" },
                new Product { Id = 3, Name = "Laptop HP Pavilion 15-eg3097TX", Price = 18990000, CategoryId = 1, Description = "Core i7-1355U, RAM 16GB, SSD 512GB, GeForce RTX 2050 4GB, 15.6\" FHD IPS", ImageUrl = "https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400&q=80" },
                new Product { Id = 4, Name = "Laptop Lenovo IdeaPad 5 Pro 16ARH7", Price = 21490000, CategoryId = 1, Description = "Ryzen 7 6800H, RAM 16GB, SSD 512GB, Radeon RX 6600M, 16\" 2.5K 120Hz", ImageUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&q=80" },
                new Product { Id = 5, Name = "Laptop MacBook Air M2 13\"", Price = 27990000, CategoryId = 1, Description = "Apple M2 8-core, RAM 8GB Unified, SSD 256GB, 13.6\" Liquid Retina, macOS", ImageUrl = "https://images.unsplash.com/photo-1611186871525-b6a9b7b3a4f4?w=400&q=80" },
                new Product { Id = 6, Name = "Laptop ASUS ROG Strix G16 2024", Price = 42990000, CategoryId = 1, Description = "Core i9-14900HX, RTX 4070 8GB, RAM 32GB DDR5, SSD 1TB, 16\" QHD 240Hz", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400&q=80" },
                new Product { Id = 7, Name = "Laptop Acer Nitro 5 AN515-58", Price = 19990000, CategoryId = 1, Description = "Core i5-12500H, RTX 3050 4GB, RAM 8GB, SSD 512GB, 15.6\" FHD 144Hz", ImageUrl = "https://images.unsplash.com/photo-1580927752452-89d86da3fa0a?w=400&q=80" },
                new Product { Id = 8, Name = "Laptop MSI Thin GF63 12VE", Price = 22490000, CategoryId = 1, Description = "Core i7-12650H, RTX 4050 6GB, RAM 16GB, SSD 512GB, 15.6\" FHD 144Hz", ImageUrl = "https://images.unsplash.com/photo-1547082299-de196ea013d6?w=400&q=80" },

                // ── PC / DESKTOP ────────────────────────────────────────────────
                new Product { Id = 9, Name = "PC Gaming RGB i7 RTX 4060", Price = 28900000, CategoryId = 2, Description = "Core i7-13700F, RTX 4060 8GB, RAM 16GB DDR5, SSD 1TB NVMe, Case ATX Full RGB", ImageUrl = "https://images.unsplash.com/photo-1587202372634-32705e3bf49c?w=400&q=80" },
                new Product { Id = 10, Name = "PC Workstation Ryzen 9 Pro", Price = 38500000, CategoryId = 2, Description = "Ryzen 9 7900X, RTX 4070 Ti 12GB, RAM 32GB DDR5, SSD 2TB, Cooler Noctua NH-D15", ImageUrl = "https://images.unsplash.com/photo-1520659649-1b4f85f5cfc0?w=400&q=80" },
                new Product { Id = 11, Name = "Mini PC Intel NUC 13 Pro", Price = 11990000, CategoryId = 2, Description = "Core i5-1340P, RAM 16GB LPDDR5, SSD 512GB, Thunderbolt 4, WiFi 6E, BT 5.3", ImageUrl = "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400&q=80" },
                new Product { Id = 12, Name = "PC Văn phòng Intel Core i3 Budget", Price = 8490000, CategoryId = 2, Description = "Core i3-12100, RAM 8GB DDR4, SSD 256GB, Case mATX, Tặng bàn phím + chuột", ImageUrl = "https://images.unsplash.com/photo-1593640408182-31c228f1e97e?w=400&q=80" },

                // ── MÀN HÌNH ────────────────────────────────────────────────────
                new Product { Id = 13, Name = "Màn hình LG 27\" 4K IPS UltraFine", Price = 8990000, CategoryId = 3, Description = "27\", 4K UHD 3840x2160, IPS, 60Hz, HDR400, USB-C 60W, sRGB 99%", ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&q=80" },
                new Product { Id = 14, Name = "Màn hình Samsung 32\" Odyssey G5", Price = 7490000, CategoryId = 3, Description = "32\", QHD 2560x1440, VA Curved, 165Hz, 1ms MPRT, FreeSync Premium", ImageUrl = "https://images.unsplash.com/photo-1586210579191-33b45e38fa2c?w=400&q=80" },
                new Product { Id = 15, Name = "Màn hình ASUS ROG Swift 27\" 240Hz", Price = 12990000, CategoryId = 3, Description = "27\", FHD IPS, 240Hz, 1ms GTG, G-Sync Compatible, HDR10, USB Hub", ImageUrl = "https://images.unsplash.com/photo-1551645120-d70bfe84c826?w=400&q=80" },
                new Product { Id = 16, Name = "Màn hình AOC 24\" FHD 75Hz", Price = 2990000, CategoryId = 3, Description = "24\", FHD 1920x1080, IPS, 75Hz, 4ms, FreeSync, VGA+HDMI, Frameless", ImageUrl = "https://images.unsplash.com/photo-1616763355548-1b606f439f86?w=400&q=80" },

                // ── PHỤ KIỆN ────────────────────────────────────────────────────
                new Product { Id = 17, Name = "Bàn phím cơ Keychron K2 V2", Price = 2290000, CategoryId = 4, Description = "75%, Hot-swap, RGB backlit, Gateron Brown switch, Bluetooth 5.1 + USB-C", ImageUrl = "https://images.unsplash.com/photo-1609081219090-a6d81d3085bf?w=400&q=80" },
                new Product { Id = 18, Name = "Chuột gaming Logitech G502 X Plus", Price = 2890000, CategoryId = 4, Description = "LIGHTFORCE hybrid switch, HERO 25K sensor, 25,600 DPI, 13 nút, sạc không dây", ImageUrl = "https://images.unsplash.com/photo-1527814050087-3793815479db?w=400&q=80" },
                new Product { Id = 19, Name = "Tai nghe Sony WH-1000XM5", Price = 8990000, CategoryId = 4, Description = "Chống ồn ANC hàng đầu, 30h pin, LDAC Hi-Res, Multipoint, gập gọn", ImageUrl = "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400&q=80" },
                new Product { Id = 20, Name = "Tai nghe gaming HyperX Cloud Alpha", Price = 1990000, CategoryId = 4, Description = "Driver kép 50mm, mic tháo rời, âm thanh vòm 7.1, tương thích PC/PS/Xbox", ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&q=80" },
                new Product { Id = 21, Name = "Webcam Logitech C920 HD Pro", Price = 1690000, CategoryId = 4, Description = "Full HD 1080p/30fps, autofocus, mic kép, tích hợp, clip gắn màn hình", ImageUrl = "https://images.unsplash.com/photo-1587826080692-f439cd0b70da?w=400&q=80" },
                new Product { Id = 22, Name = "Bộ hub USB-C 7-in-1 Anker", Price = 890000, CategoryId = 4, Description = "HDMI 4K, USB-A 3.0 x2, USB-C PD 100W, SD/microSD, tương thích MacBook/Win", ImageUrl = "https://images.unsplash.com/photo-1625948515291-69613efd103f?w=400&q=80" },

                // ── LINH KIỆN ───────────────────────────────────────────────────
                new Product { Id = 23, Name = "SSD Samsung 980 Pro 1TB NVMe M.2", Price = 2790000, CategoryId = 5, Description = "PCIe 4.0 x4, Read 7,000MB/s, Write 5,000MB/s, TLC V-NAND, tản nhiệt", ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&q=80" },
                new Product { Id = 24, Name = "RAM DDR5 Kingston Fury Beast 32GB", Price = 2490000, CategoryId = 5, Description = "DDR5-5200MHz, 32GB (2x16GB), CL40, Intel XMP 3.0 & AMD EXPO, tản RGB", ImageUrl = "https://images.unsplash.com/photo-1541029071515-84cc54f84dc5?w=400&q=80" },
                new Product { Id = 25, Name = "CPU Intel Core i7-13700K", Price = 8990000, CategoryId = 5, Description = "16 nhân (8P+8E)/24 luồng, Boost 5.4GHz, L3 30MB, TDP 125W, LGA1700", ImageUrl = "https://images.unsplash.com/photo-1591799265444-d66432b91588?w=400&q=80" },
                new Product { Id = 26, Name = "CPU AMD Ryzen 5 7600X", Price = 5490000, CategoryId = 5, Description = "6 nhân/12 luồng, Boost 5.3GHz, L3 32MB, TDP 105W, PCIe 5.0, AM5", ImageUrl = "https://images.unsplash.com/photo-1555617981-dac3880eac6e?w=400&q=80" },
                new Product { Id = 31, Name = "CPU Intel Core i5-13600K", Price = 7290000, CategoryId = 5, Description = "14 nhân (6P+8E)/20 luồng, Boost 5.1GHz, LGA1700, DDR5", ImageUrl = "https://images.unsplash.com/photo-1593642702821-c8da6771f0c6?w=400&q=80" },
                new Product { Id = 32, Name = "CPU AMD Ryzen 7 7700X", Price = 8990000, CategoryId = 5, Description = "8 nhân/16 luồng, Boost 5.4GHz, AM5, DDR5, PCIe 5.0", ImageUrl = "https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=400&q=80" },
                new Product { Id = 33, Name = "CPU Intel Core i3-13100F", Price = 2790000, CategoryId = 5, Description = "4 nhân/8 luồng, Budget gaming/office, LGA1700", ImageUrl = "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?w=400&q=80" },
                new Product { Id = 34, Name = "RAM DDR4 16GB Corsair Vengeance 3200MHz", Price = 1590000, CategoryId = 5, Description = "16GB (2x8GB) CL16, RGB optional, Intel/AMD", ImageUrl = "https://images.unsplash.com/photo-1541029071515-84cc54f84dc5?w=400&q=80" },
                new Product { Id = 35, Name = "RAM DDR5 32GB G.Skill Trident Z5 6000MHz", Price = 3490000, CategoryId = 5, Description = "32GB (2x16GB) CL36, RGB, XMP 3.0", ImageUrl = "https://images.unsplash.com/photo-1611186871525-b6a9b7b3a4f4?w=400&q=80" },
                new Product { Id = 36, Name = "RAM DDR4 8GB Kingston HyperX Fury 3200MHz", Price = 890000, CategoryId = 5, Description = "Budget 8GB single stick, CL16", ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=400&q=80" },
                new Product { Id = 27, Name = "VGA NVIDIA RTX 4070 12GB", Price = 16990000, CategoryId = 5, Description = "DLSS 3, Ray Tracing, 1440p/4K gaming", ImageUrl = "https://images.unsplash.com/photo-1580927752452-89d86da3fa0a?w=400&q=80" },
                new Product { Id = 37, Name = "VGA NVIDIA RTX 4060 Ti 8GB", Price = 11990000, CategoryId = 5, Description = "1080p/1440p 144Hz, DLSS 3", ImageUrl = "https://images.unsplash.com/photo-1547082299-de196ea013d6?w=400&q=80" },
                new Product { Id = 38, Name = "VGA AMD RX 7600 8GB", Price = 8990000, CategoryId = 5, Description = "FSR 3, 1080p/1440p high FPS", ImageUrl = "https://images.unsplash.com/photo-1587202372634-32705e3bf49c?w=400&q=80" },
                new Product { Id = 28, Name = "Mainboard MSI B760M Mortar WiFi", Price = 3990000, CategoryId = 5, Description = "LGA1700, DDR5, PCIe 5.0, WiFi 6E", ImageUrl = "https://images.unsplash.com/photo-1520659649-1b4f85f5cfc0?w=400&q=80" },
                new Product { Id = 39, Name = "Mainboard ASUS PRIME B650-PLUS", Price = 4490000, CategoryId = 5, Description = "AM5, DDR5, PCIe 5.0, 3x M.2", ImageUrl = "https://images.unsplash.com/photo-1518770660439-4636190af475?w=400&q=80" },
                new Product { Id = 40, Name = "Mainboard Gigabyte B550 AORUS Elite", Price = 3290000, CategoryId = 5, Description = "AM4, DDR4, PCIe 4.0, budget gaming", ImageUrl = "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?w=400&q=80" },
                new Product { Id = 29, Name = "PSU Corsair RM850x 850W 80+ Gold", Price = 2990000, CategoryId = 5, Description = "Modular, ATX 3.0, 10y warranty", ImageUrl = "https://images.unsplash.com/photo-1560732488-6b0df240254a?w=400&q=80" },
                new Product { Id = 41, Name = "PSU Seasonic Focus GX-650 650W 80+ Gold", Price = 2390000, CategoryId = 5, Description = "Fully modular, 10y warranty", ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400&q=80" },
                new Product { Id = 42, Name = "PSU Cooler Master MWE 550 Bronze V2", Price = 1290000, CategoryId = 5, Description = "Budget 550W, non-modular", ImageUrl = "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=400&q=80" },
                new Product { Id = 43, Name = "Case NZXT H5 Flow RGB", Price = 2490000, CategoryId = 5, Description = "M-ATX, mesh front, RGB fans, tempered glass", ImageUrl = "https://images.unsplash.com/photo-1586210579191-33b45e38fa2c?w=400&q=80" },
                new Product { Id = 44, Name = "Case Corsair 4000D Airflow", Price = 2790000, CategoryId = 5, Description = "ATX, high airflow, front I/O USB-C", ImageUrl = "https://images.unsplash.com/photo-1551645120-d70bfe84c826?w=400&q=80" },
                new Product { Id = 45, Name = "Case Fractal Design Meshify C", Price = 3090000, CategoryId = 5, Description = "M-ATX, mesh design, compact", ImageUrl = "https://images.unsplash.com/photo-1616763355548-1b606f439f86?w=400&q=80" }
            );

            modelBuilder.Entity<Voucher>().HasData(
                new Voucher { Id = 1, Code = "TECHSTORE10", DiscountPercent = 10, IsActive = true, ExpiryDate = new DateTime(2027, 12, 31), UsageLimit = 1000 },
                new Voucher { Id = 2, Code = "SALE20", DiscountPercent = 20, IsActive = true, ExpiryDate = new DateTime(2027, 12, 31), UsageLimit = 100 },
                new Voucher { Id = 3, Code = "NEWUSER15", DiscountPercent = 15, IsActive = true, ExpiryDate = new DateTime(2027, 12, 31), UsageLimit = 500 },
                new Voucher { Id = 4, Code = "GAMING30", DiscountPercent = 30, IsActive = true, ExpiryDate = new DateTime(2027, 12, 31), UsageLimit = 50 },
                new Voucher { Id = 5, Code = "LINH5", DiscountPercent = 5, IsActive = true, ExpiryDate = new DateTime(2027, 12, 31), UsageLimit = 999 }
            );
        }
    }
}
