using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("anni_fiscali")]
    public class AnnoFiscale
    {
        [Key]
        [Column("id_anno")]
        public int IdAnno { get; set; }
        
        [Column("anno")]
        [Required(ErrorMessage = "L'anno è obbligatorio")]
        [Range(1900, 2100, ErrorMessage = "L'anno deve essere compreso tra 1900 e 2100")]
        public int Anno { get; set; }
        
        [Column("descrizione")]
        [StringLength(100, ErrorMessage = "La descrizione non può superare i 100 caratteri")]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }
        
        [Column("attivo")]
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;
        
        [Column("anno_corrente")]
        [Display(Name = "Anno Corrente")]
        public bool AnnoCorrente { get; set; } = false;
        
        // Scadenze fiscali
        [Column("scadenza_730")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 730")]
        public DateTime? Scadenza730 { get; set; }
        
        [Column("scadenza_740")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 740")]
        public DateTime? Scadenza740 { get; set; }
        
        [Column("scadenza_750")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 750")]
        public DateTime? Scadenza750 { get; set; }
        
        [Column("scadenza_760")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 760")]
        public DateTime? Scadenza760 { get; set; }
        
        [Column("scadenza_ENC")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza ENC")]
        public DateTime? ScadenzaENC { get; set; }
        
        [Column("scadenza_IRAP")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza IRAP")]
        public DateTime? ScadenzaIRAP { get; set; }
        
        [Column("scadenza_770")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 770")]
        public DateTime? Scadenza770 { get; set; }
        
        [Column("scadenza_CU")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza CU")]
        public DateTime? ScadenzaCU { get; set; }
        
        [Column("scadenza_DIVA")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza DRIVA")]
        public DateTime? ScadenzaDIVA { get; set; }
        
        [Column("scadenza_Lipe_1t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 1° Trimestre")]
        public DateTime? ScadenzaLipe1t { get; set; }
        
        [Column("scadenza_Lipe_2t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 2° Trimestre")]
        public DateTime? ScadenzaLipe2t { get; set; }
        
        [Column("scadenza_Lipe_3t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 3° Trimestre")]
        public DateTime? ScadenzaLipe3t { get; set; }
        
        [Column("scadenza_Lipe_4t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 4° Trimestre")]
        public DateTime? ScadenzaLipe4t { get; set; }
        
        [Column("scadenza_Mod_TR_Iva_1t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 1° Trimestre")]
        public DateTime? ScadenzaModTRIva1t { get; set; }
        
        [Column("scadenza_Mod_TR_Iva_2t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 2° Trimestre")]
        public DateTime? ScadenzaModTRIva2t { get; set; }
        
        [Column("scadenza_Mod_TR_Iva_3t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 3° Trimestre")]
        public DateTime? ScadenzaModTRIva3t { get; set; }
        
        [Column("scadenza_Mod_TR_Iva_4t")]
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 4° Trimestre")]
        public DateTime? ScadenzaModTRIva4t { get; set; }
        
        [Column("note")]
        [StringLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
        [Display(Name = "Note")]
        public string? Note { get; set; }
        
        // Campi di audit
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
        
        // Relazioni inverse 
        public virtual ICollection<Studio>? StudiosRiattivati { get; set; }
        public virtual ICollection<Programma>? ProgrammiRiattivati { get; set; }
    }
}
