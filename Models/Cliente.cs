using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("clienti")]
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("nome_cliente")]
        [MaxLength(100)]
        [Display(Name = "Nome Cliente")]
        public string NomeCliente { get; set; } = string.Empty;

        [Column("id_programma")]
        [Display(Name = "Programma")]
        public int? IdProgramma { get; set; }

        [Column("id_professionista")]
        [Display(Name = "Professionista")]
        public int? IdProfessionista { get; set; }

        [Column("mail_cliente")]
        [MaxLength(100)]
        [EmailAddress]
        [Display(Name = "Email Cliente")]
        public string? MailCliente { get; set; }

        [Column("id_regime_contabile")]
        [Display(Name = "Regime Contabile")]
        public int? IdRegimeContabile { get; set; }

        [Column("id_tipologia_inps")]
        [Display(Name = "Tipologia INPS")]
        public int? IdTipologiaInps { get; set; }

        [Column("contabilita")]
        [Display(Name = "Contabilità")]
        public bool Contabilita { get; set; } = false; // false=Esterno, true=Interno

        [Column("periodo_contabilita")]
        [Display(Name = "Periodo Contabilità")]
        public bool PeriodoContabilita { get; set; } = false; // false=Trimestrale, true=Mensile

        // ATTIVITÀ REDDITI
        [Column("mod_730")]
        [Display(Name = "Mod. 730")]
        public bool Mod730 { get; set; } = false;

        [Column("mod_740")]
        [Display(Name = "Mod. 740")]
        public bool Mod740 { get; set; } = false;

        [Column("mod_750")]
        [Display(Name = "Mod. 750")]
        public bool Mod750 { get; set; } = false;

        [Column("mod_760")]
        [Display(Name = "Mod. 760")]
        public bool Mod760 { get; set; } = false;

        [Column("mod_770")]
        [Display(Name = "Mod. 770")]
        public bool Mod770 { get; set; } = false;

        [Column("mod_cu")]
        [Display(Name = "Mod. CU")]
        public bool ModCu { get; set; } = false;

        [Column("mod_enc")]
        [Display(Name = "Mod. ENC")]
        public bool ModEnc { get; set; } = false;

        [Column("mod_irap")]
        [Display(Name = "Mod. IRAP")]
        public bool ModIrap { get; set; } = false;

        // ATTIVITÀ IVA
        [Column("driva")]
        [Display(Name = "DRIVA")]
        public bool Driva { get; set; } = false;

        [Column("lipe")]
        [Display(Name = "LIPE")]
        public bool Lipe { get; set; } = false;

        [Column("mod_tr_iva")]
        [Display(Name = "Mod. TR IVA")]
        public bool ModTrIva { get; set; } = false;

        // ATTIVITÀ CONTABILE
        [Column("inail")]
        [Display(Name = "INAIL")]
        public bool Inail { get; set; } = false;

        [Column("cassetto_fiscale")]
        [Display(Name = "Cassetto Fiscale")]
        public bool CassettoFiscale { get; set; } = false;

        [Column("fatturazione_elettronica_ts")]
        [Display(Name = "Fatturazione Elettronica TS")]
        public bool FatturazioneElettronicaTs { get; set; } = false;

        [Column("conservazione")]
        [Display(Name = "Conservazione")]
        public bool Conservazione { get; set; } = false;

        [Column("imu")]
        [Display(Name = "IMU")]
        public bool Imu { get; set; } = false;

        [Column("reg_iva")]
        [Display(Name = "Registro IVA")]
        public bool RegIva { get; set; } = false;

        [Column("reg_cespiti")]
        [Display(Name = "Registro Cespiti")]
        public bool RegCespiti { get; set; } = false;

        [Column("inventari")]
        [Display(Name = "Inventari")]
        public bool Inventari { get; set; } = false;

        [Column("libro_giornale")]
        [Display(Name = "Libro Giornale")]
        public bool LibroGiornale { get; set; } = false;

        [Column("lettere_intento")]
        [Display(Name = "Lettere d'Intento")]
        public bool LettereIntento { get; set; } = false;

        [Column("mod_intrastat")]
        [Display(Name = "Mod. INTRASTAT")]
        public bool ModIntrastat { get; set; } = false;

        [Column("firma_digitale")]
        [Display(Name = "Firma Digitale")]
        public bool FirmaDigitale { get; set; } = false;

        [Column("titolare_effettivo")]
        [Display(Name = "Titolare Effettivo")]
        public bool TitolareEffettivo { get; set; } = false;

        // CODICE ATECO - CAMPO MASTER
        [Column("codice_ateco")]
        [MaxLength(20)]
        [Display(Name = "Codice ATECO")]
        public string? CodiceAteco { get; set; }

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

        // Navigation properties
        [ForeignKey("IdProgramma")]
        public virtual Programma? Programma { get; set; }

        [ForeignKey("IdProfessionista")]
        public virtual Professionista? Professionista { get; set; }

        [ForeignKey("IdRegimeContabile")]
        public virtual RegimeContabile? RegimeContabile { get; set; }

        [ForeignKey("IdTipologiaInps")]
        public virtual TipologiaInps? TipologiaInps { get; set; }

        [ForeignKey("RiattivatoPerAnno")]
        public virtual AnnoFiscale? AnnoFiscaleRiattivazione { get; set; }
    }
}




