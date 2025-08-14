using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    public class SeniorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SeniorController> _logger;

        public SeniorController(
            UserManager<ApplicationUser> userManager,
            ILogger<SeniorController> logger)
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

        public IActionResult Profile()
        {
            return View();
        }

        // Senior può accedere a tutte le aree tranne la gestione utenti
        [UserPermission("gestioneclienti")]
        public IActionResult GestioneClienti()
        {
            return RedirectToAction("Index", "GestioneClienti");
        }

        [UserPermission("datiutenzariservata")]
        public IActionResult DatiUtenza()
        {
            return RedirectToAction("Index", "DatiUtenza");
        }

        [Authorize(Roles = "UserSenior,Administrator")]
        public IActionResult Reports()
        {
            ViewBag.Message = "Area Reports - Accesso completo per utenti Senior";
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Settings()
        {
            ViewBag.Message = "Impostazioni di sistema - Solo Administrator";
            return View();
        }
    }
}