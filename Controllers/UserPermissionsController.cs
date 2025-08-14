using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Data;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;

namespace ConsultingGroup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserPermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserPermissionsController> _logger;

        public UserPermissionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserPermissionsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? searchRole = null, string? searchName = null)
        {
            var users = await _userManager.Users.ToListAsync();
            var viewModel = new UserPermissionsListViewModel
            {
                SearchRole = searchRole ?? "",
                SearchName = searchName ?? ""
            };

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault() ?? "";

                // Filtra solo User e UserSenior
                if (primaryRole != "User" && primaryRole != "UserSenior")
                    continue;

                // Applica filtri se specificati
                if (!string.IsNullOrEmpty(searchRole) && primaryRole != searchRole)
                    continue;

                if (!string.IsNullOrEmpty(searchName) && 
                    !user.FullName.Contains(searchName, StringComparison.OrdinalIgnoreCase) &&
                    !user.UserName!.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    continue;

                var permissions = await _context.UserPermissions
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (permissions == null)
                {
                    // Crea permessi di default basati sul ruolo
                    permissions = CreateDefaultPermissions(user.Id, primaryRole);
                    _context.UserPermissions.Add(permissions);
                    await _context.SaveChangesAsync();
                }

                viewModel.Users.Add(UserPermissionsViewModel.FromUserPermissions(permissions, user, primaryRole));
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "";

            // Verifica che sia User o UserSenior
            if (primaryRole != "User" && primaryRole != "UserSenior")
                return NotFound();

            var permissions = await _context.UserPermissions
                .FirstOrDefaultAsync(p => p.UserId == id);

            if (permissions == null)
            {
                permissions = CreateDefaultPermissions(id, primaryRole);
                _context.UserPermissions.Add(permissions);
                await _context.SaveChangesAsync();
            }

            var viewModel = UserPermissionsViewModel.FromUserPermissions(permissions, user, primaryRole);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserPermissionsViewModel model)
        {
            if (id != model.UserId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var permissions = await _context.UserPermissions
                    .FirstOrDefaultAsync(p => p.UserId == id);

                if (permissions == null)
                    return NotFound();

                // SISTEMA SEMPLIFICATO - Aggiorna i 5 permessi essenziali
                permissions.CanAccessGestioneClienti = model.CanAccessGestioneClienti;
                permissions.CanAccessDatiUtenzaRiservata = model.CanAccessDatiUtenzaRiservata;
                permissions.CanAccessDatiUtenzaGenerale = model.CanAccessDatiUtenzaGenerale;
                permissions.CanAccessAreaAmministrativa = model.CanAccessAreaAmministrativa;
                permissions.CanAccessAnagrafiche = model.CanAccessAnagrafiche;
                permissions.GestioneAttivita = model.GestioneAttivita;
                permissions.UpdatedAt = DateTime.UtcNow;
                permissions.ModifiedBy = User.Identity?.Name ?? "System";

                _context.Update(permissions);
                await _context.SaveChangesAsync();

                // Se l'utente modificato è quello corrente, pulisce la cache per aggiornare il menu immediatamente
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id == model.UserId)
                {
                    HttpContext.Session.Clear();
                }

                TempData["SuccessMessage"] = $"Permessi aggiornati con successo per {model.FullName}. Ricarica la pagina per vedere le modifiche nel menu.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Errore durante l'aggiornamento. Riprova.");
                return View(model);
            }
        }

        public async Task<IActionResult> ResetToDefault(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "";

            if (primaryRole != "User" && primaryRole != "UserSenior")
                return NotFound();

            var permissions = await _context.UserPermissions
                .FirstOrDefaultAsync(p => p.UserId == id);

            if (permissions != null)
            {
                var defaultPermissions = CreateDefaultPermissions(id, primaryRole);
                
                // Copia i valori di default
                // SISTEMA SEMPLIFICATO - 5 permessi
                permissions.CanAccessGestioneClienti = defaultPermissions.CanAccessGestioneClienti;
                permissions.CanAccessDatiUtenzaRiservata = defaultPermissions.CanAccessDatiUtenzaRiservata;
                permissions.CanAccessDatiUtenzaGenerale = defaultPermissions.CanAccessDatiUtenzaGenerale;
                permissions.CanAccessAreaAmministrativa = defaultPermissions.CanAccessAreaAmministrativa;
                permissions.CanAccessAnagrafiche = defaultPermissions.CanAccessAnagrafiche;
                permissions.GestioneAttivita = defaultPermissions.GestioneAttivita;
                permissions.UpdatedAt = DateTime.UtcNow;
                permissions.ModifiedBy = User.Identity?.Name ?? "System";

                _context.Update(permissions);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Permessi ripristinati ai valori di default per {user.FullName}";
            return RedirectToAction(nameof(Index));
        }

        private UserPermissions CreateDefaultPermissions(string userId, string role)
        {
            var permissions = new UserPermissions
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ModifiedBy = User.Identity?.Name ?? "System"
            };

            // LOGICA SEMPLIFICATA
            if (role == "UserSenior")
            {
                permissions.CanAccessGestioneClienti = true;
                permissions.CanAccessDatiUtenzaRiservata = true; // Senior può accedere a dati riservati
                permissions.CanAccessDatiUtenzaGenerale = true;
                permissions.CanAccessAreaAmministrativa = true; // Senior può accedere all'area amministrativa
                permissions.CanAccessAnagrafiche = true; // Senior può accedere alle anagrafiche
                permissions.GestioneAttivita = true; // Senior può accedere alle attività
            }
            else // User
            {
                permissions.CanAccessGestioneClienti = true;
                permissions.CanAccessDatiUtenzaRiservata = false; // User NON può accedere a dati riservati
                permissions.CanAccessDatiUtenzaGenerale = true;
                permissions.CanAccessAreaAmministrativa = false; // User NON può accedere all'area amministrativa
                permissions.CanAccessAnagrafiche = true; // User può accedere alle anagrafiche
                permissions.GestioneAttivita = true; // User può accedere alle attività
            }

            return permissions;
        }

        // Metodo temporaneo per sistemare i permessi GestioneAttivita per tutti gli utenti
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> FixGestioneAttivitaPermissions()
        {
            try
            {
                var usersWithoutPermission = await _context.UserPermissions
                    .Where(up => up.GestioneAttivita == false)
                    .ToListAsync();

                foreach (var permission in usersWithoutPermission)
                {
                    permission.GestioneAttivita = true;
                    permission.UpdatedAt = DateTime.UtcNow;
                    permission.ModifiedBy = "System_Fix";
                }

                await _context.SaveChangesAsync();

                // Forza il refresh del ViewComponent per tutti gli utenti
                HttpContext.Session.Clear();

                TempData["SuccessMessage"] = $"Sistemati i permessi GestioneAttivita per {usersWithoutPermission.Count} utenti. Ricarica la pagina per vedere le modifiche.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Errore nell'aggiornamento dei permessi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}