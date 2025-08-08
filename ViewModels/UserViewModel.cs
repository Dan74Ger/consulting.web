using System.ComponentModel.DataAnnotations;

namespace ConsultingGroup.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        [Display(Name = "Nome Utente")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [Display(Name = "Cognome")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Ruolo")]
        public string Role { get; set; } = "User";

        [Display(Name = "Attivo")]
        public bool IsActive { get; set; } = true;

        // Solo per creazione/modifica password
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public string? ConfirmPassword { get; set; }

        // Per visualizzazione
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public class CreateUserViewModel : UserViewModel
    {
        [Required(ErrorMessage = "La password è obbligatoria")]
        [StringLength(100, ErrorMessage = "La password deve essere di almeno {2} caratteri", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public new string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La conferma password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Password")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public new string ConfirmPassword { get; set; } = string.Empty;
    }

    public class EditUserViewModel : UserViewModel
    {
        [StringLength(100, ErrorMessage = "La password deve essere di almeno {2} caratteri", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova Password (lascia vuoto per non modificare)")]
        public new string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma Nuova Password")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public new string? ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nuova password è obbligatoria")]
        [StringLength(100, ErrorMessage = "La password deve essere di almeno {2} caratteri", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La conferma password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma Nuova Password")]
        [Compare("NewPassword", ErrorMessage = "Le password non corrispondono")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}