using ConsultingGroup.Models;

namespace ConsultingGroup.ViewModels
{
    public class UserPermissionsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        
        // SISTEMA PERMESSI SEMPLIFICATO
        public bool CanAccessGestioneClienti { get; set; } = true;
        public bool CanAccessDatiUtenzaRiservata { get; set; } = false;
        public bool CanAccessDatiUtenzaGenerale { get; set; } = true;
        public bool CanAccessAreaAmministrativa { get; set; } = false;
        public bool CanAccessAnagrafiche { get; set; } = true;
        public bool GestioneAttivita { get; set; } = true;

        public DateTime LastModified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;

        public static UserPermissionsViewModel FromUserPermissions(UserPermissions permissions, ApplicationUser user, string role)
        {
            return new UserPermissionsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Role = role,
                FullName = user.FullName,
                CanAccessGestioneClienti = permissions.CanAccessGestioneClienti,
                CanAccessDatiUtenzaRiservata = permissions.CanAccessDatiUtenzaRiservata,
                CanAccessDatiUtenzaGenerale = permissions.CanAccessDatiUtenzaGenerale,
                CanAccessAreaAmministrativa = permissions.CanAccessAreaAmministrativa,
                CanAccessAnagrafiche = permissions.CanAccessAnagrafiche,
                GestioneAttivita = permissions.GestioneAttivita,
                LastModified = permissions.UpdatedAt,
                ModifiedBy = permissions.ModifiedBy
            };
        }
    }

    public class UserPermissionsListViewModel
    {
        public List<UserPermissionsViewModel> Users { get; set; } = new();
        public string SearchRole { get; set; } = string.Empty;
        public string SearchName { get; set; } = string.Empty;
    }
}