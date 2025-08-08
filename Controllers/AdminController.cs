using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Models;

namespace ConsultingGroup.Controllers
{
    [Authorize(Roles = "Administrator")]
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

        public IActionResult Dashboard()
        {
            return RedirectToAction("Index");
        }

        // Placeholder pages
        [Authorize(Roles = "Administrator,UserSenior")]
        public IActionResult GestioneAmministrativa()
        {
            ViewData["PageTitle"] = "Gestione Amministrativa";
            ViewData["Message"] = "Questa sezione è in fase di sviluppo.";
            return View("Placeholder");
        }
    }
}