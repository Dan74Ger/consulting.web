using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultingGroup.ViewModels
{
    public class TipologieInpsViewModel
    {
        public int IdTipologiaInps { get; set; }

        [Required(ErrorMessage = "La tipologia è obbligatoria")]
        [StringLength(100, ErrorMessage = "La tipologia non può superare i 100 caratteri")]
        [Display(Name = "Tipologia")]
        public string Tipologia { get; set; } = string.Empty;

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
        [Display(Name = "Anno Fiscale")]
        public string? AnnoFiscaleDescrizione { get; set; }

        // Lista degli anni fiscali disponibili per il dropdown
        public List<SelectListItem> AnniFiscaliDisponibili { get; set; } = new List<SelectListItem>();
    }
}






