using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultingGroup.ViewModels
{
    public class ClientiViewModel
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Il nome cliente è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome cliente non può superare i 100 caratteri")]
        [Display(Name = "Nome Cliente")]
        public string NomeCliente { get; set; } = string.Empty;

        [Display(Name = "Programma")]
        public int? IdProgramma { get; set; }

        [Display(Name = "Professionista")]
        public int? IdProfessionista { get; set; }

        [EmailAddress(ErrorMessage = "Inserire un indirizzo email valido")]
        [StringLength(100, ErrorMessage = "L'email non può superare i 100 caratteri")]
        [Display(Name = "Email Cliente")]
        public string? MailCliente { get; set; }

        [Display(Name = "Regime Contabile")]
        public int? IdRegimeContabile { get; set; }

        [Display(Name = "Tipologia INPS")]
        public int? IdTipologiaInps { get; set; }

        [Display(Name = "Contabilità")]
        public bool Contabilita { get; set; } = false; // false=Esterno, true=Interno

        [Display(Name = "Periodo Contabilità")]
        public bool PeriodoContabilita { get; set; } = false; // false=Trimestrale, true=Mensile

        // ATTIVITÀ REDDITI
        [Display(Name = "Mod. 730")]
        public bool Mod730 { get; set; } = false;

        [Display(Name = "Mod. 740")]
        public bool Mod740 { get; set; } = false;

        [Display(Name = "Mod. 750")]
        public bool Mod750 { get; set; } = false;

        [Display(Name = "Mod. 760")]
        public bool Mod760 { get; set; } = false;

        [Display(Name = "Mod. 770")]
        public bool Mod770 { get; set; } = false;

        [Display(Name = "Mod. CU")]
        public bool ModCu { get; set; } = false;

        [Display(Name = "Mod. ENC")]
        public bool ModEnc { get; set; } = false;

        [Display(Name = "Mod. IRAP")]
        public bool ModIrap { get; set; } = false;

        // ATTIVITÀ IVA
        [Display(Name = "DRIVA")]
        public bool Driva { get; set; } = false;

        [Display(Name = "LIPE")]
        public bool Lipe { get; set; } = false;

        [Display(Name = "Mod. TR IVA")]
        public bool ModTrIva { get; set; } = false;

        // ATTIVITÀ CONTABILE
        [Display(Name = "INAIL")]
        public bool Inail { get; set; } = false;

        [Display(Name = "Cassetto Fiscale")]
        public bool CassettoFiscale { get; set; } = false;

        [Display(Name = "Fatturazione Elettronica TS")]
        public bool FatturazioneElettronicaTs { get; set; } = false;

        [Display(Name = "Conservazione")]
        public bool Conservazione { get; set; } = false;

        [Display(Name = "IMU")]
        public bool Imu { get; set; } = false;

        [Display(Name = "Registro IVA")]
        public bool RegIva { get; set; } = false;

        [Display(Name = "Registro Cespiti")]
        public bool RegCespiti { get; set; } = false;

        [Display(Name = "Inventari")]
        public bool Inventari { get; set; } = false;

        [Display(Name = "Libro Giornale")]
        public bool LibroGiornale { get; set; } = false;

        [Display(Name = "Lettere d'Intento")]
        public bool LettereIntento { get; set; } = false;

        [Display(Name = "Mod. INTRASTAT")]
        public bool ModIntrastat { get; set; } = false;

        [Display(Name = "Firma Digitale")]
        public bool FirmaDigitale { get; set; } = false;

        [Display(Name = "Titolare Effettivo")]
        public bool TitolareEffettivo { get; set; } = false;

        [StringLength(20, ErrorMessage = "Il codice ATECO non può superare i 20 caratteri")]
        [Display(Name = "Codice ATECO")]
        public string? CodiceAteco { get; set; }

        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        [Display(Name = "Data Attivazione")]
        public DateTime DataAttivazione { get; set; } = DateTime.UtcNow;

        [Display(Name = "Data Modifica")]
        public DateTime DataModifica { get; set; } = DateTime.UtcNow;

        [Display(Name = "Data Cessazione")]
        public DateTime? DataCessazione { get; set; }

        [Display(Name = "Riattivato per Anno")]
        public int? RiattivatoPerAnno { get; set; }

        [Display(Name = "Data Riattivazione")]
        public DateTime? DataRiattivazione { get; set; }

        // Proprietà aggiuntive per la vista
        [Display(Name = "Programma")]
        public string? ProgrammaDescrizione { get; set; }

        [Display(Name = "Professionista")]
        public string? ProfessionistaDescrizione { get; set; }

        [Display(Name = "Regime Contabile")]
        public string? RegimeContabileDescrizione { get; set; }

        [Display(Name = "Tipologia INPS")]
        public string? TipologiaInpsDescrizione { get; set; }

        [Display(Name = "Anno Fiscale")]
        public string? AnnoFiscaleDescrizione { get; set; }

        // Liste per i dropdown
        public List<SelectListItem> ProgrammiDisponibili { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ProfessionistiDisponibili { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> RegimiContabiliDisponibili { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TipologieInpsDisponibili { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> AnniFiscaliDisponibili { get; set; } = new List<SelectListItem>();

        // Proprietà helper per raggruppamento attività
        public int TotaleAttivitaRedditi => 
            (Mod730 ? 1 : 0) + (Mod740 ? 1 : 0) + (Mod750 ? 1 : 0) + (Mod760 ? 1 : 0) + 
            (Mod770 ? 1 : 0) + (ModCu ? 1 : 0) + (ModEnc ? 1 : 0) + (ModIrap ? 1 : 0);

        public int TotaleAttivitaIva => 
            (Driva ? 1 : 0) + (Lipe ? 1 : 0) + (ModTrIva ? 1 : 0);

        public int TotaleAttivitaContabile => 
            (Inail ? 1 : 0) + (CassettoFiscale ? 1 : 0) + (FatturazioneElettronicaTs ? 1 : 0) + 
            (Conservazione ? 1 : 0) + (Imu ? 1 : 0) + (RegIva ? 1 : 0) + (RegCespiti ? 1 : 0) + 
            (Inventari ? 1 : 0) + (LibroGiornale ? 1 : 0) + (LettereIntento ? 1 : 0) + 
            (ModIntrastat ? 1 : 0) + (FirmaDigitale ? 1 : 0) + (TitolareEffettivo ? 1 : 0);

        public int TotaleAttivitaComplessive => TotaleAttivitaRedditi + TotaleAttivitaIva + TotaleAttivitaContabile;
    }
}






