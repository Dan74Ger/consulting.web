using System.ComponentModel.DataAnnotations;

namespace ConsultingGroup.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        [Display(Name = "Nome Utente")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}