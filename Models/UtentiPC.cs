using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    public class UtentiPC
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Il nome PC è obbligatorio")]
        [Display(Name = "Nome PC")]
        [StringLength(100)]
        public string NomePC { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'utente è obbligatorio")]
        [Display(Name = "Utente")]
        [StringLength(100)]
        public string Utente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [Display(Name = "Password")]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Indirizzo Rete")]
        [StringLength(100)]
        public string? IndirizzoRete { get; set; }

        [Display(Name = "Note")]
        [StringLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}