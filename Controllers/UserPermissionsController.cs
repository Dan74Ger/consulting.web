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

                // SISTEMA SEMPLIFICATO - Aggiorna solo i 3 permessi essenziali
                permissions.CanAccessGestioneClienti = model.CanAccessGestioneClienti;
                permissions.CanAccessDatiUtenzaRiservata = model.CanAccessDatiUtenzaRiservata;
                permissions.CanAccessDatiUtenzaGenerale = model.CanAccessDatiUtenzaGenerale;
                permissions.UpdatedAt = DateTime.UtcNow;
                permissions.ModifiedBy = User.Identity?.Name ?? "System";

                _context.Update(permissions);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Permessi aggiornati con successo per {model.FullName}";
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
                // SISTEMA SEMPLIFICATO - Solo 3 permessi
                permissions.CanAccessGestioneClienti = defaultPermissions.CanAccessGestioneClienti;
                permissions.CanAccessDatiUtenzaRiservata = defaultPermissions.CanAccessDatiUtenzaRiservata;
                permissions.CanAccessDatiUtenzaGenerale = defaultPermissions.CanAccessDatiUtenzaGenerale;
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
            }
            else // User
            {
                permissions.CanAccessGestioneClienti = true;
                permissions.CanAccessDatiUtenzaRiservata = false; // User NON può accedere a dati riservati
                permissions.CanAccessDatiUtenzaGenerale = true;
            }

            return permissions;
        }
    }
}