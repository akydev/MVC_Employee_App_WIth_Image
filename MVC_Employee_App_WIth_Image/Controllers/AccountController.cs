using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Employee_App_WIth_Image.Models;
using MVC_Employee_App_WIth_Image.ViewModel;

namespace MVC_Employee_App_WIth_Image.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ===================== REGISTER =====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Check if username already exists (FIX ADDED)
            var existingUser = await _userManager.FindByNameAsync(vm.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already taken. Please choose another.");
                return View(vm);
            }

            // Check email uniqueness
            var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Email
            };

            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Employee");
            }

            // Better error handling
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Try to find user by username first
            var user = await _userManager.FindByNameAsync(vm.UserName);

            // If not found and input looks like email, try email
            if (user == null && vm.UserName.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(vm.UserName);
            }

            // If still not found → invalid login
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or email or password.");
                return View(vm);
            }

            // Now login using the actual username
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                vm.Password,
                vm.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Employee");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or email or password.");
            return View(vm);
        }
        

        // ===================== LOGOUT =====================
        // FIX: Add POST + AntiForgery (SECURITY FIX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ===================== ACCESS DENIED =====================
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}