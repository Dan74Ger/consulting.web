using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class StudioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StudioController> _logger;

        public StudioController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<StudioController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: Studio
        public async Task<IActionResult> Index(string filtroStato = "tutti", string ricerca = "")
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            // Ottieni studios dal database
            var studiosQuery = _context.Studios.AsQueryable();

            // Applica filtri
            switch (filtroStato.ToLower())
            {
                case "attivi":
                    studiosQuery = studiosQuery.Where(s => s.Attivo && !s.RiattivatoPerAnno.HasValue);
                    break;
                case "cessati":
                    studiosQuery = studiosQuery.Where(s => !s.Attivo);
                    break;
                case "riattivati":
                    studiosQuery = studiosQuery.Where(s => s.RiattivatoPerAnno.HasValue);
                    break;
                default: // tutti
                    break;
            }

            if (!string.IsNullOrWhiteSpace(ricerca))
            {
                studiosQuery = studiosQuery.Where(s => s.NomeStudio.Contains(ricerca));
            }

            var studios = await studiosQuery.OrderBy(s => s.NomeStudio).ToListAsync();

            // Converti in ViewModel
            var studioViewModels = studios.Select(s => new StudioViewModel
            {
                Id = s.IdStudio,
                NomeStudio = s.NomeStudio,
                Attivo = s.Attivo,
                DataAttivazione = s.DataAttivazione,
                DataModifica = s.DataModifica,
                DataCessazione = s.DataCessazione,
                RiattivatoperAnno = s.RiattivatoPerAnno,
                DataRiattivazione = s.DataRiattivazione,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            var allStudios = await _context.Studios.ToListAsync();
            var viewModel = new StudioIndexViewModel
            {
                Studios = studioViewModels,
                TotaleAttivi = allStudios.Count(s => s.Attivo && !s.RiattivatoPerAnno.HasValue),
                TotaleCessati = allStudios.Count(s => !s.Attivo),
                TotaleRiattivati = allStudios.Count(s => s.RiattivatoPerAnno.HasValue),
                FiltroStato = filtroStato,
                Ricerca = ricerca
            };

            return View(viewModel);
        }

        // GET: Studio/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }

            var studioViewModel = new StudioViewModel
            {
                Id = studio.IdStudio,
                NomeStudio = studio.NomeStudio,
                Attivo = studio.Attivo,
                DataAttivazione = studio.DataAttivazione,
                DataModifica = studio.DataModifica,
                DataCessazione = studio.DataCessazione,
                RiattivatoperAnno = studio.RiattivatoPerAnno,
                DataRiattivazione = studio.DataRiattivazione,
                CreatedAt = studio.CreatedAt,
                UpdatedAt = studio.UpdatedAt
            };

            return View(studioViewModel);
        }

        // GET: Studio/Create
        public IActionResult Create()
        {
            var viewModel = new StudioCreateEditViewModel
            {
                Attivo = true,
                IsEdit = false,
                AnniFiscali = GetAnniFiscaliMock()
            };

            return View("CreateEdit", viewModel);
        }

        // POST: Studio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudioCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentDateTime = DateTime.UtcNow;
                
                var studio = new Studio
                {
                    NomeStudio = model.NomeStudio,
                    Attivo = model.Attivo,
                    RiattivatoPerAnno = model.RiattivatoperAnno,
                    // Impostiamo le date automaticamente
                    DataAttivazione = currentDateTime,
                    DataModifica = currentDateTime,
                    CreatedAt = currentDateTime,
                    UpdatedAt = currentDateTime
                };

                _context.Studios.Add(studio);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Creazione Studio: {model.NomeStudio} (ID: {studio.IdStudio})");
                
                TempData["SuccessMessage"] = $"Studio '{model.NomeStudio}' creato con successo!";
                return RedirectToAction(nameof(Index));
            }

            model.AnniFiscali = GetAnniFiscaliMock();
            return View("CreateEdit", model);
        }

        // GET: Studio/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }

            var viewModel = new StudioCreateEditViewModel
            {
                Id = studio.IdStudio,
                NomeStudio = studio.NomeStudio,
                Attivo = studio.Attivo,
                RiattivatoperAnno = studio.RiattivatoPerAnno,
                IsEdit = true,
                AnniFiscali = GetAnniFiscaliMock()
            };

            return View("CreateEdit", viewModel);
        }

        // POST: Studio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudioCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var studio = await _context.Studios.FindAsync(id);
                    if (studio == null)
                    {
                        return NotFound();
                    }

                    studio.NomeStudio = model.NomeStudio;
                    studio.Attivo = model.Attivo;
                    studio.RiattivatoPerAnno = model.RiattivatoperAnno;
                    // Impostiamo le date di modifica automaticamente
                    studio.DataModifica = DateTime.UtcNow;
                    studio.UpdatedAt = DateTime.UtcNow;

                    _context.Update(studio);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Modifica Studio ID {id}: {model.NomeStudio}");
                    
                    TempData["SuccessMessage"] = $"Studio '{model.NomeStudio}' modificato con successo!";
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

        // POST: Studio/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }

            _context.Studios.Remove(studio);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Eliminazione Studio ID {id}: {studio.NomeStudio}");
            
            TempData["SuccessMessage"] = $"Studio '{studio.NomeStudio}' eliminato con successo!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Studio/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }

            studio.Attivo = !studio.Attivo;
            // Impostiamo le date di modifica automaticamente
            studio.DataModifica = DateTime.UtcNow;
            studio.UpdatedAt = DateTime.UtcNow;
            
            // Impostiamo DataCessazione se stiamo disattivando
            if (!studio.Attivo)
            {
                studio.DataCessazione = DateTime.UtcNow;
            }
            else
            {
                // Se riattivato, rimuoviamo la data di cessazione
                studio.DataCessazione = null;
            }

            _context.Update(studio);
            await _context.SaveChangesAsync();

            var azione = studio.Attivo ? "attivato" : "disattivato";
            _logger.LogInformation($"Cambio stato Studio ID {id}: {azione}");
            
            TempData["SuccessMessage"] = $"Studio '{studio.NomeStudio}' {azione} con successo!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Studio/Riattiva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riattiva(int id, int annoFiscale)
        {
            var studio = await _context.Studios.FindAsync(id);
            if (studio == null)
            {
                return NotFound();
            }

            var currentDateTime = DateTime.UtcNow;
            
            // Riattiva lo studio
            studio.Attivo = true;
            studio.RiattivatoPerAnno = annoFiscale;
            studio.DataRiattivazione = currentDateTime;
            studio.DataModifica = currentDateTime;
            studio.UpdatedAt = currentDateTime;
            studio.DataCessazione = null; // Rimuovi la data di cessazione

            _context.Update(studio);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Riattivazione Studio ID {id} per anno fiscale {annoFiscale}");
            
            TempData["SuccessMessage"] = $"Studio '{studio.NomeStudio}' riattivato per l'anno fiscale {annoFiscale}!";
            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods

        private List<SelectListItem> GetAnniFiscaliMock()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "2024", Text = "2024" },
                new SelectListItem { Value = "2025", Text = "2025", Selected = true },
                new SelectListItem { Value = "2026", Text = "2026" }
            };
        }

        #endregion
    }
}
