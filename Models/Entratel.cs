using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    public class Entratel
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Il sito è obbligatorio")]
        [Display(Name = "Sito")]
        [StringLength(300)]
        public string Sito { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'utente è obbligatorio")]
        [Display(Name = "Utente")]
        [StringLength(100)]
        public string Utente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        [Display(Name = "Password")]
        [StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "PIN Dati Catastali")]
        [StringLength(50)]
        public string? PinDatiCatastali { get; set; }

        [Display(Name = "PIN Deleghe Cassetto Fiscale")]
        [StringLength(50)]
        public string? PinDelegheCassettoFiscale { get; set; }

        [Display(Name = "PIN Completo")]
        [StringLength(50)]
        public string? PinCompleto { get; set; }

        [Display(Name = "Desktop Telematico Utente")]
        [StringLength(100)]
        public string? DesktopTelematicoUtente { get; set; }

        [Display(Name = "Desktop Telematico Password")]
        [StringLength(200)]
        public string? DesktopTelematicoPassword { get; set; }

        [Display(Name = "Note")]
        [StringLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}