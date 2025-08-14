using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("professionisti")]
    public class Professionista
    {
        [Key]
        [Column("id_professionista")]
        public int IdProfessionista { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nome")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("cognome")]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        // Campi di gestione stato
        [Column("attivo")]
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        [Column("data_attivazione")]
        [Display(Name = "Data Attivazione")]
        public DateTime DataAttivazione { get; set; } = DateTime.UtcNow;

        [Column("data_modifica")]
        [Display(Name = "Data Modifica")]
        public DateTime DataModifica { get; set; } = DateTime.UtcNow;

        [Column("data_cessazione")]
        [Display(Name = "Data Cessazione")]
        public DateTime? DataCessazione { get; set; }

        // Gestione riattivazione per anno fiscale
        [Column("riattivato_per_anno")]
        [Display(Name = "Riattivato per Anno")]
        public int? RiattivatoPerAnno { get; set; }

        [Column("data_riattivazione")]
        [Display(Name = "Data Riattivazione")]
        public DateTime? DataRiattivazione { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigazioni
        [ForeignKey("RiattivatoPerAnno")]
        public virtual AnnoFiscale? AnnoFiscaleRiattivazione { get; set; }

        // Proprietà calcolate
        [NotMapped]
        [Display(Name = "Nome Completo")]
        public string NomeCompleto => $"{Nome} {Cognome}";

        [NotMapped]
        [Display(Name = "Stato")]
        public string StatoDescrizione => Attivo ? "Attivo" : "Cessato";

        [NotMapped]
        [Display(Name = "Stato CSS")]
        public string StatoCssClass => Attivo ? "success" : "danger";
    }
}






