using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ConsultingGroup.Attributes;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("GestioneAttivita")]
    public class GestioneAttivitaController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Gestione Attività";
            return View();
        }

        // Placeholder per le sezioni principali
        public IActionResult AttivitaRedditi()
        {
            ViewBag.Title = "Attività Redditi";
            ViewBag.Section = "redditi";
            return View("SectionIndex");
        }

        public IActionResult AttivitaIva()
        {
            ViewBag.Title = "Attività IVA";
            ViewBag.Section = "iva";
            return View("SectionIndex");
        }

        public IActionResult AttivitaContabile()
        {
            ViewBag.Title = "Attività Contabile";
            ViewBag.Section = "contabile";
            return View("SectionIndex");
        }
    }
}
