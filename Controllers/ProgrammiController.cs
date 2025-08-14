using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ConsultingGroup.Attributes;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("anagrafiche")]
    public class ProgrammiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ProgrammiController> _logger;
        private readonly ApplicationDbContext _context;

        public ProgrammiController(UserManager<ApplicationUser> userManager, ILogger<ProgrammiController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        // GET: Programmi
        public async Task<IActionResult> Index(string filtroStato = "tutti", string ricerca = "")
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var programmiQuery = _context.Programmi.AsQueryable();

            // Filtri ricerca
            if (!string.IsNullOrEmpty(ricerca))
            {
                programmiQuery = programmiQuery.Where(p => 
                    p.NomeProgramma.Contains(ricerca));
            }

            // Filtri stato
            switch (filtroStato.ToLower())
            {
                case "attivi":
                    programmiQuery = programmiQuery.Where(p => p.Attivo && !p.RiattivatoPerAnno.HasValue);
                    break;
                case "cessati":
                    programmiQuery = programmiQuery.Where(p => !p.Attivo);
                    break;
                case "riattivati":
                    programmiQuery = programmiQuery.Where(p => p.RiattivatoPerAnno.HasValue);
                    break;
                // "tutti" non applica filtri
            }

            var programmi = await programmiQuery.OrderBy(p => p.NomeProgramma).ToListAsync();

            var programmiViewModels = programmi.Select(p => new ProgrammiViewModel
            {
                Id = p.IdProgramma,
                NomeProgramma = p.NomeProgramma,
                Attivo = p.Attivo,
                DataAttivazione = p.DataAttivazione,
                DataModifica = p.DataModifica,
                DataCessazione = p.DataCessazione,
                RiattivatoperAnno = p.RiattivatoPerAnno,
                DataRiattivazione = p.DataRiattivazione,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            var allProgrammi = await _context.Programmi.ToListAsync();
            var viewModel = new ProgrammiIndexViewModel
            {
                Programmi = programmiViewModels,
                TotaleAttivi = allProgrammi.Count(p => p.Attivo && !p.RiattivatoPerAnno.HasValue),
                TotaleCessati = allProgrammi.Count(p => !p.Attivo),
                TotaleRiattivati = allProgrammi.Count(p => p.RiattivatoPerAnno.HasValue),
                FiltroStato = filtroStato,
                Ricerca = ricerca
            };

            return View(viewModel);
        }

        // GET: Programmi/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var programma = await _context.Programmi.FindAsync(id);
            if (programma == null)
            {
                return NotFound();
            }

            var programmaViewModel = new ProgrammiViewModel
            {
                Id = programma.IdProgramma,
                NomeProgramma = programma.NomeProgramma,
                Attivo = programma.Attivo,
                DataAttivazione = programma.DataAttivazione,
                DataModifica = programma.DataModifica,
                DataCessazione = programma.DataCessazione,
                RiattivatoperAnno = programma.RiattivatoPerAnno,
                DataRiattivazione = programma.DataRiattivazione,
                CreatedAt = programma.CreatedAt,
                UpdatedAt = programma.UpdatedAt
            };

            return View(programmaViewModel);
        }

        // GET: Programmi/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var model = new ProgrammiCreateEditViewModel
            {
                AnniFiscali = GetAnniFiscaliMock()
            };

            return View("CreateEdit", model);
        }

        // POST: Programmi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProgrammiCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentDateTime = DateTime.UtcNow;
                
                var programma = new Programma
                {
                    NomeProgramma = model.NomeProgramma,
                    Attivo = model.Attivo,
                    RiattivatoPerAnno = model.RiattivatoperAnno,
                    // Impostiamo le date automaticamente
                    DataAttivazione = currentDateTime,
                    DataModifica = currentDateTime,
                    CreatedAt = currentDateTime,
                    UpdatedAt = currentDateTime
                };

                _context.Programmi.Add(programma);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Creazione Programma: {model.NomeProgramma} (ID: {programma.IdProgramma})");
                
                TempData["SuccessMessage"] = $"Programma '{model.NomeProgramma}' creato con successo!";
                return RedirectToAction(nameof(Index));
            }

            model.AnniFiscali = GetAnniFiscaliMock();
            return View("CreateEdit", model);
        }

        // GET: Programmi/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var programma = await _context.Programmi.FindAsync(id);
            if (programma == null)
            {
                return NotFound();
            }

            var model = new ProgrammiCreateEditViewModel
            {
                Id = programma.IdProgramma,
                NomeProgramma = programma.NomeProgramma,
                Attivo = programma.Attivo,
                RiattivatoperAnno = programma.RiattivatoPerAnno,
                AnniFiscali = GetAnniFiscaliMock()
            };

            return View("CreateEdit", model);
        }

        // POST: Programmi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProgrammiCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var programma = await _context.Programmi.FindAsync(id);
                    if (programma == null)
                    {
                        return NotFound();
                    }

                    programma.NomeProgramma = model.NomeProgramma;
                    programma.Attivo = model.Attivo;
                    programma.RiattivatoPerAnno = model.RiattivatoperAnno;
                    // Impostiamo le date di modifica automaticamente
                    programma.DataModifica = DateTime.UtcNow;
                    programma.UpdatedAt = DateTime.UtcNow;

                    _context.Update(programma);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Modifica Programma ID {id}: {model.NomeProgramma}");
                    
                    TempData["SuccessMessage"] = $"Programma '{model.NomeProgramma}' modificato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }

            model.AnniFiscali = GetAnniFiscaliMock();
            return View("CreateEdit", model);
        }

        // POST: Programmi/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var programma = await _context.Programmi.FindAsync(id);
            if (programma == null)
            {
                return NotFound();
            }

            programma.Attivo = !programma.Attivo;
            // Impostiamo le date di modifica automaticamente
            programma.DataModifica = DateTime.UtcNow;
            programma.UpdatedAt = DateTime.UtcNow;
            
            // Impostiamo DataCessazione se stiamo disattivando
            if (!programma.Attivo)
            {
                programma.DataCessazione = DateTime.UtcNow;
            }
            else
            {
                // Se riattivato, rimuoviamo la data di cessazione
                programma.DataCessazione = null;
            }

            _context.Update(programma);
            await _context.SaveChangesAsync();

            var azione = programma.Attivo ? "attivato" : "disattivato";
            _logger.LogInformation($"Cambio stato Programma ID {id}: {azione}");
            
            TempData["SuccessMessage"] = $"Programma '{programma.NomeProgramma}' {azione} con successo!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Programmi/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var programma = await _context.Programmi.FindAsync(id);
            if (programma == null)
            {
                return NotFound();
            }

            _context.Programmi.Remove(programma);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Eliminazione Programma ID {id}: {programma.NomeProgramma}");
            
            TempData["SuccessMessage"] = $"Programma '{programma.NomeProgramma}' eliminato con successo!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Programmi/Riattiva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riattiva(int id, int annoFiscale)
        {
            var programma = await _context.Programmi.FindAsync(id);
            if (programma == null)
            {
                return NotFound();
            }

            var currentDateTime = DateTime.UtcNow;
            
            // Riattiva il programma
            programma.Attivo = true;
            programma.RiattivatoPerAnno = annoFiscale;
            programma.DataRiattivazione = currentDateTime;
            programma.DataModifica = currentDateTime;
            programma.UpdatedAt = currentDateTime;
            programma.DataCessazione = null; // Rimuovi la data di cessazione

            _context.Update(programma);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Riattivazione Programma ID {id} per anno fiscale {annoFiscale}");
            
            TempData["SuccessMessage"] = $"Programma '{programma.NomeProgramma}' riattivato per l'anno fiscale {annoFiscale}!";
            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods

        private List<SelectListItem> GetAnniFiscaliMock()
        {
            var anniFiscali = new List<SelectListItem>();
            
            for (int anno = DateTime.Now.Year; anno >= DateTime.Now.Year - 5; anno--)
            {
                anniFiscali.Add(new SelectListItem
                {
                    Value = anno.ToString(),
                    Text = anno.ToString()
                });
            }
            
            return anniFiscali;
        }

        #endregion
    }
}
