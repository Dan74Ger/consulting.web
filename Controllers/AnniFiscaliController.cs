using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Attributes;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("anagrafiche")]
    public class AnniFiscaliController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnniFiscaliController> _logger;

        public AnniFiscaliController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<AnniFiscaliController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: AnniFiscali
        public async Task<IActionResult> Index(string filtroStato = "tutti", string ricerca = "")
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            // Ottieni anni fiscali dal database
            var anniFiscaliQuery = _context.AnniFiscali.OrderBy(a => a.Anno).AsQueryable();

            // Applica filtri
            switch (filtroStato.ToLower())
            {
                case "corrente":
                    anniFiscaliQuery = anniFiscaliQuery.Where(a => a.AnnoCorrente);
                    break;
                case "con_scadenze":
                    anniFiscaliQuery = anniFiscaliQuery.Where(a => 
                        a.Scadenza730.HasValue || a.Scadenza740.HasValue || a.Scadenza750.HasValue || 
                        a.Scadenza760.HasValue || a.ScadenzaENC.HasValue || a.ScadenzaIRAP.HasValue || 
                        a.Scadenza770.HasValue || a.ScadenzaCU.HasValue || a.ScadenzaDIVA.HasValue || 
                        a.ScadenzaLipe1t.HasValue || a.ScadenzaLipe2t.HasValue || a.ScadenzaLipe3t.HasValue || 
                        a.ScadenzaLipe4t.HasValue || a.ScadenzaModTRIva1t.HasValue || a.ScadenzaModTRIva2t.HasValue || 
                        a.ScadenzaModTRIva3t.HasValue || a.ScadenzaModTRIva4t.HasValue);
                    break;
                case "senza_scadenze":
                    anniFiscaliQuery = anniFiscaliQuery.Where(a => 
                        !a.Scadenza730.HasValue && !a.Scadenza740.HasValue && !a.Scadenza750.HasValue && 
                        !a.Scadenza760.HasValue && !a.ScadenzaENC.HasValue && !a.ScadenzaIRAP.HasValue && 
                        !a.Scadenza770.HasValue && !a.ScadenzaCU.HasValue && !a.ScadenzaDIVA.HasValue && 
                        !a.ScadenzaLipe1t.HasValue && !a.ScadenzaLipe2t.HasValue && !a.ScadenzaLipe3t.HasValue && 
                        !a.ScadenzaLipe4t.HasValue && !a.ScadenzaModTRIva1t.HasValue && !a.ScadenzaModTRIva2t.HasValue && 
                        !a.ScadenzaModTRIva3t.HasValue && !a.ScadenzaModTRIva4t.HasValue);
                    break;
                default: // tutti
                    break;
            }

            if (!string.IsNullOrWhiteSpace(ricerca))
            {
                anniFiscaliQuery = anniFiscaliQuery.Where(a => 
                    a.Anno.ToString().Contains(ricerca) || 
                    (a.Descrizione != null && a.Descrizione.Contains(ricerca)));
            }

            var anniFiscali = await anniFiscaliQuery.OrderByDescending(a => a.Anno).ToListAsync();

            // Converti in ViewModel
            var anniFiscaliViewModels = anniFiscali.Select(a => new AnniFiscaliViewModel
            {
                Id = a.IdAnno,
                Anno = a.Anno,
                Descrizione = a.Descrizione,
                Attivo = a.Attivo,
                AnnoCorrente = a.AnnoCorrente,
                Scadenza730 = a.Scadenza730,
                Scadenza740 = a.Scadenza740,
                Scadenza750 = a.Scadenza750,
                Scadenza760 = a.Scadenza760,
                ScadenzaENC = a.ScadenzaENC,
                ScadenzaIRAP = a.ScadenzaIRAP,
                Scadenza770 = a.Scadenza770,
                ScadenzaCU = a.ScadenzaCU,
                ScadenzaDIVA = a.ScadenzaDIVA,
                ScadenzaLipe1t = a.ScadenzaLipe1t,
                ScadenzaLipe2t = a.ScadenzaLipe2t,
                ScadenzaLipe3t = a.ScadenzaLipe3t,
                ScadenzaLipe4t = a.ScadenzaLipe4t,
                ScadenzaModTRIva1t = a.ScadenzaModTRIva1t,
                ScadenzaModTRIva2t = a.ScadenzaModTRIva2t,
                ScadenzaModTRIva3t = a.ScadenzaModTRIva3t,
                ScadenzaModTRIva4t = a.ScadenzaModTRIva4t,
                Note = a.Note,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

            var allAnniFiscali = await _context.AnniFiscali.ToListAsync();
            var viewModel = new AnniFiscaliIndexViewModel
            {
                AnniFiscali = anniFiscaliViewModels,
                TotaleAttivi = allAnniFiscali.Count(a => a.Attivo),
                TotaleNonAttivi = allAnniFiscali.Count(a => !a.Attivo),
                AnnoCorrente = allAnniFiscali.FirstOrDefault(a => a.AnnoCorrente)?.Anno,
                FiltroStato = filtroStato,
                Ricerca = ricerca
            };

            return View(viewModel);
        }

        // GET: AnniFiscali/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var annoFiscale = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.IdAnno == id);
            if (annoFiscale == null)
            {
                return NotFound();
            }

            var annoFiscaleViewModel = new AnniFiscaliViewModel
            {
                Id = annoFiscale.IdAnno,
                Anno = annoFiscale.Anno,
                Descrizione = annoFiscale.Descrizione,
                Attivo = annoFiscale.Attivo,
                AnnoCorrente = annoFiscale.AnnoCorrente,
                Scadenza730 = annoFiscale.Scadenza730,
                Scadenza740 = annoFiscale.Scadenza740,
                Scadenza750 = annoFiscale.Scadenza750,
                Scadenza760 = annoFiscale.Scadenza760,
                ScadenzaENC = annoFiscale.ScadenzaENC,
                ScadenzaIRAP = annoFiscale.ScadenzaIRAP,
                Scadenza770 = annoFiscale.Scadenza770,
                ScadenzaCU = annoFiscale.ScadenzaCU,
                ScadenzaDIVA = annoFiscale.ScadenzaDIVA,
                ScadenzaLipe1t = annoFiscale.ScadenzaLipe1t,
                ScadenzaLipe2t = annoFiscale.ScadenzaLipe2t,
                ScadenzaLipe3t = annoFiscale.ScadenzaLipe3t,
                ScadenzaLipe4t = annoFiscale.ScadenzaLipe4t,
                ScadenzaModTRIva1t = annoFiscale.ScadenzaModTRIva1t,
                ScadenzaModTRIva2t = annoFiscale.ScadenzaModTRIva2t,
                ScadenzaModTRIva3t = annoFiscale.ScadenzaModTRIva3t,
                ScadenzaModTRIva4t = annoFiscale.ScadenzaModTRIva4t,
                Note = annoFiscale.Note,
                CreatedAt = annoFiscale.CreatedAt,
                UpdatedAt = annoFiscale.UpdatedAt
            };

            return View(annoFiscaleViewModel);
        }

        // GET: AnniFiscali/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var model = new AnniFiscaliCreateEditViewModel
            {
                Attivo = true,
                Anno = DateTime.Now.Year
            };

            return View("CreateEdit", model);
        }



        // GET: AnniFiscali/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            var annoFiscale = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.IdAnno == id);
            if (annoFiscale == null)
            {
                return NotFound();
            }

            var model = new AnniFiscaliCreateEditViewModel
            {
                Id = annoFiscale.IdAnno,
                Anno = annoFiscale.Anno,
                Descrizione = annoFiscale.Descrizione,
                Attivo = annoFiscale.Attivo,
                AnnoCorrente = annoFiscale.AnnoCorrente,
                Scadenza730 = annoFiscale.Scadenza730,
                Scadenza740 = annoFiscale.Scadenza740,
                Scadenza750 = annoFiscale.Scadenza750,
                Scadenza760 = annoFiscale.Scadenza760,
                ScadenzaENC = annoFiscale.ScadenzaENC,
                ScadenzaIRAP = annoFiscale.ScadenzaIRAP,
                Scadenza770 = annoFiscale.Scadenza770,
                ScadenzaCU = annoFiscale.ScadenzaCU,
                ScadenzaDIVA = annoFiscale.ScadenzaDIVA,
                ScadenzaLipe1t = annoFiscale.ScadenzaLipe1t,
                ScadenzaLipe2t = annoFiscale.ScadenzaLipe2t,
                ScadenzaLipe3t = annoFiscale.ScadenzaLipe3t,
                ScadenzaLipe4t = annoFiscale.ScadenzaLipe4t,
                ScadenzaModTRIva1t = annoFiscale.ScadenzaModTRIva1t,
                ScadenzaModTRIva2t = annoFiscale.ScadenzaModTRIva2t,
                ScadenzaModTRIva3t = annoFiscale.ScadenzaModTRIva3t,
                ScadenzaModTRIva4t = annoFiscale.ScadenzaModTRIva4t,
                Note = annoFiscale.Note
            };

            return View("CreateEdit", model);
        }







        // POST: AnniFiscali/SetCorrente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCorrente(int id)
        {
            var annoFiscale = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.IdAnno == id);
            if (annoFiscale == null)
            {
                return NotFound();
            }

            // TODO: Impostare come anno corrente nel database (rimuovendo il flag dagli altri)
            
            _logger.LogInformation($"Impostazione Anno Corrente ID {id}: {annoFiscale.Anno}");
            
            TempData["SuccessMessage"] = $"Anno Fiscale '{annoFiscale.Anno}' impostato come corrente!";
            return RedirectToAction(nameof(Index));
        }

        // POST: AnniFiscali/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnniFiscaliCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Controlla se l'anno esiste già
                    var annoEsistente = await _context.AnniFiscali
                        .AnyAsync(a => a.Anno == model.Anno);
                    
                    if (annoEsistente)
                    {
                        ModelState.AddModelError("Anno", $"ATTENZIONE: L'anno {model.Anno} esiste già nel database! Non è possibile creare un duplicato. Puoi solo MODIFICARE l'anno esistente oppure ELIMINARLO e poi ricrearlo.");
                        ViewData["UserName"] = (await _userManager.GetUserAsync(User))?.FullName ?? "Utente";
                        var currentUser = await _userManager.GetUserAsync(User);
                        ViewData["UserRole"] = currentUser != null ? 
                            (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault() ?? "User" : "User";
                        return View("CreateEdit", model);
                    }

                    // Se richiesto, duplica le scadenze dall'anno precedente
                    if (model.DuplicaScadenzeAnnoPrecedente)
                    {
                        var annoPrecedente = await _context.AnniFiscali
                            .Where(a => a.Anno < model.Anno)
                            .OrderByDescending(a => a.Anno)
                            .FirstOrDefaultAsync();

                        if (annoPrecedente != null)
                        {
                            // Duplica le scadenze aggiornando l'anno
                            model.Scadenza730 = annoPrecedente.Scadenza730?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.Scadenza740 = annoPrecedente.Scadenza740?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.Scadenza750 = annoPrecedente.Scadenza750?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.Scadenza760 = annoPrecedente.Scadenza760?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.Scadenza770 = annoPrecedente.Scadenza770?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaENC = annoPrecedente.ScadenzaENC?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaIRAP = annoPrecedente.ScadenzaIRAP?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaCU = annoPrecedente.ScadenzaCU?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaDIVA = annoPrecedente.ScadenzaDIVA?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaLipe1t = annoPrecedente.ScadenzaLipe1t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaLipe2t = annoPrecedente.ScadenzaLipe2t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaLipe3t = annoPrecedente.ScadenzaLipe3t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaLipe4t = annoPrecedente.ScadenzaLipe4t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaModTRIva1t = annoPrecedente.ScadenzaModTRIva1t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaModTRIva2t = annoPrecedente.ScadenzaModTRIva2t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaModTRIva3t = annoPrecedente.ScadenzaModTRIva3t?.AddYears(model.Anno - annoPrecedente.Anno);
                            model.ScadenzaModTRIva4t = annoPrecedente.ScadenzaModTRIva4t?.AddYears(model.Anno - annoPrecedente.Anno);
                        }
                    }

                    var annoFiscale = new AnnoFiscale
                    {
                        Anno = model.Anno,
                        Descrizione = model.Descrizione,
                        Attivo = model.Attivo,
                        AnnoCorrente = model.AnnoCorrente,
                        Scadenza730 = model.Scadenza730,
                        Scadenza740 = model.Scadenza740,
                        Scadenza750 = model.Scadenza750,
                        Scadenza760 = model.Scadenza760,
                        ScadenzaENC = model.ScadenzaENC,
                        ScadenzaIRAP = model.ScadenzaIRAP,
                        Scadenza770 = model.Scadenza770,
                        ScadenzaCU = model.ScadenzaCU,
                        ScadenzaDIVA = model.ScadenzaDIVA,
                        ScadenzaLipe1t = model.ScadenzaLipe1t,
                        ScadenzaLipe2t = model.ScadenzaLipe2t,
                        ScadenzaLipe3t = model.ScadenzaLipe3t,
                        ScadenzaLipe4t = model.ScadenzaLipe4t,
                        ScadenzaModTRIva1t = model.ScadenzaModTRIva1t,
                        ScadenzaModTRIva2t = model.ScadenzaModTRIva2t,
                        ScadenzaModTRIva3t = model.ScadenzaModTRIva3t,
                        ScadenzaModTRIva4t = model.ScadenzaModTRIva4t,
                        Note = model.Note,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Se è impostato come anno corrente, disattiva gli altri
                    if (model.AnnoCorrente)
                    {
                        var altriAnniCorrenti = await _context.AnniFiscali
                            .Where(a => a.AnnoCorrente)
                            .ToListAsync();
                        
                        foreach (var anno in altriAnniCorrenti)
                        {
                            anno.AnnoCorrente = false;
                        }
                    }

                    _context.AnniFiscali.Add(annoFiscale);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Anno fiscale creato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la creazione dell'anno fiscale");
                    ModelState.AddModelError("", "Errore durante la creazione dell'anno fiscale.");
                }
            }

            return View(model);
        }

        // POST: AnniFiscali/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnniFiscaliCreateEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var annoFiscale = await _context.AnniFiscali.FindAsync(id);
                    if (annoFiscale == null)
                    {
                        return NotFound();
                    }

                    // Controlla se l'anno che si sta tentando di impostare esiste già (e non è l'anno corrente)
                    if (model.Anno != annoFiscale.Anno)
                    {
                        var annoEsistente = await _context.AnniFiscali
                            .AnyAsync(a => a.Anno == model.Anno && a.IdAnno != id);
                        
                        if (annoEsistente)
                        {
                            ModelState.AddModelError("Anno", $"ATTENZIONE: L'anno {model.Anno} esiste già nel database! Non è possibile modificare verso un anno duplicato. Scegli un anno diverso oppure ELIMINA l'anno esistente {model.Anno} prima di procedere.");
                            ViewData["UserName"] = (await _userManager.GetUserAsync(User))?.FullName ?? "Utente";
                            var currentUser = await _userManager.GetUserAsync(User);
                            ViewData["UserRole"] = currentUser != null ? 
                                (await _userManager.GetRolesAsync(currentUser)).FirstOrDefault() ?? "User" : "User";
                            return View("CreateEdit", model);
                        }
                    }

                    // Se è impostato come anno corrente, disattiva gli altri
                    if (model.AnnoCorrente && !annoFiscale.AnnoCorrente)
                    {
                        var altriAnniCorrenti = await _context.AnniFiscali
                            .Where(a => a.AnnoCorrente && a.IdAnno != id)
                            .ToListAsync();
                        
                        foreach (var anno in altriAnniCorrenti)
                        {
                            anno.AnnoCorrente = false;
                        }
                    }

                    // Aggiorna i campi
                    annoFiscale.Anno = model.Anno;
                    annoFiscale.Descrizione = model.Descrizione;
                    annoFiscale.Attivo = model.Attivo;
                    annoFiscale.AnnoCorrente = model.AnnoCorrente;
                    annoFiscale.Scadenza730 = model.Scadenza730;
                    annoFiscale.Scadenza740 = model.Scadenza740;
                    annoFiscale.Scadenza750 = model.Scadenza750;
                    annoFiscale.Scadenza760 = model.Scadenza760;
                    annoFiscale.ScadenzaENC = model.ScadenzaENC;
                    annoFiscale.ScadenzaIRAP = model.ScadenzaIRAP;
                    annoFiscale.Scadenza770 = model.Scadenza770;
                    annoFiscale.ScadenzaCU = model.ScadenzaCU;
                    annoFiscale.ScadenzaDIVA = model.ScadenzaDIVA;
                    annoFiscale.ScadenzaLipe1t = model.ScadenzaLipe1t;
                    annoFiscale.ScadenzaLipe2t = model.ScadenzaLipe2t;
                    annoFiscale.ScadenzaLipe3t = model.ScadenzaLipe3t;
                    annoFiscale.ScadenzaLipe4t = model.ScadenzaLipe4t;
                    annoFiscale.ScadenzaModTRIva1t = model.ScadenzaModTRIva1t;
                    annoFiscale.ScadenzaModTRIva2t = model.ScadenzaModTRIva2t;
                    annoFiscale.ScadenzaModTRIva3t = model.ScadenzaModTRIva3t;
                    annoFiscale.ScadenzaModTRIva4t = model.ScadenzaModTRIva4t;
                    annoFiscale.Note = model.Note;
                    annoFiscale.UpdatedAt = DateTime.UtcNow;

                    _context.Update(annoFiscale);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Anno fiscale aggiornato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await AnnoFiscaleExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'aggiornamento dell'anno fiscale {Id}", id);
                    ModelState.AddModelError("", "Errore durante l'aggiornamento dell'anno fiscale.");
                }
            }

            return View(model);
        }

        // POST: AnniFiscali/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var annoFiscale = await _context.AnniFiscali.FindAsync(id);
                if (annoFiscale != null)
                {
                    _context.AnniFiscali.Remove(annoFiscale);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Anno fiscale eliminato con successo!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'anno fiscale {Id}", id);
                TempData["ErrorMessage"] = "Errore durante l'eliminazione dell'anno fiscale.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AnniFiscali/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var annoFiscale = await _context.AnniFiscali.FindAsync(id);
                if (annoFiscale == null)
                {
                    return NotFound();
                }

                annoFiscale.Attivo = !annoFiscale.Attivo;
                annoFiscale.UpdatedAt = DateTime.UtcNow;

                _context.Update(annoFiscale);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Anno fiscale {(annoFiscale.Attivo ? "attivato" : "disattivato")} con successo!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato dell'anno fiscale {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il cambio stato dell'anno fiscale.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AnniFiscali/SetAnnoCorrente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAnnoCorrente(int id)
        {
            try
            {
                var annoFiscale = await _context.AnniFiscali.FindAsync(id);
                if (annoFiscale == null)
                {
                    return NotFound();
                }

                // Disattiva tutti gli altri anni correnti
                var altriAnniCorrenti = await _context.AnniFiscali
                    .Where(a => a.AnnoCorrente && a.IdAnno != id)
                    .ToListAsync();
                
                foreach (var anno in altriAnniCorrenti)
                {
                    anno.AnnoCorrente = false;
                }

                // Imposta questo come anno corrente
                annoFiscale.AnnoCorrente = true;
                annoFiscale.UpdatedAt = DateTime.UtcNow;

                _context.UpdateRange(altriAnniCorrenti);
                _context.Update(annoFiscale);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Anno {annoFiscale.Anno} impostato come anno corrente!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'impostazione dell'anno corrente {Id}", id);
                TempData["ErrorMessage"] = "Errore durante l'impostazione dell'anno corrente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // API: Ottieni scadenze anno precedente per duplicazione
        [HttpGet]
        public async Task<IActionResult> GetScadenzeAnnoPrecedente(int annoCorrente)
        {
            try
            {
                var annoPrecedente = await _context.AnniFiscali
                    .Where(a => a.Anno < annoCorrente)
                    .OrderByDescending(a => a.Anno)
                    .FirstOrDefaultAsync();

                if (annoPrecedente == null)
                {
                    return Json(new { success = false, message = "Nessun anno precedente trovato" });
                }

                var scadenze = new
                {
                    success = true,
                    anno = annoPrecedente.Anno,
                    scadenze = new
                    {
                        scadenza730 = annoPrecedente.Scadenza730?.ToString("yyyy-MM-dd"),
                        scadenza740 = annoPrecedente.Scadenza740?.ToString("yyyy-MM-dd"),
                        scadenza750 = annoPrecedente.Scadenza750?.ToString("yyyy-MM-dd"),
                        scadenza760 = annoPrecedente.Scadenza760?.ToString("yyyy-MM-dd"),
                        scadenza770 = annoPrecedente.Scadenza770?.ToString("yyyy-MM-dd"),
                        scadenzaENC = annoPrecedente.ScadenzaENC?.ToString("yyyy-MM-dd"),
                        scadenzaIRAP = annoPrecedente.ScadenzaIRAP?.ToString("yyyy-MM-dd"),
                        scadenzaCU = annoPrecedente.ScadenzaCU?.ToString("yyyy-MM-dd"),
                        scadenzaDRIVA = annoPrecedente.ScadenzaDIVA?.ToString("yyyy-MM-dd"),
                        scadenzaLipe1t = annoPrecedente.ScadenzaLipe1t?.ToString("yyyy-MM-dd"),
                        scadenzaLipe2t = annoPrecedente.ScadenzaLipe2t?.ToString("yyyy-MM-dd"),
                        scadenzaLipe3t = annoPrecedente.ScadenzaLipe3t?.ToString("yyyy-MM-dd"),
                        scadenzaLipe4t = annoPrecedente.ScadenzaLipe4t?.ToString("yyyy-MM-dd")
                    }
                };

                return Json(scadenze);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze dell'anno precedente");
                return Json(new { success = false, message = "Errore durante il recupero dei dati" });
            }
        }

        #region Helper Methods

        private async Task<bool> AnnoFiscaleExists(int id)
        {
            return await _context.AnniFiscali.AnyAsync(e => e.IdAnno == id);
        }

        #endregion
    }
}
