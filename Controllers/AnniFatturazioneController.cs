using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Attributes;
using ConsultingGroup.Models;
using ConsultingGroup.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("anagrafiche")]
    public class AnniFatturazioneController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnniFatturazioneController> _logger;

        public AnniFatturazioneController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<AnniFatturazioneController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: AnniFatturazione
        public async Task<IActionResult> Index(string filtroStato = "tutti", string ricerca = "")
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            // Ottieni anni fatturazione dal database
            var anniFatturazioneQuery = _context.AnniFatturazione.AsQueryable();

            // Applica filtri
            switch (filtroStato.ToLower())
            {
                case "corrente":
                    anniFatturazioneQuery = anniFatturazioneQuery.Where(a => a.AnnoCorrente);
                    break;
                case "attivi":
                    anniFatturazioneQuery = anniFatturazioneQuery.Where(a => a.Attivo);
                    break;
                case "non_attivi":
                    anniFatturazioneQuery = anniFatturazioneQuery.Where(a => !a.Attivo);
                    break;
                default: // tutti
                    break;
            }

            if (!string.IsNullOrWhiteSpace(ricerca))
            {
                anniFatturazioneQuery = anniFatturazioneQuery.Where(a => 
                    a.Anno.ToString().Contains(ricerca) || 
                    (a.Descrizione != null && a.Descrizione.Contains(ricerca)) ||
                    (a.Note != null && a.Note.Contains(ricerca)));
            }

            var anniFatturazione = await anniFatturazioneQuery.OrderByDescending(a => a.Anno).ToListAsync();

            ViewBag.FiltroStato = filtroStato;
            ViewBag.Ricerca = ricerca;

            return View(anniFatturazione);
        }

        // GET: AnniFatturazione/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annoFatturazione = await _context.AnniFatturazione
                .FirstOrDefaultAsync(m => m.IdAnnoFatturazione == id);

            if (annoFatturazione == null)
            {
                return NotFound();
            }

            return View(annoFatturazione);
        }

        // GET: AnniFatturazione/Create
        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (_userManager.GetRolesAsync(user!).Result).FirstOrDefault() ?? "User";

            return View();
        }

        // POST: AnniFatturazione/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Anno,Descrizione,Attivo,AnnoCorrente,Note")] AnnoFatturazione annoFatturazione)
        {
            if (ModelState.IsValid)
            {
                // Verifica che l'anno non esista già
                var annoEsistente = await _context.AnniFatturazione
                    .FirstOrDefaultAsync(a => a.Anno == annoFatturazione.Anno);

                if (annoEsistente != null)
                {
                    ModelState.AddModelError("Anno", $"L'anno {annoFatturazione.Anno} esiste già.");
                    return View(annoFatturazione);
                }

                // Se viene impostato come anno corrente, rimuovi il flag dagli altri
                if (annoFatturazione.AnnoCorrente)
                {
                    var altriAnniCorrente = await _context.AnniFatturazione
                        .Where(a => a.AnnoCorrente)
                        .ToListAsync();

                    foreach (var anno in altriAnniCorrente)
                    {
                        anno.AnnoCorrente = false;
                        anno.UpdatedAt = DateTime.UtcNow;
                    }
                }

                annoFatturazione.CreatedAt = DateTime.UtcNow;
                annoFatturazione.UpdatedAt = DateTime.UtcNow;

                _context.Add(annoFatturazione);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Anno fatturazione {annoFatturazione.Anno} creato con successo.";
                return RedirectToAction(nameof(Index));
            }

            return View(annoFatturazione);
        }

        // GET: AnniFatturazione/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annoFatturazione = await _context.AnniFatturazione.FindAsync(id);
            if (annoFatturazione == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            return View(annoFatturazione);
        }

        // POST: AnniFatturazione/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnnoFatturazione,Anno,Descrizione,Attivo,AnnoCorrente,Note")] AnnoFatturazione annoFatturazione)
        {
            if (id != annoFatturazione.IdAnnoFatturazione)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica che l'anno non esista già (escludendo quello corrente)
                    var annoEsistente = await _context.AnniFatturazione
                        .FirstOrDefaultAsync(a => a.Anno == annoFatturazione.Anno && a.IdAnnoFatturazione != id);

                    if (annoEsistente != null)
                    {
                        ModelState.AddModelError("Anno", $"L'anno {annoFatturazione.Anno} esiste già.");
                        return View(annoFatturazione);
                    }

                    // Se viene impostato come anno corrente, rimuovi il flag dagli altri
                    if (annoFatturazione.AnnoCorrente)
                    {
                        var altriAnniCorrente = await _context.AnniFatturazione
                            .Where(a => a.AnnoCorrente && a.IdAnnoFatturazione != id)
                            .ToListAsync();

                        foreach (var anno in altriAnniCorrente)
                        {
                            anno.AnnoCorrente = false;
                            anno.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    annoFatturazione.UpdatedAt = DateTime.UtcNow;
                    _context.Update(annoFatturazione);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Anno fatturazione {annoFatturazione.Anno} aggiornato con successo.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnoFatturazioneExists(annoFatturazione.IdAnnoFatturazione))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(annoFatturazione);
        }

        // GET: AnniFatturazione/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var annoFatturazione = await _context.AnniFatturazione
                .FirstOrDefaultAsync(m => m.IdAnnoFatturazione == id);

            if (annoFatturazione == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            ViewData["UserName"] = user?.FullName ?? "Utente";
            ViewData["UserRole"] = (await _userManager.GetRolesAsync(user!)).FirstOrDefault() ?? "User";

            return View(annoFatturazione);
        }

        // POST: AnniFatturazione/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var annoFatturazione = await _context.AnniFatturazione.FindAsync(id);
            if (annoFatturazione != null)
            {
                // Verifica se è l'anno corrente
                if (annoFatturazione.AnnoCorrente)
                {
                    TempData["ErrorMessage"] = "Non è possibile eliminare l'anno corrente di fatturazione.";
                    return RedirectToAction(nameof(Index));
                }

                _context.AnniFatturazione.Remove(annoFatturazione);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Anno fatturazione {annoFatturazione.Anno} eliminato con successo.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AnniFatturazione/SetCorrente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetCorrente(int id)
        {
            var annoFatturazione = await _context.AnniFatturazione.FindAsync(id);
            if (annoFatturazione == null)
            {
                return NotFound();
            }

            // Rimuovi il flag corrente da tutti gli anni
            var altriAnniCorrente = await _context.AnniFatturazione
                .Where(a => a.AnnoCorrente)
                .ToListAsync();

            foreach (var anno in altriAnniCorrente)
            {
                anno.AnnoCorrente = false;
                anno.UpdatedAt = DateTime.UtcNow;
            }

            // Imposta questo come anno corrente
            annoFatturazione.AnnoCorrente = true;
            annoFatturazione.Attivo = true; // Se viene impostato come corrente, deve essere attivo
            annoFatturazione.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Anno {annoFatturazione.Anno} impostato come anno corrente di fatturazione.";
            return RedirectToAction(nameof(Index));
        }

        private bool AnnoFatturazioneExists(int id)
        {
            return _context.AnniFatturazione.Any(e => e.IdAnnoFatturazione == id);
        }
    }
}
