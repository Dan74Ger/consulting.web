using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    public class CarteCredito
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Il numero carta è obbligatorio")]
        [Display(Name = "Numero Carta")]
        [StringLength(20)]
        public string NumeroCarta { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'intestazione è obbligatoria")]
        [Display(Name = "Intestazione")]
        [StringLength(200)]
        public string Intestazione { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il mese di scadenza è obbligatorio")]
        [Display(Name = "Mese Scadenza")]
        [Range(1, 12, ErrorMessage = "Il mese deve essere compreso tra 1 e 12")]
        public int MeseScadenza { get; set; }

        [Required(ErrorMessage = "L'anno di scadenza è obbligatorio")]
        [Display(Name = "Anno Scadenza")]
        [Range(2024, 2040, ErrorMessage = "L'anno deve essere valido")]
        public int AnnoScadenza { get; set; }

        [Required(ErrorMessage = "Il PIN è obbligatorio")]
        [Display(Name = "PIN")]
        [StringLength(10)]
        public string PIN { get; set; } = string.Empty;

        [Display(Name = "Note")]
        [StringLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}