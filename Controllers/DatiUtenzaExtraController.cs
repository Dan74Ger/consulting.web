using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Attributes;
using ConsultingGroup.Data;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("datiutenzagenerale")]
    public class DatiUtenzaExtraController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DatiUtenzaExtraController> _logger;

        public DatiUtenzaExtraController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DatiUtenzaExtraController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: DatiUtenzaExtra - Dashboard principale per Dati Generali
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // DATI GENERALI CONDIVISI - Tutti gli utenti autorizzati possono vedere e gestire questi dati
            var viewModel = new DatiUtenzaExtraViewModel
            {
                UserId = user.Id,
                UserName = user.UserName!,
                FullName = user.FullName,
                Cancelleria = await _context.Cancelleria.Include(c => c.User).ToListAsync(),
                UtentiPC = await _context.UtentiPC.Include(u => u.User).ToListAsync(),
                AltriDati = await _context.AltriDati.Include(a => a.User).ToListAsync(),
                Entratel = await _context.Entratel.Include(e => e.User).ToListAsync(),
                UtentiTS = await _context.UtentiTS.Include(u => u.User).ToListAsync()
            };

            return View(viewModel);
        }

        #region Cancelleria
        public async Task<IActionResult> Cancelleria()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // DATI GENERALI CONDIVISI - Tutti gli utenti vedono tutti i dati
            var cancelleria = await _context.Cancelleria.Include(c => c.User).ToListAsync();
            return View(cancelleria);
        }

        public IActionResult CreateCancelleria()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCancelleria([Bind("DenominazioneFornitore,SitoWeb,NomeUtente,Password,Note")] Cancelleria model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.Cancelleria.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dati cancelleria aggiunti con successo.";
                return RedirectToAction(nameof(Cancelleria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio dei dati cancelleria");
                ModelState.AddModelError("", "Errore durante il salvataggio. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> EditCancelleria(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cancelleria = await _context.Cancelleria.FirstOrDefaultAsync(c => c.Id == id);
            if (cancelleria == null) return NotFound();

            return View(cancelleria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCancelleria(int id, [Bind("Id,DenominazioneFornitore,SitoWeb,NomeUtente,Password,Note")] Cancelleria model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var cancelleria = await _context.Cancelleria.FirstOrDefaultAsync(c => c.Id == id);
            if (cancelleria == null) return NotFound();

            if (ModelState.IsValid)
            {
                cancelleria.DenominazioneFornitore = model.DenominazioneFornitore;
                cancelleria.SitoWeb = model.SitoWeb;
                cancelleria.NomeUtente = model.NomeUtente;
                cancelleria.Password = model.Password;
                cancelleria.Note = model.Note;
                cancelleria.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dati cancelleria aggiornati con successo.";
                return RedirectToAction(nameof(Cancelleria));
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteCancelleria(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cancelleria = await _context.Cancelleria.FirstOrDefaultAsync(c => c.Id == id);
            if (cancelleria == null) return NotFound();

            return View(cancelleria);
        }

        [HttpPost, ActionName("DeleteCancelleria")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCancelleriaConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var cancelleria = await _context.Cancelleria.FirstOrDefaultAsync(c => c.Id == id);
            if (cancelleria != null)
            {
                _context.Cancelleria.Remove(cancelleria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dati cancelleria eliminati con successo.";
            }

            return RedirectToAction(nameof(Cancelleria));
        }
        #endregion

        #region UtentiPC
        public async Task<IActionResult> UtentiPC()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utentiPC = await _context.UtentiPC.Include(u => u.User).ToListAsync();
            return View(utentiPC);
        }

        public IActionResult CreateUtentiPC()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUtentiPC([Bind("NomePC,Utente,Password,IndirizzoRete,Note")] UtentiPC model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.UtentiPC.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Utente PC aggiunto con successo.";
                return RedirectToAction(nameof(UtentiPC));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio dell'utente PC");
                ModelState.AddModelError("", "Errore durante il salvataggio. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> EditUtentiPC(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utentePC = await _context.UtentiPC.FirstOrDefaultAsync(u => u.Id == id);
            if (utentePC == null) return NotFound();

            return View(utentePC);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUtentiPC(int id, [Bind("Id,NomePC,Utente,Password,IndirizzoRete,Note")] UtentiPC model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var utentePC = await _context.UtentiPC.FirstOrDefaultAsync(u => u.Id == id);
            if (utentePC == null) return NotFound();

            if (ModelState.IsValid)
            {
                utentePC.NomePC = model.NomePC;
                utentePC.Utente = model.Utente;
                utentePC.Password = model.Password;
                utentePC.IndirizzoRete = model.IndirizzoRete;
                utentePC.Note = model.Note;
                utentePC.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utente PC aggiornato con successo.";
                return RedirectToAction(nameof(UtentiPC));
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteUtentiPC(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utentePC = await _context.UtentiPC.FirstOrDefaultAsync(u => u.Id == id);
            if (utentePC == null) return NotFound();

            return View(utentePC);
        }

        [HttpPost, ActionName("DeleteUtentiPC")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUtentiPCConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utentePC = await _context.UtentiPC.FirstOrDefaultAsync(u => u.Id == id);
            if (utentePC != null)
            {
                _context.UtentiPC.Remove(utentePC);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utente PC eliminato con successo.";
            }

            return RedirectToAction(nameof(UtentiPC));
        }
        #endregion

        #region AltriDati
        public async Task<IActionResult> AltriDati()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var altriDati = await _context.AltriDati.Include(a => a.User).ToListAsync();
            return View(altriDati);
        }

        public IActionResult CreateAltriDati()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAltriDati([Bind("Nome,SitoWeb,Utente,Password,Note")] AltriDati model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.AltriDati.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Altri dati aggiunti con successo.";
                return RedirectToAction(nameof(AltriDati));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio di altri dati");
                ModelState.AddModelError("", "Errore durante il salvataggio. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> EditAltriDati(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var altriDati = await _context.AltriDati.FirstOrDefaultAsync(a => a.Id == id);
            if (altriDati == null) return NotFound();

            return View(altriDati);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAltriDati(int id, [Bind("Id,Nome,SitoWeb,Utente,Password,Note")] AltriDati model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var altriDati = await _context.AltriDati.FirstOrDefaultAsync(a => a.Id == id);
            if (altriDati == null) return NotFound();

            if (ModelState.IsValid)
            {
                altriDati.Nome = model.Nome;
                altriDati.SitoWeb = model.SitoWeb;
                altriDati.Utente = model.Utente;
                altriDati.Password = model.Password;
                altriDati.Note = model.Note;
                altriDati.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Altri dati aggiornati con successo.";
                return RedirectToAction(nameof(AltriDati));
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteAltriDati(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var altriDati = await _context.AltriDati.FirstOrDefaultAsync(a => a.Id == id);
            if (altriDati == null) return NotFound();

            return View(altriDati);
        }

        [HttpPost, ActionName("DeleteAltriDati")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAltriDatiConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var altriDati = await _context.AltriDati.FirstOrDefaultAsync(a => a.Id == id);
            if (altriDati != null)
            {
                _context.AltriDati.Remove(altriDati);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Altri dati eliminati con successo.";
            }

            return RedirectToAction(nameof(AltriDati));
        }
        #endregion

        #region Entratel
        public async Task<IActionResult> Entratel()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var entratel = await _context.Entratel.Include(e => e.User).ToListAsync();
            return View(entratel);
        }

        public IActionResult CreateEntratel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEntratel([Bind("Sito,Utente,Password,PinDatiCatastali,PinDelegheCassettoFiscale,PinCompleto,DesktopTelematicoUtente,DesktopTelematicoPassword,Note")] Entratel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.Entratel.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dati Entratel aggiunti con successo.";
                return RedirectToAction(nameof(Entratel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio dei dati Entratel");
                ModelState.AddModelError("", "Errore durante il salvataggio. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> EditEntratel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var entratel = await _context.Entratel.FirstOrDefaultAsync(e => e.Id == id);
            if (entratel == null) return NotFound();

            return View(entratel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEntratel(int id, [Bind("Id,Sito,Utente,Password,PinDatiCatastali,PinDelegheCassettoFiscale,PinCompleto,DesktopTelematicoUtente,DesktopTelematicoPassword,Note")] Entratel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var entratel = await _context.Entratel.FirstOrDefaultAsync(e => e.Id == id);
            if (entratel == null) return NotFound();

            if (ModelState.IsValid)
            {
                entratel.Sito = model.Sito;
                entratel.Utente = model.Utente;
                entratel.Password = model.Password;
                entratel.PinDatiCatastali = model.PinDatiCatastali;
                entratel.PinDelegheCassettoFiscale = model.PinDelegheCassettoFiscale;
                entratel.PinCompleto = model.PinCompleto;
                entratel.DesktopTelematicoUtente = model.DesktopTelematicoUtente;
                entratel.DesktopTelematicoPassword = model.DesktopTelematicoPassword;
                entratel.Note = model.Note;
                entratel.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dati Entratel aggiornati con successo.";
                return RedirectToAction(nameof(Entratel));
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteEntratel(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var entratel = await _context.Entratel.FirstOrDefaultAsync(e => e.Id == id);
            if (entratel == null) return NotFound();

            return View(entratel);
        }

        [HttpPost, ActionName("DeleteEntratel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEntratelConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var entratel = await _context.Entratel.FirstOrDefaultAsync(e => e.Id == id);
            if (entratel != null)
            {
                _context.Entratel.Remove(entratel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dati Entratel eliminati con successo.";
            }

            return RedirectToAction(nameof(Entratel));
        }
        #endregion

        #region UtentiTS
        public async Task<IActionResult> UtentiTS()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utentiTS = await _context.UtentiTS.Include(u => u.User).ToListAsync();
            return View(utentiTS);
        }

        public IActionResult CreateUtentiTS()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUtentiTS([Bind("Nome,Utente,Password,Note")] UtentiTS model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.UtentiTS.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Utente TS aggiunto con successo.";
                return RedirectToAction(nameof(UtentiTS));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio dell'utente TS");
                ModelState.AddModelError("", "Errore durante il salvataggio. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> EditUtentiTS(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utenteTS = await _context.UtentiTS.FirstOrDefaultAsync(u => u.Id == id);
            if (utenteTS == null) return NotFound();

            return View(utenteTS);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUtentiTS(int id, [Bind("Id,Nome,Utente,Password,Note")] UtentiTS model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var utenteTS = await _context.UtentiTS.FirstOrDefaultAsync(u => u.Id == id);
            if (utenteTS == null) return NotFound();

            if (ModelState.IsValid)
            {
                utenteTS.Nome = model.Nome;
                utenteTS.Utente = model.Utente;
                utenteTS.Password = model.Password;
                utenteTS.Note = model.Note;
                utenteTS.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utente TS aggiornato con successo.";
                return RedirectToAction(nameof(UtentiTS));
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteUtentiTS(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utenteTS = await _context.UtentiTS.FirstOrDefaultAsync(u => u.Id == id);
            if (utenteTS == null) return NotFound();

            return View(utenteTS);
        }

        [HttpPost, ActionName("DeleteUtentiTS")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUtentiTSConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utenteTS = await _context.UtentiTS.FirstOrDefaultAsync(u => u.Id == id);
            if (utenteTS != null)
            {
                _context.UtentiTS.Remove(utenteTS);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utente TS eliminato con successo.";
            }

            return RedirectToAction(nameof(UtentiTS));
        }
        #endregion
    }
}