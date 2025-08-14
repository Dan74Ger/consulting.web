using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("tipo_programmi")]
    public class Programma
    {
        [Key]
        [Column("id_programma")]
        public int IdProgramma { get; set; }
        
        [Column("nome_programma")]
        [Required]
        [StringLength(100)]
        public string NomeProgramma { get; set; } = string.Empty;
        
        // Campi di gestione stato
        [Column("attivo")]
        public bool Attivo { get; set; }
        
        [Column("data_attivazione")]
        public DateTime DataAttivazione { get; set; }
        
        [Column("data_modifica")]
        public DateTime DataModifica { get; set; }
        
        [Column("data_cessazione")]
        public DateTime? DataCessazione { get; set; }
        
        // Gestione riattivazione per anno fiscale
        [Column("riattivato_per_anno")]
        public int? RiattivatoPerAnno { get; set; }
        
        [Column("data_riattivazione")]
        public DateTime? DataRiattivazione { get; set; }
        
        // Campi di audit
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        // Foreign Key per anni fiscali 
        [ForeignKey("RiattivatoPerAnno")]
        public virtual AnnoFiscale? AnnoFiscaleRiattivazione { get; set; }
        
        // Proprietà calcolate per la visualizzazione
        [NotMapped]
        public string StatoDescrizione => Attivo 
            ? (RiattivatoPerAnno.HasValue ? $"Riattivato (Anno {RiattivatoPerAnno})" : "Attivo")
            : "Cessato";

        [NotMapped]
        public string StatoClasse => Attivo 
            ? (RiattivatoPerAnno.HasValue ? "success" : "success")
            : "danger";

        [NotMapped]
        public string StatoIcona => Attivo 
            ? (RiattivatoPerAnno.HasValue ? "fas fa-redo" : "fas fa-check-circle")
            : "fas fa-times-circle";

        [NotMapped]
        public bool IsRiattivato => RiattivatoPerAnno.HasValue;
    }
}

