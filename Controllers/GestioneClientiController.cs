using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("gestioneclienti")]
    public class GestioneClientiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<GestioneClientiController> _logger;

        public GestioneClientiController(
            UserManager<ApplicationUser> userManager,
            ILogger<GestioneClientiController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";
            
            return View();
        }

        // Placeholder per funzionalità future
        public IActionResult Lista()
        {
            ViewData["PageTitle"] = "Lista Clienti";
            ViewData["Message"] = "Questa sezione mostrerà la lista completa dei clienti.";
            return View("Placeholder");
        }

        public IActionResult Nuovo()
        {
            ViewData["PageTitle"] = "Nuovo Cliente";
            ViewData["Message"] = "Qui potrai aggiungere un nuovo cliente al sistema.";
            return View("Placeholder");
        }

        public IActionResult Ricerca()
        {
            ViewData["PageTitle"] = "Ricerca Clienti";
            ViewData["Message"] = "Funzionalità di ricerca avanzata per trovare clienti specifici.";
            return View("Placeholder");
        }
    }
}