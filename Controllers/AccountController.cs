using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;

namespace ConsultingGroup.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Redirect authenticated users to their appropriate dashboard
                if (User.IsInRole("Administrator"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (User.IsInRole("UserSenior"))
                {
                    return RedirectToAction("Welcome", "Senior");
                }
                else if (User.IsInRole("User"))
                {
                    return RedirectToAction("Dashboard", "User");
                }
                
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{UserName}' logged in.", model.UserName);
                    
                    // Update last login time
                    var user = await _userManager.FindByNameAsync(model.UserName);
                    if (user != null)
                    {
                        user.LastLoginAt = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirect based on user role
                    var userRoles = await _userManager.GetRolesAsync(user!);
                    
                    if (userRoles.Contains("Administrator"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (userRoles.Contains("UserSenior"))
                    {
                        return RedirectToAction("Welcome", "Senior");
                    }
                    else if (userRoles.Contains("User"))
                    {
                        return RedirectToAction("Dashboard", "User");
                    }
                    
                    // Default fallback
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User '{UserName}' account locked out.", model.UserName);
                    ModelState.AddModelError("", "Account bloccato per troppi tentativi falliti. Riprova più tardi.");
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt for user '{UserName}'.", model.UserName);
                    ModelState.AddModelError("", "Nome utente o password non validi.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}