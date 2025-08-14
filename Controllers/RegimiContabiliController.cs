using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Data;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("anagrafiche")]
    public class RegimiContabiliController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegimiContabiliController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RegimiContabili
        public async Task<IActionResult> Index()
        {
            var regimiContabili = await _context.RegimiContabili
                .Include(r => r.AnnoFiscaleRiattivazione)
                .OrderByDescending(r => r.DataAttivazione)
                .Select(r => new RegimiContabiliViewModel
                {
                    IdRegimeContabile = r.IdRegimeContabile,
                    NomeRegime = r.NomeRegime,
                    Attivo = r.Attivo,
                    DataAttivazione = r.DataAttivazione,
                    DataModifica = r.DataModifica,
                    DataCessazione = r.DataCessazione,
                    RiattivatoPerAnno = r.RiattivatoPerAnno,
                    DataRiattivazione = r.DataRiattivazione,
                    AnnoFiscaleDescrizione = r.AnnoFiscaleRiattivazione != null ? 
                        $"{r.AnnoFiscaleRiattivazione.Anno} - {r.AnnoFiscaleRiattivazione.Descrizione}" : null
                })
                .ToListAsync();

            return View(regimiContabili);
        }

        // GET: RegimiContabili/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimeContabile = await _context.RegimiContabili
                .Include(r => r.AnnoFiscaleRiattivazione)
                .FirstOrDefaultAsync(r => r.IdRegimeContabile == id);

            if (regimeContabile == null)
            {
                return NotFound();
            }

            var viewModel = new RegimiContabiliViewModel
            {
                IdRegimeContabile = regimeContabile.IdRegimeContabile,
                NomeRegime = regimeContabile.NomeRegime,
                Attivo = regimeContabile.Attivo,
                DataAttivazione = regimeContabile.DataAttivazione,
                DataModifica = regimeContabile.DataModifica,
                DataCessazione = regimeContabile.DataCessazione,
                RiattivatoPerAnno = regimeContabile.RiattivatoPerAnno,
                DataRiattivazione = regimeContabile.DataRiattivazione,
                AnnoFiscaleDescrizione = regimeContabile.AnnoFiscaleRiattivazione != null ? 
                    $"{regimeContabile.AnnoFiscaleRiattivazione.Anno} - {regimeContabile.AnnoFiscaleRiattivazione.Descrizione}" : null
            };

            return View(viewModel);
        }

        // GET: RegimiContabili/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new RegimiContabiliViewModel();
            await LoadAnniFiscaliDisponibili(viewModel);
            return View(viewModel);
        }

        // POST: RegimiContabili/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegimiContabiliViewModel model)
        {
            if (ModelState.IsValid)
            {
                var regimeContabile = new RegimeContabile
                {
                    NomeRegime = model.NomeRegime,
                    Attivo = true,
                    DataAttivazione = DateTime.UtcNow,
                    DataModifica = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RiattivatoPerAnno = model.RiattivatoPerAnno
                };

                if (model.RiattivatoPerAnno.HasValue)
                {
                    regimeContabile.DataRiattivazione = DateTime.UtcNow;
                }

                _context.RegimiContabili.Add(regimeContabile);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Regime contabile creato con successo!";
                return RedirectToAction(nameof(Index));
            }

            await LoadAnniFiscaliDisponibili(model);
            return View(model);
        }

        // GET: RegimiContabili/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimeContabile = await _context.RegimiContabili.FindAsync(id);
            if (regimeContabile == null)
            {
                return NotFound();
            }

            var viewModel = new RegimiContabiliViewModel
            {
                IdRegimeContabile = regimeContabile.IdRegimeContabile,
                NomeRegime = regimeContabile.NomeRegime,
                Attivo = regimeContabile.Attivo,
                DataAttivazione = regimeContabile.DataAttivazione,
                DataModifica = regimeContabile.DataModifica,
                DataCessazione = regimeContabile.DataCessazione,
                RiattivatoPerAnno = regimeContabile.RiattivatoPerAnno,
                DataRiattivazione = regimeContabile.DataRiattivazione
            };

            await LoadAnniFiscaliDisponibili(viewModel);
            return View(viewModel);
        }

        // POST: RegimiContabili/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegimiContabiliViewModel model)
        {
            if (id != model.IdRegimeContabile)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var regimeContabile = await _context.RegimiContabili.FindAsync(id);
                    if (regimeContabile == null)
                    {
                        return NotFound();
                    }

                    regimeContabile.NomeRegime = model.NomeRegime;
                    regimeContabile.DataModifica = DateTime.UtcNow;
                    regimeContabile.UpdatedAt = DateTime.UtcNow;

                    // Gestione riattivazione
                    if (model.RiattivatoPerAnno.HasValue && regimeContabile.RiattivatoPerAnno != model.RiattivatoPerAnno)
                    {
                        regimeContabile.RiattivatoPerAnno = model.RiattivatoPerAnno;
                        regimeContabile.DataRiattivazione = DateTime.UtcNow;
                    }
                    else if (!model.RiattivatoPerAnno.HasValue)
                    {
                        regimeContabile.RiattivatoPerAnno = null;
                        regimeContabile.DataRiattivazione = null;
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Regime contabile aggiornato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegimeContabileExists(model.IdRegimeContabile))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await LoadAnniFiscaliDisponibili(model);
            return View(model);
        }

        // POST: RegimiContabili/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var regimeContabile = await _context.RegimiContabili.FindAsync(id);
            if (regimeContabile != null)
            {
                _context.RegimiContabili.Remove(regimeContabile);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Regime contabile eliminato con successo!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RegimiContabili/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var regimeContabile = await _context.RegimiContabili.FindAsync(id);
            if (regimeContabile == null)
            {
                return NotFound();
            }

            regimeContabile.Attivo = !regimeContabile.Attivo;
            regimeContabile.DataModifica = DateTime.UtcNow;
            regimeContabile.UpdatedAt = DateTime.UtcNow;

            if (!regimeContabile.Attivo)
            {
                regimeContabile.DataCessazione = DateTime.UtcNow;
            }
            else
            {
                regimeContabile.DataCessazione = null;
            }

            await _context.SaveChangesAsync();

            var message = regimeContabile.Attivo ? "Regime contabile riattivato con successo!" : "Regime contabile cessato con successo!";
            TempData["SuccessMessage"] = message;

            return RedirectToAction(nameof(Index));
        }

        // POST: RegimiContabili/Riattiva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riattiva(int id, int annoFiscaleId)
        {
            var regimeContabile = await _context.RegimiContabili.FindAsync(id);
            if (regimeContabile == null)
            {
                return NotFound();
            }

            regimeContabile.RiattivatoPerAnno = annoFiscaleId;
            regimeContabile.DataRiattivazione = DateTime.UtcNow;
            regimeContabile.DataModifica = DateTime.UtcNow;
            regimeContabile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Regime contabile riattivato per l'anno fiscale selezionato!";
            return RedirectToAction(nameof(Index));
        }

        private bool RegimeContabileExists(int id)
        {
            return _context.RegimiContabili.Any(e => e.IdRegimeContabile == id);
        }

        private async Task LoadAnniFiscaliDisponibili(RegimiContabiliViewModel model)
        {
            var anniFiscali = await _context.AnniFiscali
                .Where(a => a.Attivo)
                .OrderByDescending(a => a.Anno)
                .Select(a => new SelectListItem
                {
                    Value = a.IdAnno.ToString(),
                    Text = $"{a.Anno} - {a.Descrizione}",
                    Selected = model.RiattivatoPerAnno == a.IdAnno
                })
                .ToListAsync();

            // Aggiungi opzione vuota
            anniFiscali.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Seleziona Anno Fiscale --",
                Selected = !model.RiattivatoPerAnno.HasValue
            });

            model.AnniFiscaliDisponibili = anniFiscali;
        }
    }
}

