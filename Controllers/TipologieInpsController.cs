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
    public class TipologieInpsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipologieInpsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipologieInps
        public async Task<IActionResult> Index(string searchString = "", int page = 1, int pageSize = 10)
        {
            ViewBag.CurrentFilter = searchString;

            var tipologieQuery = _context.TipologieInps
                .Include(t => t.AnnoFiscaleRiattivazione)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                tipologieQuery = tipologieQuery.Where(t => 
                    t.Tipologia.Contains(searchString));
            }

            // Statistiche
            var allTipologie = await _context.TipologieInps.ToListAsync();
            ViewBag.TotaleTipologie = allTipologie.Count;
            ViewBag.TipologieAttive = allTipologie.Count(t => t.Attivo);
            ViewBag.TipologieCessate = allTipologie.Count(t => !t.Attivo);
            ViewBag.TipologieRiattivate = allTipologie.Count(t => t.RiattivatoPerAnno.HasValue);

            // Paginazione
            var totalItems = await tipologieQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var tipologie = await tipologieQuery
                .OrderByDescending(t => t.DataAttivazione)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(tipologie);
        }

        // GET: TipologieInps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipologiaInps = await _context.TipologieInps
                .Include(t => t.AnnoFiscaleRiattivazione)
                .FirstOrDefaultAsync(t => t.IdTipologiaInps == id);

            if (tipologiaInps == null)
            {
                return NotFound();
            }

            return View(tipologiaInps);
        }

        // GET: TipologieInps/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new TipologieInpsViewModel();
            await PopulateAnniFiscaliDropdown(viewModel);
            return View(viewModel);
        }

        // POST: TipologieInps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipologieInpsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tipologiaInps = new TipologiaInps
                {
                    Tipologia = viewModel.Tipologia,
                    Attivo = true,
                    DataAttivazione = DateTime.UtcNow,
                    DataModifica = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RiattivatoPerAnno = viewModel.RiattivatoPerAnno,
                    DataRiattivazione = viewModel.RiattivatoPerAnno.HasValue ? DateTime.UtcNow : null
                };

                _context.TipologieInps.Add(tipologiaInps);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Tipologia INPS '{tipologiaInps.Tipologia}' creata con successo.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateAnniFiscaliDropdown(viewModel);
            return View(viewModel);
        }

        // GET: TipologieInps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipologiaInps = await _context.TipologieInps.FindAsync(id);
            if (tipologiaInps == null)
            {
                return NotFound();
            }

            var viewModel = new TipologieInpsViewModel
            {
                IdTipologiaInps = tipologiaInps.IdTipologiaInps,
                Tipologia = tipologiaInps.Tipologia,
                Attivo = tipologiaInps.Attivo,
                DataAttivazione = tipologiaInps.DataAttivazione,
                DataModifica = tipologiaInps.DataModifica,
                DataCessazione = tipologiaInps.DataCessazione,
                RiattivatoPerAnno = tipologiaInps.RiattivatoPerAnno,
                DataRiattivazione = tipologiaInps.DataRiattivazione
            };

            await PopulateAnniFiscaliDropdown(viewModel);
            return View(viewModel);
        }

        // POST: TipologieInps/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipologieInpsViewModel viewModel)
        {
            if (id != viewModel.IdTipologiaInps)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tipologiaInps = await _context.TipologieInps.FindAsync(id);
                    if (tipologiaInps == null)
                    {
                        return NotFound();
                    }

                    tipologiaInps.Tipologia = viewModel.Tipologia;
                    tipologiaInps.DataModifica = DateTime.UtcNow;
                    tipologiaInps.UpdatedAt = DateTime.UtcNow;
                    tipologiaInps.RiattivatoPerAnno = viewModel.RiattivatoPerAnno;

                    // Se è stato impostato un anno di riattivazione e non era già impostato
                    if (viewModel.RiattivatoPerAnno.HasValue && !tipologiaInps.DataRiattivazione.HasValue)
                    {
                        tipologiaInps.DataRiattivazione = DateTime.UtcNow;
                    }
                    // Se è stato rimosso l'anno di riattivazione
                    else if (!viewModel.RiattivatoPerAnno.HasValue)
                    {
                        tipologiaInps.DataRiattivazione = null;
                    }

                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Tipologia INPS '{tipologiaInps.Tipologia}' modificata con successo.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipologiaInpsExists(viewModel.IdTipologiaInps))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateAnniFiscaliDropdown(viewModel);
            return View(viewModel);
        }

        // POST: TipologieInps/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipologiaInps = await _context.TipologieInps.FindAsync(id);
            if (tipologiaInps != null)
            {
                var tipologiaNome = tipologiaInps.Tipologia;
                _context.TipologieInps.Remove(tipologiaInps);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Tipologia INPS '{tipologiaNome}' eliminata con successo.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: TipologieInps/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var tipologiaInps = await _context.TipologieInps.FindAsync(id);
            if (tipologiaInps == null)
            {
                return NotFound();
            }

            if (tipologiaInps.Attivo)
            {
                // Cessazione
                tipologiaInps.Attivo = false;
                tipologiaInps.DataCessazione = DateTime.UtcNow;
                tipologiaInps.DataModifica = DateTime.UtcNow;
                tipologiaInps.UpdatedAt = DateTime.UtcNow;
                
                TempData["SuccessMessage"] = $"Tipologia INPS '{tipologiaInps.Tipologia}' cessata con successo.";
            }
            else
            {
                // Riattivazione
                tipologiaInps.Attivo = true;
                tipologiaInps.DataCessazione = null;
                tipologiaInps.DataModifica = DateTime.UtcNow;
                tipologiaInps.UpdatedAt = DateTime.UtcNow;
                
                TempData["SuccessMessage"] = $"Tipologia INPS '{tipologiaInps.Tipologia}' riattivata con successo.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipologiaInpsExists(int id)
        {
            return _context.TipologieInps.Any(e => e.IdTipologiaInps == id);
        }

        private async Task PopulateAnniFiscaliDropdown(TipologieInpsViewModel viewModel)
        {
            var anniFiscali = await _context.AnniFiscali
                .Where(a => a.Attivo)
                .OrderByDescending(a => a.Anno)
                .Select(a => new SelectListItem
                {
                    Value = a.IdAnno.ToString(),
                    Text = $"{a.Anno} - {a.Descrizione}",
                    Selected = false
                })
                .ToListAsync();

            viewModel.AnniFiscaliDisponibili = anniFiscali;
        }
    }
}

