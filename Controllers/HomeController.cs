using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using ConsultingGroup.ViewModels;

namespace ConsultingGroup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Se l'utente è autenticato, mostra direttamente la prima funzionalità disponibile
            if (User.Identity?.IsAuthenticated == true)
            {
                // Reindirizza alla prima sezione disponibile nel menu
                if (User.IsInRole("Administrator"))
                {
                    return RedirectToAction("Index", "UserManagement");
                }
                else if (User.IsInRole("UserSenior"))
                {
                    return RedirectToAction("Index", "GestioneClienti");
                }
                else
                {
                    return RedirectToAction("Index", "DatiUtenzaExtra");
                }
            }

            // Altrimenti mostra la pagina di login
            return RedirectToAction("Login", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}