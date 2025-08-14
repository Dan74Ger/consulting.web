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
    public class ProfessionistiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfessionistiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Professionisti
        public async Task<IActionResult> Index()
        {
            var professionisti = await _context.Professionisti
                .Include(p => p.AnnoFiscaleRiattivazione)
                .OrderByDescending(p => p.DataAttivazione)
                .Select(p => new ProfessionistiViewModel
                {
                    IdProfessionista = p.IdProfessionista,
                    Nome = p.Nome,
                    Cognome = p.Cognome,
                    Attivo = p.Attivo,
                    DataAttivazione = p.DataAttivazione,
                    DataModifica = p.DataModifica,
                    DataCessazione = p.DataCessazione,
                    RiattivatoPerAnno = p.RiattivatoPerAnno,
                    DataRiattivazione = p.DataRiattivazione,
                    AnnoFiscaleDescrizione = p.AnnoFiscaleRiattivazione != null ? 
                        $"{p.AnnoFiscaleRiattivazione.Anno} - {p.AnnoFiscaleRiattivazione.Descrizione}" : null
                })
                .ToListAsync();

            return View(professionisti);
        }

        // GET: Professionisti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professionista = await _context.Professionisti
                .Include(p => p.AnnoFiscaleRiattivazione)
                .FirstOrDefaultAsync(p => p.IdProfessionista == id);

            if (professionista == null)
            {
                return NotFound();
            }

            var viewModel = new ProfessionistiViewModel
            {
                IdProfessionista = professionista.IdProfessionista,
                Nome = professionista.Nome,
                Cognome = professionista.Cognome,
                Attivo = professionista.Attivo,
                DataAttivazione = professionista.DataAttivazione,
                DataModifica = professionista.DataModifica,
                DataCessazione = professionista.DataCessazione,
                RiattivatoPerAnno = professionista.RiattivatoPerAnno,
                DataRiattivazione = professionista.DataRiattivazione,
                AnnoFiscaleDescrizione = professionista.AnnoFiscaleRiattivazione != null ? 
                    $"{professionista.AnnoFiscaleRiattivazione.Anno} - {professionista.AnnoFiscaleRiattivazione.Descrizione}" : null
            };

            return View(viewModel);
        }

        // GET: Professionisti/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProfessionistiViewModel();
            await LoadAnniFiscaliDisponibili(viewModel);
            return View(viewModel);
        }

        // POST: Professionisti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfessionistiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var professionista = new Professionista
                {
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    Attivo = true,
                    DataAttivazione = DateTime.UtcNow,
                    DataModifica = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RiattivatoPerAnno = model.RiattivatoPerAnno
                };

                if (model.RiattivatoPerAnno.HasValue)
                {
                    professionista.DataRiattivazione = DateTime.UtcNow;
                }

                _context.Professionisti.Add(professionista);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Professionista creato con successo!";
                return RedirectToAction(nameof(Index));
            }

            await LoadAnniFiscaliDisponibili(model);
            return View(model);
        }

        // GET: Professionisti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var professionista = await _context.Professionisti.FindAsync(id);
            if (professionista == null)
            {
                return NotFound();
            }

            var viewModel = new ProfessionistiViewModel
            {
                IdProfessionista = professionista.IdProfessionista,
                Nome = professionista.Nome,
                Cognome = professionista.Cognome,
                Attivo = professionista.Attivo,
                DataAttivazione = professionista.DataAttivazione,
                DataModifica = professionista.DataModifica,
                DataCessazione = professionista.DataCessazione,
                RiattivatoPerAnno = professionista.RiattivatoPerAnno,
                DataRiattivazione = professionista.DataRiattivazione
            };

            await LoadAnniFiscaliDisponibili(viewModel);
            return View(viewModel);
        }

        // POST: Professionisti/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProfessionistiViewModel model)
        {
            if (id != model.IdProfessionista)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var professionista = await _context.Professionisti.FindAsync(id);
                    if (professionista == null)
                    {
                        return NotFound();
                    }

                    professionista.Nome = model.Nome;
                    professionista.Cognome = model.Cognome;
                    professionista.DataModifica = DateTime.UtcNow;
                    professionista.UpdatedAt = DateTime.UtcNow;

                    // Gestione riattivazione
                    if (model.RiattivatoPerAnno.HasValue && professionista.RiattivatoPerAnno != model.RiattivatoPerAnno)
                    {
                        professionista.RiattivatoPerAnno = model.RiattivatoPerAnno;
                        professionista.DataRiattivazione = DateTime.UtcNow;
                    }
                    else if (!model.RiattivatoPerAnno.HasValue)
                    {
                        professionista.RiattivatoPerAnno = null;
                        professionista.DataRiattivazione = null;
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Professionista aggiornato con successo!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfessionistaExists(model.IdProfessionista))
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

        // POST: Professionisti/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var professionista = await _context.Professionisti.FindAsync(id);
            if (professionista != null)
            {
                _context.Professionisti.Remove(professionista);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Professionista eliminato con successo!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Professionisti/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var professionista = await _context.Professionisti.FindAsync(id);
            if (professionista == null)
            {
                return NotFound();
            }

            professionista.Attivo = !professionista.Attivo;
            professionista.DataModifica = DateTime.UtcNow;
            professionista.UpdatedAt = DateTime.UtcNow;

            if (!professionista.Attivo)
            {
                professionista.DataCessazione = DateTime.UtcNow;
            }
            else
            {
                professionista.DataCessazione = null;
            }

            await _context.SaveChangesAsync();

            var message = professionista.Attivo ? "Professionista riattivato con successo!" : "Professionista cessato con successo!";
            TempData["SuccessMessage"] = message;

            return RedirectToAction(nameof(Index));
        }

        // POST: Professionisti/Riattiva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riattiva(int id, int annoFiscaleId)
        {
            var professionista = await _context.Professionisti.FindAsync(id);
            if (professionista == null)
            {
                return NotFound();
            }

            professionista.RiattivatoPerAnno = annoFiscaleId;
            professionista.DataRiattivazione = DateTime.UtcNow;
            professionista.DataModifica = DateTime.UtcNow;
            professionista.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Professionista riattivato per l'anno fiscale selezionato!";
            return RedirectToAction(nameof(Index));
        }

        private bool ProfessionistaExists(int id)
        {
            return _context.Professionisti.Any(e => e.IdProfessionista == id);
        }

        private async Task LoadAnniFiscaliDisponibili(ProfessionistiViewModel model)
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

