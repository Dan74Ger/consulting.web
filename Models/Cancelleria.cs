using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    public class Cancelleria
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "La denominazione fornitore è obbligatoria")]
        [Display(Name = "Denominazione Fornitore")]
        [StringLength(200)]
        public string DenominazioneFornitore { get; set; } = string.Empty;

        [Display(Name = "Sito Web")]
        [StringLength(300)]
        public string? SitoWeb { get; set; }

        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        [Display(Name = "Nome Utente")]
        [StringLength(100)]
        public string NomeUtente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [Display(Name = "Password")]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Note")]
        [StringLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}