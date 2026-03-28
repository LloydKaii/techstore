namespace TechStore.Config
{
    /// <summary>
    /// Application constants - Replace magic strings/numbers across the app
    /// </summary>
    public static class AppConstants
    {
        // ═══ PAGINATION ═══════════════════════════════════════════
        public const int DEFAULT_PAGE_SIZE = 12;
        public const int MANAGER_PAGE_SIZE = 20;
        public const int ITEMS_PER_DASHBOARD = 5;
        public const int ITEMS_PER_REPORT = 10;
        public const int ITEMS_PER_SEARCH = 25;

        // ═══ ORDER STATUS ═════════════════════════════════════════
        public const string ORDER_STATUS_PENDING = "Chờ xác nhận";
        public const string ORDER_STATUS_CONFIRMED = "Đã xác nhận";
        public const string ORDER_STATUS_SHIPPING = "Đang giao";
        public const string ORDER_STATUS_COMPLETED = "Hoàn thành";
        public const string ORDER_STATUS_CANCELLED = "Đã hủy";

        public static readonly string[] ALL_ORDER_STATUSES = new[]
        {
            ORDER_STATUS_PENDING,
            ORDER_STATUS_CONFIRMED,
            ORDER_STATUS_SHIPPING,
            ORDER_STATUS_COMPLETED,
            ORDER_STATUS_CANCELLED
        };

        // ═══ ROLES ════════════════════════════════════════════════
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_MANAGER = "Manager";
        public const string ROLE_CUSTOMER = "Customer";

        // ═══ PC BUILDER ═══════════════════════════════════════════
        public const decimal MIN_BUDGET = 5_000_000m; // 5 triệu VND
        public const decimal MAX_BUDGET = 100_000_000m; // 100 triệu VND
        public const int FRESH_COMPONENT_DAYS = 180; // 6 months
        public const decimal BUDGET_TOLERANCE = 0.10m; // 10% over-budget allowed

        public static readonly Dictionary<string, decimal> PC_BUILDER_BALANCED = new()
        {
            { "CPU", 0.25m }, { "GPU", 0.30m }, { "RAM", 0.15m }, { "SSD", 0.12m },
            { "MAINBOARD", 0.10m }, { "PSU", 0.08m }
        };

        public static readonly Dictionary<string, decimal> PC_BUILDER_GAMING = new()
        {
            { "CPU", 0.22m }, { "GPU", 0.42m }, { "RAM", 0.12m }, { "SSD", 0.10m },
            { "MAINBOARD", 0.08m }, { "PSU", 0.06m }
        };

        public static readonly Dictionary<string, decimal> PC_BUILDER_OFFICE = new()
        {
            { "CPU", 0.30m }, { "GPU", 0.15m }, { "RAM", 0.20m }, { "SSD", 0.15m },
            { "MAINBOARD", 0.12m }, { "PSU", 0.08m }
        };

        public static readonly Dictionary<string, decimal> PC_BUILDER_CONTENT = new()
        {
            { "CPU", 0.35m }, { "GPU", 0.28m }, { "RAM", 0.18m }, { "SSD", 0.10m },
            { "MAINBOARD", 0.05m }, { "PSU", 0.04m }
        };

        // ═══ VALIDATION ═══════════════════════════════════════════
        public const int NAME_MIN_LENGTH = 3;
        public const int NAME_MAX_LENGTH = 255;
        public const int DESCRIPTION_MAX_LENGTH = 1000;
        public const int NOTES_MAX_LENGTH = 500;
        public const decimal MIN_PRODUCT_PRICE = 0m;
        public const decimal MAX_PRODUCT_PRICE = 1_000_000_000m;

        // ═══ VOUCHER ══════════════════════════════════════════════
        public const int MIN_DISCOUNT_PERCENT = 5;
        public const int MAX_DISCOUNT_PERCENT = 100;
        public const int MIN_USAGE_LIMIT = 1;
        public const int MAX_USAGE_LIMIT = 10000;
        public const int MIN_VOUCHER_DAYS = 1;
        public const int MAX_VOUCHER_DAYS = 365;

        // ═══ PRODUCT CATEGORY ═════════════════════════════════════
        public const string CATEGORY_COMPONENTS = "Linh kiện";
        public const string CATEGORY_LAPTOPS = "Laptop";
        public const string CATEGORY_PC = "PC";

        // ═══ DATE/TIME ════════════════════════════════════════════
        public const int RECENT_DAYS = 7;
        public const int DASHBOARD_CHART_DAYS = 7;

        // ═══ UI/UX ════════════════════════════════════════════════
        public const int RESULTS_PER_SEARCH_DROPDOWN = 15;
        public const int TIMEOUT_MILLISECONDS = 5000;

        // ═══ ERROR MESSAGES ═══════════════════════════════════════
        public const string MSG_ADD_SUCCESS = "Đã thêm {0} thành công!";
        public const string MSG_UPDATE_SUCCESS = "Đã cập nhật {0} thành công!";
        public const string MSG_DELETE_SUCCESS = "Đã xóa {0} thành công!";
        public const string MSG_NOT_FOUND = "{0} không tồn tại!";
        public const string MSG_INVALID_ROLE = "Bạn không có quyền thực hiện hành động này!";
        public const string MSG_INVALID_INPUT = "{0} không hợp lệ!";
        public const string MSG_ITEM_IN_USE = "Không thể xóa {0} vì nó đang được sử dụng!";
    }
}
