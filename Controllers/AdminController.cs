using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Amministratore";
            ViewData["LastLogin"] = user?.LastLoginAt?.ToString("dd/MM/yyyy HH:mm");
            
            // Statistiche dashboard
            var totalUsers = _userManager.Users.Count();
            var activeUsers = _userManager.Users.Count(u => u.IsActive);
            var adminUsers = await _userManager.GetUsersInRoleAsync("Administrator");
            
            ViewData["TotalUsers"] = totalUsers;
            ViewData["ActiveUsers"] = activeUsers;
            ViewData["AdminUsers"] = adminUsers.Count;
            ViewData["InactiveUsers"] = totalUsers - activeUsers;
            
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Welcome()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Dashboard()
        {
            // Dashboard temporaneamente non disponibile - reindirizza alla gestione utenti
            return RedirectToAction("Index", "UserManagement");
        }

        // Placeholder pages
        [UserPermission("areaamministrativa")]
        public IActionResult GestioneAmministrativa()
        {
            ViewData["PageTitle"] = "Gestione Amministrativa";
            ViewData["Message"] = "Questa sezione è in fase di sviluppo.";
            return View("Placeholder");
        }
    }
}