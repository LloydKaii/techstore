# 🔑 TechStore Role-Based Access Control (RBAC) Guide

## 🎯 Role Definitions & Responsibilities

### 👑 ADMIN (System Administrator)
**Scope:** Full system control

**Dashboard:** `/Admin/Dashboard`
- Total products, categories, users, admins count
- Total orders and revenue
- Pending orders count
- 7-day revenue chart
- Recent products, orders, users (samples)
- Quick action links

**Permissions:**
| Action | Allow |
|--------|-------|
| Add/Edit/Delete Products | ✅ |
| Add/Edit/Delete Categories | ✅ |
| View All Orders | ✅ |
| Update Order Status | ✅ |
| Manage Users (Add/Delete) | ✅ |
| Set User Roles | ✅ |
| Manage Vouchers (CRUD) | ✅ |
| Access Admin Panel | ✅ |
| Access Manager Panel | ❌ |

**Key Controllers:**
- `AdminController` - Users, Vouchers, Dashboard
- `ProductController` - Add/Update/Delete (public browse OK)
- `OrderController` - Manage all orders

---

### 📊 MANAGER (Inventory & Sales Manager)
**Scope:** Product and order management

**Dashboard:** `/Manager/Dashboard`
- Total products and orders count
- Total revenue (valid orders only)
- Pending, shipping, completed orders count
- Top 5 selling products
- 7-day revenue chart
- Recent orders list (10 latest)
- Quick action links (Products, Orders, Categories, Sales Report)

**Permissions:**
| Action | Allow |
|--------|-------|
| Add/Edit/Delete Products | ✅ |
| Add/Edit/Delete Categories | ✅ |
| View All Orders | ✅ |
| Update Order Status | ✅ |
| Generate Sales Reports | ✅ |
| View Top Products | ✅ |
| Stream Analytics | ✅ |
| **Manage Users** | ❌ |
| **Set Roles** | ❌ |
| **Manage Vouchers** | ❌ |
| **Access Admin Panel** | ❌ |

**Key Controllers:**
- `ManagerController` - Full manager features
- `ProductController` - Can only view/browse (not add/delete)
- `OrderController` - View and update only

**Features:**
```
/Manager/Dashboard           → Sales overview + metrics
/Manager/Products           → Product CRUD management
/Manager/Categories         → Category CRUD management
/Manager/Orders            → View + status management
/Manager/Orders?status=X   → Filter by status
/Manager/SalesReport       → Analytics & reporting
```

---

### 👤 CUSTOMER (End User/Shopper)
**Scope:** Personal shopping activities

**Dashboard:** `/Account/Profile`
- Profile information
- Order history (`/Order/MyOrders`)
- Wishlist (`/Wishlist`)
- PC Builds (`/Build/MyBuilds`)

**Permissions:**
| Action | Allow |
|--------|-------|
| Browse Products | ✅ |
| View Product Details | ✅ |
| Search Products | ✅ |
| Add to Cart | ✅ |
| Checkout | ✅ |
| View Own Orders | ✅ |
| Add to Wishlist | ✅ |
| Write Reviews | ✅ |
| Delete Own Reviews | ✅ |
| Build PC (Custom + Auto) | ✅ |
| Save Builds | ✅ |
| **View All Orders** | ❌ |
| **Access Admin Panel** | ❌ |
| **Manage Inventory** | ❌ |
| **Delete Other's Reviews** | ❌ |

**Key Controllers:**
- `HomeController` - Main pages
- `ProductController` - Browse only
- `CartController` - Cart operations
- `OrderController` - View own orders
- `WishlistController` - Personal wishlist
- `BuildController` - PC builder
- `ReviewController` - Write & edit own reviews
- `AccountController` - Profile management

---

### 🕵️ GUEST (Anonymous User)
**Scope:** Read-only access

**Access:**
- Browse products
- View product details
- View categories
- Access home page
- Access cart (temporary session)
- **Cannot** checkout without login
- **Cannot** save builds or wishlist

---

## 🔐 Authorization Flow in Code

### Using [Authorize] Attributes:

```csharp
// Admin only
[Authorize(Roles = "Admin")]
public async Task<IActionResult> AdminDashboard() { }

// Manager only
[Authorize(Roles = "Manager")]
public async Task<IActionResult> ManagerDashboard() { }

// Any authenticated user
[Authorize]
public async Task<IActionResult> MyOrders() { }

// Admin OR Manager
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> ManageOrders() { }

// Public (no auth required)
[AllowAnonymous]
public async Task<IActionResult> Index() { }
```

---

## 🗺️ Route Map by Role

```
/
├─ Home (public)
├─ Product/
│  ├─ Index (public) → Browse all
│  ├─ Display/:id (public) → View details
│  ├─ Add (Admin only) → Add product
│  ├─ Update/:id (Admin only) → Edit product
│  └─ Delete/:id (Admin only) → Remove product
│
├─ Cart/ (public → requires login for checkout)
│  ├─ Index (public) → View cart
│  ├─ Add (public) → Add to cart
│  ├─ Checkout (must login) → Checkout page
│  └─ PlaceOrder (must login) → Place order
│
├─ Order/
│  ├─ MyOrders (must login) → Customer's orders
│  ├─ Detail/:id (must login) → Order details
│  ├─ Manage (Admin, Manager) → All orders
│  └─ UpdateStatus (Admin, Manager) → Change status
│
├─ Admin/ (Admin only)
│  ├─ Dashboard → Stats & charts
│  ├─ Users → User management
│  ├─ Vouchers → Voucher CRUD
│  └─ ... other admin functions
│
├─ Manager/ (Manager only)
│  ├─ Dashboard → Sales metrics
│  ├─ Products → Product management
│  ├─ Categories → Category management
│  ├─ Orders → Order management
│  ├─ SalesReport → Analytics
│  └─ ... other manager functions
│
├─ Account/
│  ├─ Login (public)
│  ├─ Register (public)
│  ├─ Profile (must login)
│  └─ Logout (must login)
│
└─ ... other public routes
```

---

## 🎨 UI/Navigation Changes

### Navigation Bar Changes by Role:

**Anonymous/Guest:**
```
[Home] [Sản phẩm] [Build PC] [Danh mục] [🛒 Giỏ]  [Đăng nhập]
```

**Customer:**
```
[Home] [Sản phẩm] [Build PC] [Danh mục] [🛒 Giỏ] [👤 Profile ▼]
                                                  ├─ Hồ sơ
                                                  ├─ Đơn hàng của tôi
                                                  ├─ Yêu thích
                                                  ├─ ─────────
                                                  └─ Đăng xuất
```

**Manager:**
```
[Home] [Sản phẩm] [Build PC] [Danh mục] [ 📊 Manager] [🛒 Giỏ] [👤 Profile ▼]
                                                                 ├─ Hồ sơ
                                                                 ├─ Đơn hàng của tôi
                                                                 ├─ Yêu thích
                                                                 ├─ ─────────
                                                                 ├─ Manager Dashboard
                                                                 ├─ Quản lý sản phẩm
                                                                 ├─ Quản lý đơn hàng
                                                                 ├─ Báo cáo bán hàng
                                                                 ├─ ─────────
                                                                 └─ Đăng xuất
```

**Admin:**
```
[Home] [Sản phẩm] [Build PC] [Danh mục] [🛡️ Admin Panel] [🛒 Giỏ] [👤 Profile ▼]
                                                                      ├─ Hồ sơ
                                                                      ├─ Đơn hàng của tôi
                                                                      ├─ Yêu thích
                                                                      ├─ ─────────
                                                                      ├─ Admin Dashboard
                                                                      ├─ Quản lý Users
                                                                      ├─ Mã giảm giá
                                                                      ├─ ─────────
                                                                      └─ Đăng xuất
```

### Dashboard Colors:
- **Admin Dashboard**: Cyan accent (#00ffff)
- **Manager Dashboard**: Yellow accent (#fbbf24)
- **Customer Profile**: Default (blue/cyan)

---

## 🧪 Testing Authorization

### Step 1: Test Admin Access
```
1. Create admin account (or use seed admin@techstore.vn)
2. Login as admin
3. Navigate to /Admin/Dashboard
4. Verify can see: Users page, Vouchers, All tabs
5. Verify cannot see: Manager link in navbar
```

### Step 2: Test Manager Access
```
1. Create manager account (database seeding or manual)
2. Login as manager
3. Navigate to /Manager/Dashboard
4. Verify can see: Products, Orders, Sales Report
5. Verify cannot see: Admin link, Users page (404)
6. Try /Admin/Dashboard → Should get 403 Forbid
```

### Step 3: Test Customer Access
```
1. Register as customer (new account)
2. Login
3. Can browse products, add to cart, checkout
4. Can view own orders (/Order/MyOrders)
5. Try /Admin/Dashboard → Should get 403 Forbid
6. Try /Manager/Dashboard → Should get 403 Forbid
```

### Step 4: Test Guest (Anonymous)
```
1. Don't login, stay anonymous
2. Can browse /Product
3. Can add to /Cart
4. Try /Cart/Checkout → Redirects to /Account/Login
5. Try /Admin/Dashboard → Redirects to /Account/Login
```

---

## 📝 Common Authorization Errors & Solutions

| Error | Cause | Fix |
|-------|-------|-----|
| **404 Not Found** | Route doesn't exist | Check controller/action exists |
| **403 Forbidden** | Insufficient permissions | Not in required role |
| **Redirect to Login** | Not authenticated | Need to login first |
| **Authorization Failed** | Role mismatch | Wrong user role for action |

---

## 🔄 Changing User Roles

**Only Admin Can:**
1. Go to `/Admin/Users`
2. Find user in list
3. Click dropdown to change role
4. Select new role (Admin, Manager, Customer)
5. Confirm

**Programmatically:**
```csharp
var user = await _userManager.FindByIdAsync(userId);
await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
await _userManager.AddToRoleAsync(user, "Manager");
```

---

## ⚠️ Security Notes

1. **Never downgrade last Admin**: Always keep at least one Admin account
2. **Manager limitations**: Cannot create users or delete accounts
3. **Customer isolation**: Can only see their own orders
4. **API authentication**: All API endpoints require authorization
5. **Session timeout**: Configure in `appsettings.json`

---

*Last Updated: 2026-03-28*
*TechStore RBAC Documentation v1.0*
