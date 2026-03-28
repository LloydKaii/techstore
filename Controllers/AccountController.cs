using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using TechStore.ViewModels;

namespace TechStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm)
        {
            _userManager = um;
            _signInManager = sm;
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.FullName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                await _signInManager.SignInAsync(user, false);
                TempData["Success"] = $"Chào mừng {vm.FullName}! Đăng ký thành công.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return View(vm);
        }

        // GET: /Account/Login
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
            if (result.Succeeded)
            {
                TempData["Success"] = "Đăng nhập thành công!";
                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(vm);
        }

        // POST: /Account/Logout
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Đã đăng xuất.";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied() => View();

        // GET: /Account/Profile
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;
            ViewBag.ProfileView = roles.Contains("Admin") ? "Admin" : roles.Contains("Manager") ? "Manager" : "Customer";
            return View(user);
        }
    }
}
