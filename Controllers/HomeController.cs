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
            // Se l'utente è autenticato, reindirizza alla dashboard admin
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Administrator"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Dashboard", "Admin");
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