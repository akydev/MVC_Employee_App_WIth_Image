using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_Employee_App_WIth_Image.Models;
using MVC_Employee_App_WIth_Image.ViewModel;

namespace MVC_Employee_App_WIth_Image.Controllers
{
    // Handles user authentication: Register, Login, Logout, and Access Denied.
    // Uses ASP.NET Core Identity for secure user management.
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Constructor with dependency injection
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ===================== REGISTER =====================
        // Displays the registration page.
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handles user registration.
        // Validates input, checks for duplicate username/email,
        // and creates a new user in the system.
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            // Return if validation fails.
            if (!ModelState.IsValid)
                return View(vm);

            // Check if username already exists.
            var existingUser = await _userManager.FindByNameAsync(vm.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username already taken. Please choose another.");
                return View(vm);
            }

            // Check email uniqueness.
            var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(vm);
            }

            // Create new user.
            var user = new ApplicationUser
            {
                UserName = vm.UserName,
                Email = vm.Email
            };
            // Attempt to create user with password.
            var result = await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                // Auto login after successful registration.
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Employee");
            }

            // Identity error handling
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        // ===================== LOGIN =====================
        // Displays the login page.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        // Handles user login using username or email.
        // Supports "Remember Me" functionality.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            // Return if validation fails
            if (!ModelState.IsValid)
                return View(vm);

            // Attempt to find user by username first
            var user = await _userManager.FindByNameAsync(vm.UserName);

            // If not found and input looks like email, try email
            if (user == null && vm.UserName.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(vm.UserName);
            }

            // If user not found, return → invalid login
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or email or password.");
                return View(vm);
            }

            // Now login using the actual username & password
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
            // Invalid login attempt
            ModelState.AddModelError(string.Empty, "Invalid username or email or password.");
            return View(vm);
        }


        // ===================== LOGOUT =====================
        // Logs out the current user securely.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // ===================== ACCESS DENIED =====================
        // Displays access denied page for unauthorized users.
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}