using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Welcome()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = user.FullName;
            ViewBag.LastLogin = user.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Primo accesso";
            
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = user.FullName;
            ViewBag.LastLogin = user.LastLoginAt?.ToString("dd/MM/yyyy HH:mm") ?? "Primo accesso";
            
            // Determina quali sezioni l'utente può vedere
            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.CanAccessAdmin = userRoles.Contains("Administrator");
            ViewBag.CanAccessSenior = userRoles.Contains("UserSenior") || userRoles.Contains("Administrator");
            ViewBag.CanCreateUsers = userRoles.Contains("Administrator");
            
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Profile()
        {
            return View();
        }

        // Visualizzazione dati base - Solo per Administrator
        [Authorize(Roles = "Administrator")]
        public IActionResult ViewBasicData()
        {
            ViewBag.Message = "Visualizzazione dati - Solo Administrator";
            return View();
        }

        // Report avanzati - Solo per utenti Senior e Administrator
        [Authorize(Roles = "UserSenior,Administrator")]
        public IActionResult AdvancedReports()
        {
            ViewBag.Message = "Report avanzati - Solo per utenti Senior e Administrator";
            return View();
        }

        // Gestione limitata solo per Administrator
        [Authorize(Roles = "Administrator")]
        public IActionResult BasicOperations()
        {
            ViewBag.Message = "Operazioni base - Solo Amministratori";
            return View();
        }

        // Area riservata - Solo Senior e Administrator
        [Authorize(Roles = "UserSenior,Administrator")]
        public IActionResult RestrictedArea()
        {
            ViewBag.Message = "Area riservata - Accesso autorizzato";
            return View();
        }
    }
}