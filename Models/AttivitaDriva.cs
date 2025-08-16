using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_driva")]
    public class AttivitaDriva
    {
        [Key]
        [Column("id_attivita_driva")]
        public int IdAttivitaDriva { get; set; }

        [Required]
        [Column("id_anno")]
        [Display(Name = "Anno Fiscale")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Column("id_professionista")]
        [Display(Name = "Professionista")]
        public int? IdProfessionista { get; set; }

        [Column("codice_dr_iva")]
        [MaxLength(20)]
        [Display(Name = "Codice DR IVA")]
        public string? CodiceDrIva { get; set; }

        [Column("appuntamento_data_ora")]
        [Display(Name = "Appuntamento")]
        public DateTime? AppuntamentoDataOra { get; set; }

        [Column("acconto_iva_tipo")]
        [MaxLength(20)]
        [Display(Name = "Acconto IVA Tipo")]
        public string? AccontoIvaTipo { get; set; } // storico/analitico/al_20_12

        [Column("acconto_iva_credito_debito")]
        [MaxLength(10)]
        [Display(Name = "Acconto IVA C/D")]
        public string AccontoIvaCreditoDebito { get; set; } = "zero"; // credito/debito/zero

        [Column("importo_acconto_iva", TypeName = "decimal(10,2)")]
        [Display(Name = "Importo Acconto IVA")]
        public decimal? ImportoAccontoIva { get; set; }

        [Column("f24_acconto_iva_stato")]
        [MaxLength(20)]
        [Display(Name = "F24 Acconto IVA Stato")]
        public string F24AccontoIvaStato { get; set; } = "non_spedito"; // non_spedito/mail/entratel

        [Column("raccolta_documenti")]
        [MaxLength(20)]
        [Display(Name = "Raccolta Documenti")]
        public string RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("driva_inserita")]
        [Display(Name = "DRIVA Inserita")]
        public bool DrivaInserita { get; set; } = false;

        [Column("driva_inserita_data")]
        [Display(Name = "Data DRIVA Inserita")]
        public DateTime? DrivaInseritaData { get; set; }

        [Column("driva_controllata")]
        [Display(Name = "DRIVA Controllata")]
        public bool DrivaControllata { get; set; } = false;

        [Column("driva_controllata_data")]
        [Display(Name = "Data DRIVA Controllata")]
        public DateTime? DrivaControllataData { get; set; }

        [Column("driva_credito_debito")]
        [MaxLength(10)]
        [Display(Name = "DRIVA C/D")]
        public string DrivaCreditoDebito { get; set; } = "zero"; // credito/debito/zero

        [Column("importo_dr_iva", TypeName = "decimal(10,2)")]
        [Display(Name = "Importo DR IVA")]
        public decimal? ImportoDrIva { get; set; }

        [Column("f24_driva_consegnato")]
        [Display(Name = "F24 DRIVA Consegnato")]
        public bool F24DrivaConsegnato { get; set; } = false;

        [Column("f24_driva_data")]
        [Display(Name = "Data F24 DRIVA")]
        public DateTime? F24DrivaData { get; set; }

        [Column("dr_visto")]
        [Display(Name = "DR Visto")]
        public bool DrVisto { get; set; } = false;

        [Column("ricevuta_driva")]
        [Display(Name = "Ricevuta DRIVA")]
        public bool RicevutaDriva { get; set; } = false;

        [Column("driva_spedita")]
        [Display(Name = "DRIVA Spedita")]
        public bool DrivaSpedita { get; set; } = false;

        [Column("driva_spedita_data")]
        [Display(Name = "Data DRIVA Spedita")]
        public DateTime? DrivaSpeditaData { get; set; }

        [Column("tcg_data")]
        [Display(Name = "Data TCG")]
        public DateTime? TcgData { get; set; }

        [Column("note")]
        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("IdAnno")]
        public virtual AnnoFiscale? AnnoFiscale { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdProfessionista")]
        public virtual Professionista? Professionista { get; set; }
    }
}
