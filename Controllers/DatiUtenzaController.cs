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
    [UserPermission("datiutenzariservata")]
    public class DatiUtenzaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DatiUtenzaController> _logger;

        public DatiUtenzaController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DatiUtenzaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: DatiUtenza - Dashboard principale
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // DATI RISERVATI CONDIVISI - Admin e Senior vedono tutti i dati
            var viewModel = new DatiUtenzaViewModel
            {
                UserId = user.Id,
                UserName = user.UserName!,
                FullName = user.FullName,
                Banche = await _context.Banche.Include(b => b.User).ToListAsync(),
                CarteCredito = await _context.CarteCredito.Include(c => c.User).ToListAsync(),
                Utenze = await _context.Utenze.Include(u => u.User).ToListAsync(),
                Mail = await _context.Mail.Include(m => m.User).ToListAsync(),
                Cancelleria = new List<Cancelleria>(), // Non usato in Dati Riservati
                UtentiPC = new List<UtentiPC>(), // Non usato in Dati Riservati  
                AltriDati = new List<AltriDati>() // Non usato in Dati Riservati

            };

            return View(viewModel);
        }

        #region Banche
        public async Task<IActionResult> Banche()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // DATI RISERVATI CONDIVISI - Admin e Senior vedono tutti i dati
            var banche = await _context.Banche.Include(b => b.User).ToListAsync();
            return View(banche);
        }

        public IActionResult CreateBanca()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBanca([Bind("NomeBanca,IBAN,CodiceUtente,Password,Indirizzo,Note")] Banche model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Log per debugging
            _logger.LogInformation($"CreateBanca - NomeBanca: {model.NomeBanca}, IBAN: {model.IBAN}, CodiceUtente: {model.CodiceUtente}");
            
            if (!ModelState.IsValid)
            {
                // Log errori di validazione
                foreach (var error in ModelState)
                {
                    _logger.LogWarning($"Validation Error - Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View(model);
            }

            try
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.Banche.Add(model);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Banca salvata con successo - ID: {model.Id}");
                TempData["SuccessMessage"] = "Dati banca aggiunti con successo.";
                return RedirectToAction(nameof(Banche));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio della banca");
                ModelState.AddModelError("", "Errore durante il salvataggio. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> EditBanca(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var banca = await _context.Banche.FirstOrDefaultAsync(b => b.Id == id);
            if (banca == null) return NotFound();

            return View(banca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBanca(int id, [Bind("Id,NomeBanca,IBAN,CodiceUtente,Password,Indirizzo,Note")] Banche model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var banca = await _context.Banche.FirstOrDefaultAsync(b => b.Id == id);
            if (banca == null) return NotFound();

            if (ModelState.IsValid)
            {
                banca.NomeBanca = model.NomeBanca;
                banca.IBAN = model.IBAN;
                banca.CodiceUtente = model.CodiceUtente;
                banca.Password = model.Password;
                banca.Indirizzo = model.Indirizzo;
                banca.Note = model.Note;
                banca.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dati banca aggiornati con successo.";
                return RedirectToAction(nameof(Banche));
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteBanca(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var banca = await _context.Banche.FirstOrDefaultAsync(b => b.Id == id);
            if (banca == null) return NotFound();

            return View(banca);
        }

        [HttpPost, ActionName("DeleteBanca")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBancaConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var banca = await _context.Banche.FirstOrDefaultAsync(b => b.Id == id);
            if (banca != null)
            {
                _context.Banche.Remove(banca);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Dati banca eliminati con successo.";
            }

            return RedirectToAction(nameof(Banche));
        }
        #endregion

        #region Carte di Credito
        public async Task<IActionResult> CarteCredito()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // DATI RISERVATI CONDIVISI - Admin e Senior vedono tutti i dati
            var carte = await _context.CarteCredito.Include(c => c.User).ToListAsync();
            return View(carte);
        }

        public IActionResult CreateCarta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCarta([Bind("NumeroCarta,Intestazione,MeseScadenza,AnnoScadenza,PIN,Note")] CarteCredito model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.CarteCredito.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Carta di credito aggiunta con successo.";
                return RedirectToAction(nameof(CarteCredito));
            }

            return View(model);
        }

        public async Task<IActionResult> EditCarta(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carta = await _context.CarteCredito.FirstOrDefaultAsync(c => c.Id == id);
            if (carta == null) return NotFound();

            return View(carta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCarta(int id, [Bind("Id,NumeroCarta,Intestazione,MeseScadenza,AnnoScadenza,PIN,Note")] CarteCredito model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var carta = await _context.CarteCredito.FirstOrDefaultAsync(c => c.Id == id);
            if (carta == null) return NotFound();

            if (ModelState.IsValid)
            {
                carta.NumeroCarta = model.NumeroCarta;
                carta.Intestazione = model.Intestazione;
                carta.MeseScadenza = model.MeseScadenza;
                carta.AnnoScadenza = model.AnnoScadenza;
                carta.PIN = model.PIN;
                carta.Note = model.Note;
                carta.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Carta di credito aggiornata con successo.";
                return RedirectToAction(nameof(CarteCredito));
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteCarta(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carta = await _context.CarteCredito.FirstOrDefaultAsync(c => c.Id == id);
            if (carta == null) return NotFound();

            return View(carta);
        }

        [HttpPost, ActionName("DeleteCarta")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCartaConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var carta = await _context.CarteCredito.FirstOrDefaultAsync(c => c.Id == id);
            if (carta != null)
            {
                _context.CarteCredito.Remove(carta);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Carta di credito eliminata con successo.";
            }

            return RedirectToAction(nameof(CarteCredito));
        }
        #endregion

        #region Utenze
        public async Task<IActionResult> Utenze()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // DATI RISERVATI CONDIVISI - Admin e Senior vedono tutti i dati
            var utenze = await _context.Utenze.Include(u => u.User).ToListAsync();
            return View(utenze);
        }

        public IActionResult CreateUtenza()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUtenza([Bind("DenominazioneUtenza,SitoWeb,NomeUtente,Password,Note")] Utenze model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.Utenze.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Utenza aggiunta con successo.";
                return RedirectToAction(nameof(Utenze));
            }

            return View(model);
        }

        public async Task<IActionResult> EditUtenza(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utenza = await _context.Utenze.FirstOrDefaultAsync(u => u.Id == id);
            if (utenza == null) return NotFound();

            return View(utenza);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUtenza(int id, [Bind("Id,DenominazioneUtenza,SitoWeb,NomeUtente,Password,Note")] Utenze model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var utenza = await _context.Utenze.FirstOrDefaultAsync(u => u.Id == id);
            if (utenza == null) return NotFound();

            if (ModelState.IsValid)
            {
                utenza.DenominazioneUtenza = model.DenominazioneUtenza;
                utenza.SitoWeb = model.SitoWeb;
                utenza.NomeUtente = model.NomeUtente;
                utenza.Password = model.Password;
                utenza.Note = model.Note;
                utenza.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utenza aggiornata con successo.";
                return RedirectToAction(nameof(Utenze));
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteUtenza(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utenza = await _context.Utenze.FirstOrDefaultAsync(u => u.Id == id);
            if (utenza == null) return NotFound();

            return View(utenza);
        }

        [HttpPost, ActionName("DeleteUtenza")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUtenzaConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var utenza = await _context.Utenze.FirstOrDefaultAsync(u => u.Id == id);
            if (utenza != null)
            {
                _context.Utenze.Remove(utenza);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Utenza eliminata con successo.";
            }

            return RedirectToAction(nameof(Utenze));
        }
        #endregion

        #region Mail
        public async Task<IActionResult> Mail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // DATI RISERVATI CONDIVISI - Admin e Senior vedono tutti i dati
            var mail = await _context.Mail.Include(m => m.User).ToListAsync();
            return View(mail);
        }

        public IActionResult CreateMail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMail([Bind("IndirizzoMail,NomeUtente,Password,Note")] Mail model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                _context.Mail.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account mail aggiunto con successo.";
                return RedirectToAction(nameof(Mail));
            }

            return View(model);
        }

        public async Task<IActionResult> EditMail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var mail = await _context.Mail.FirstOrDefaultAsync(m => m.Id == id);
            if (mail == null) return NotFound();

            return View(mail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMail(int id, [Bind("Id,IndirizzoMail,NomeUtente,Password,Note")] Mail model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (id != model.Id) return NotFound();

            var mail = await _context.Mail.FirstOrDefaultAsync(m => m.Id == id);
            if (mail == null) return NotFound();

            if (ModelState.IsValid)
            {
                mail.IndirizzoMail = model.IndirizzoMail;
                mail.NomeUtente = model.NomeUtente;
                mail.Password = model.Password;
                mail.Note = model.Note;
                mail.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account mail aggiornato con successo.";
                return RedirectToAction(nameof(Mail));
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteMail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var mail = await _context.Mail.FirstOrDefaultAsync(m => m.Id == id);
            if (mail == null) return NotFound();

            return View(mail);
        }

        [HttpPost, ActionName("DeleteMail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMailConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var mail = await _context.Mail.FirstOrDefaultAsync(m => m.Id == id);
            if (mail != null)
            {
                _context.Mail.Remove(mail);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Account mail eliminato con successo.";
            }

            return RedirectToAction(nameof(Mail));
        }
        #endregion


    }
}