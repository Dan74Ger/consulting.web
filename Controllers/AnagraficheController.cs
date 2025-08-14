using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("anagrafiche")]
    public class AnagraficheController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnagraficheController> _logger;

        public AnagraficheController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<AnagraficheController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";
            
            // Ottieni statistiche reali dal database
            var allStudios = await _context.Studios.ToListAsync();
            var allProgrammi = await _context.Programmi.ToListAsync();
            var allAnniFiscali = await _context.AnniFiscali.ToListAsync();
            var allProfessionisti = await _context.Professionisti.ToListAsync();
            var allRegimiContabili = await _context.RegimiContabili.ToListAsync();
            var allTipologieInps = await _context.TipologieInps.ToListAsync();
            var allClienti = await _context.Clienti.ToListAsync();
            
            var viewModel = new AnagraficheIndexViewModel
            {
                // Statistiche reali Studios
                StudiosAttivi = allStudios.Count(s => s.Attivo && !s.RiattivatoPerAnno.HasValue),
                StudiosCessati = allStudios.Count(s => !s.Attivo),
                StudiosRiattivati = allStudios.Count(s => s.RiattivatoPerAnno.HasValue),
                
                // Statistiche reali Programmi
                ProgrammiAttivi = allProgrammi.Count(p => p.Attivo && !p.RiattivatoPerAnno.HasValue),
                ProgrammiCessati = allProgrammi.Count(p => !p.Attivo),
                ProgrammiRiattivati = allProgrammi.Count(p => p.RiattivatoPerAnno.HasValue),
                
                // Statistiche reali Professionisti
                ProfessionistiAttivi = allProfessionisti.Count(p => p.Attivo),
                ProfessionistiCessati = allProfessionisti.Count(p => !p.Attivo),
                ProfessionistiRiattivati = allProfessionisti.Count(p => p.RiattivatoPerAnno.HasValue),
                
                // Statistiche reali Anni Fiscali
                AnniFiscaliAttivi = allAnniFiscali.Count(a => a.Attivo),
                AnniFiscaliCessati = allAnniFiscali.Count(a => !a.Attivo),
                AnnoFiscaleCorrente = allAnniFiscali.FirstOrDefault(a => a.AnnoCorrente)?.Anno.ToString() ?? "N/A",
                
                // Statistiche reali Clienti
                ClientiAttivi = allClienti.Count(c => c.Attivo),
                ClientiCessati = allClienti.Count(c => !c.Attivo),
                ClientiRiattivati = allClienti.Count(c => c.RiattivatoPerAnno.HasValue),
                
                // Statistiche reali Regimi Contabili
                RegimiContabiliAttivi = allRegimiContabili.Count(r => r.Attivo),
                RegimiContabiliCessati = allRegimiContabili.Count(r => !r.Attivo),
                RegimiContabiliRiattivati = allRegimiContabili.Count(r => r.RiattivatoPerAnno.HasValue),
                
                // Statistiche reali Tipologie INPS
                TipologieInpsAttive = allTipologieInps.Count(t => t.Attivo),
                TipologieInpsCessate = allTipologieInps.Count(t => !t.Attivo),
                TipologieInpsRiattivate = allTipologieInps.Count(t => t.RiattivatoPerAnno.HasValue)
            };
            
            return View(viewModel);
        }

        // Placeholder per funzionalità future
        public IActionResult Lista()
        {
            ViewData["PageTitle"] = "Lista Anagrafiche";
            ViewData["Message"] = "Questa sezione mostrerà la lista completa delle anagrafiche.";
            return View("Placeholder");
        }

        public IActionResult Nuovo()
        {
            ViewData["PageTitle"] = "Nuova Anagrafica";
            ViewData["Message"] = "Qui potrai aggiungere una nuova anagrafica al sistema.";
            return View("Placeholder");
        }

        public IActionResult Ricerca()
        {
            ViewData["PageTitle"] = "Ricerca Anagrafiche";
            ViewData["Message"] = "Funzionalità di ricerca avanzata per trovare anagrafiche specifiche.";
            return View("Placeholder");
        }
    }
}
