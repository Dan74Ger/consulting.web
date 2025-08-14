using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("studios")]
    public class Studio
    {
        [Key]
        [Column("id_studio")]
        public int IdStudio { get; set; }
        
        [Required]
        [StringLength(50)]
        [Column("nome_studio")]
        public string NomeStudio { get; set; } = string.Empty;
        
        // Campi di gestione stato
        [Column("attivo")]
        public bool Attivo { get; set; } = true;
        
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
        
        // Proprietà calcolate
        [NotMapped]
        public bool IsRiattivato => RiattivatoPerAnno.HasValue && DataRiattivazione.HasValue;
        
        [NotMapped]
        public string StatoDescrizione => Attivo ? "Attivo" : "Cessato";
        
        [NotMapped]
        public string StatoCompleto => IsRiattivato ? $"Riattivato (Anno {RiattivatoPerAnno})" : StatoDescrizione;
    }
}
