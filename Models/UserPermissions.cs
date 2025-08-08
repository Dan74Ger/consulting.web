using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    public class UserPermissions
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        // SISTEMA PERMESSI SEMPLIFICATO
        
        // Gestione Attività - Tutti i ruoli possono accedere
        public bool CanAccessGestioneClienti { get; set; } = true;
        
        // Dati Utenza Riservata - Solo Admin + Senior (Banche, Carte, Mail, Utenze)
        public bool CanAccessDatiUtenzaRiservata { get; set; } = false;
        
        // Dati Utenza Generale - Tutti i ruoli (Cancelleria, PC, Entratel, AltriDati)  
        public bool CanAccessDatiUtenzaGenerale { get; set; } = true;

        // Metadati
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}