using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("anni_fatturazione")]
    public class AnnoFatturazione
    {
        [Key]
        [Column("id_anno_fatturazione")]
        public int IdAnnoFatturazione { get; set; }

        [Required]
        [Column("anno")]
        [Display(Name = "Anno")]
        public int Anno { get; set; }

        [Column("descrizione")]
        [MaxLength(200)]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Column("attivo")]
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        [Column("anno_corrente")]
        [Display(Name = "Anno Corrente")]
        public bool AnnoCorrente { get; set; } = false;

        [Column("note")]
        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Proprietà calcolate
        [NotMapped]
        public string AnnoDescrizione => $"{Anno}" + (!string.IsNullOrEmpty(Descrizione) ? $" - {Descrizione}" : "");

        [NotMapped]
        public string StatoDescrizione => AnnoCorrente ? "Anno Corrente" : (Attivo ? "Attivo" : "Non Attivo");

        [NotMapped]
        public string StatoClasse => AnnoCorrente ? "primary" : (Attivo ? "success" : "danger");

        [NotMapped]
        public string StatoIcona => AnnoCorrente ? "fas fa-star" : (Attivo ? "fas fa-check-circle" : "fas fa-times-circle");
    }
}
