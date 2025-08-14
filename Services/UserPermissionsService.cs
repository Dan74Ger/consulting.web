using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Data;
using ConsultingGroup.Models;

namespace ConsultingGroup.Services
{
    public interface IUserPermissionsService
    {
        Task<UserPermissions?> GetUserPermissionsAsync(string userId);
        Task<bool> UserCanAccessAsync(string userId, string permission);
        Task<UserPermissions> GetOrCreateUserPermissionsAsync(string userId);
    }

    public class UserPermissionsService : IUserPermissionsService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPermissionsService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserPermissions?> GetUserPermissionsAsync(string userId)
        {
            return await _context.UserPermissions
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> UserCanAccessAsync(string userId, string permission)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            if (userPermissions == null)
            {
                return false;
            }

            // SISTEMA COMPLETO - 6 permessi essenziali
            return permission.ToLower() switch
            {
                "gestioneclienti" => userPermissions.CanAccessGestioneClienti,
                "datiutenzariservata" => userPermissions.CanAccessDatiUtenzaRiservata,
                "datiutenzagenerale" => userPermissions.CanAccessDatiUtenzaGenerale,
                "areaamministrativa" => userPermissions.CanAccessAreaAmministrativa,
                "anagrafiche" => userPermissions.CanAccessAnagrafiche,
                "gestioneattivita" => userPermissions.GestioneAttivita,
                _ => false // Tutti gli altri permessi vengono negati
            };
        }

        public async Task<UserPermissions> GetOrCreateUserPermissionsAsync(string userId)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            if (permissions != null)
            {
                return permissions;
            }

            // Crea permessi di default basati sul ruolo
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Utente non trovato", nameof(userId));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? "";

            permissions = new UserPermissions
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ModifiedBy = "System"
            };

            // LOGICA SEMPLIFICATA PER RUOLI
            if (primaryRole == "UserSenior")
            {
                // Senior: Può vedere tutto tranne gestire utenti (che è gestito via [Authorize(Roles="Administrator")])
                permissions.CanAccessGestioneClienti = true;
                permissions.CanAccessDatiUtenzaRiservata = true; // Senior può accedere a dati riservati
                permissions.CanAccessDatiUtenzaGenerale = true;
                permissions.CanAccessAreaAmministrativa = true; // Senior può accedere all'area amministrativa
                permissions.CanAccessAnagrafiche = true; // Senior può accedere alle anagrafiche
                permissions.GestioneAttivita = true; // Senior può accedere alle attività
            }
            else if (primaryRole == "User")
            {
                // User: Solo Gestione Attività + Dati Generali + Anagrafiche
                permissions.CanAccessGestioneClienti = true;
                permissions.CanAccessDatiUtenzaRiservata = false; // User NON può accedere a dati riservati
                permissions.CanAccessDatiUtenzaGenerale = true;
                permissions.CanAccessAreaAmministrativa = false; // User NON può accedere all'area amministrativa
                permissions.CanAccessAnagrafiche = true; // User può accedere alle anagrafiche
                permissions.GestioneAttivita = true; // User può accedere alle attività
            }
            else
            {
                // Administrator: Accesso completo a tutto
                permissions.CanAccessGestioneClienti = true;
                permissions.CanAccessDatiUtenzaRiservata = true;
                permissions.CanAccessDatiUtenzaGenerale = true;
                permissions.CanAccessAreaAmministrativa = true; // Administrator può accedere all'area amministrativa
                permissions.CanAccessAnagrafiche = true; // Administrator può accedere alle anagrafiche
                permissions.GestioneAttivita = true; // Administrator può accedere alle attività
            }

            _context.UserPermissions.Add(permissions);
            await _context.SaveChangesAsync();

            return permissions;
        }
    }
}