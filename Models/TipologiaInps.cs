using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("tipologie_inps")]
    public class TipologiaInps
    {
        [Key]
        [Column("id_tipologia_inps")]
        public int IdTipologiaInps { get; set; }

        [Required]
        [Column("tipologia")]
        [MaxLength(100)]
        [Display(Name = "Tipologia")]
        public string Tipologia { get; set; } = string.Empty;

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
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("RiattivatoPerAnno")]
        public virtual AnnoFiscale? AnnoFiscaleRiattivazione { get; set; }
    }
}






