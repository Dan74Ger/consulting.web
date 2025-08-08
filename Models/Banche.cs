using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    public class Banche
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Il nome banca è obbligatorio")]
        [Display(Name = "Nome Banca")]
        [StringLength(200)]
        public string NomeBanca { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'IBAN è obbligatorio")]
        [Display(Name = "IBAN")]
        [StringLength(34)]
        public string IBAN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il codice utente è obbligatorio")]
        [Display(Name = "Codice Utente")]
        [StringLength(100)]
        public string CodiceUtente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [Display(Name = "Password")]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Indirizzo")]
        [StringLength(500)]
        public string? Indirizzo { get; set; }

        [Display(Name = "Note")]
        [StringLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}