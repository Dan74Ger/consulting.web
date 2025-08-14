using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultingGroup.ViewModels
{
    public class RegimiContabiliViewModel
    {
        public int IdRegimeContabile { get; set; }

        [Required(ErrorMessage = "Il nome del regime è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome del regime non può superare i 100 caratteri")]
        [Display(Name = "Nome Regime")]
        public string NomeRegime { get; set; } = string.Empty;

        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        [Display(Name = "Data Attivazione")]
        public DateTime? DataAttivazione { get; set; }

        [Display(Name = "Data Modifica")]
        public DateTime? DataModifica { get; set; }

        [Display(Name = "Data Cessazione")]
        public DateTime? DataCessazione { get; set; }

        [Display(Name = "Riattivato per Anno")]
        public int? RiattivatoPerAnno { get; set; }

        [Display(Name = "Data Riattivazione")]
        public DateTime? DataRiattivazione { get; set; }

        // Proprietà calcolate
        [Display(Name = "Stato")]
        public string StatoDescrizione => Attivo ? "Attivo" : "Cessato";

        [Display(Name = "Stato CSS")]
        public string StatoCssClass => Attivo ? "success" : "danger";

        // Per le relazioni
        [Display(Name = "Anno Fiscale")]
        public string? AnnoFiscaleDescrizione { get; set; }

        // Lista per dropdown anni fiscali
        public List<SelectListItem> AnniFiscaliDisponibili { get; set; } = new List<SelectListItem>();
    }
}






