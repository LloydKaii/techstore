using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;
using TechStore.Services;
using TechStore.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromMinutes(60); o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<TechStore.Services.AIService>();

// EF Core
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, EFProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();

// For HomeController DI
builder.Services.AddScoped<HomeController>(provider =>
{
    var productRepo = provider.GetRequiredService<IProductRepository>();
    var categoryRepo = provider.GetRequiredService<ICategoryRepository>();
    return new HomeController(productRepo, categoryRepo);
});

// Identity - 3 roles: Admin, Manager, Customer
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.Password.RequireDigit = true; o.Password.RequireLowercase = true;
    o.Password.RequireUppercase = false; o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 6; o.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/AccessDenied"; o.Cookie.Name = "TechStore.Auth";
});

var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting();
app.UseSession(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Roles + Users
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var um = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // 3 roles
        foreach (var r in new[] { "Admin", "Manager", "Customer" })
            if (!await rm.RoleExistsAsync(r)) await rm.CreateAsync(new IdentityRole(r));

        // Admin mặc định
        const string ae = "admin@techstore.vn", ap = "Admin@123";
        if (await um.FindByEmailAsync(ae) == null)
        {
            var a = new ApplicationUser { UserName = ae, Email = ae, FullName = "Administrator", EmailConfirmed = true };
            if ((await um.CreateAsync(a, ap)).Succeeded) await um.AddToRoleAsync(a, "Admin");
        }

        // Manager mặc định
        const string me = "manager@techstore.vn", mp = "Manager@123";
        if (await um.FindByEmailAsync(me) == null)
        {
            var m = new ApplicationUser { UserName = me, Email = me, FullName = "Quản lý cửa hàng", EmailConfirmed = true };
            if ((await um.CreateAsync(m, mp)).Succeeded) await um.AddToRoleAsync(m, "Manager");
        }
    }
    catch { }
}
app.Run();
