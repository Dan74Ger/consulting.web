using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultingGroup.ViewModels
{
    public class ProgrammiViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Nome Programma")]
        public string NomeProgramma { get; set; } = string.Empty;
        
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; }
        
        [Display(Name = "Data Attivazione")]
        public DateTime? DataAttivazione { get; set; }
        
        [Display(Name = "Data Modifica")]
        public DateTime? DataModifica { get; set; }
        
        [Display(Name = "Data Cessazione")]
        public DateTime? DataCessazione { get; set; }
        
        [Display(Name = "Riattivato per Anno")]
        public int? RiattivatoperAnno { get; set; }
        
        [Display(Name = "Data Riattivazione")]
        public DateTime? DataRiattivazione { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Proprietà calcolate per la visualizzazione
        public string StatoDescrizione => Attivo 
            ? (RiattivatoperAnno.HasValue ? $"Riattivato (Anno {RiattivatoperAnno})" : "Attivo")
            : "Cessato";

        public string StatoClasse => Attivo 
            ? (RiattivatoperAnno.HasValue ? "success" : "success")
            : "danger";

        public string StatoIcona => Attivo 
            ? (RiattivatoperAnno.HasValue ? "fas fa-redo" : "fas fa-check-circle")
            : "fas fa-times-circle";

        public bool IsRiattivato => RiattivatoperAnno.HasValue;
    }

    public class ProgrammiIndexViewModel
    {
        public List<ProgrammiViewModel> Programmi { get; set; } = new List<ProgrammiViewModel>();
        public int TotaleAttivi { get; set; }
        public int TotaleCessati { get; set; }
        public int TotaleRiattivati { get; set; }
        public string FiltroStato { get; set; } = "tutti";
        public string Ricerca { get; set; } = string.Empty;
    }

    public class ProgrammiCreateEditViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Il nome del programma è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome del programma non può superare i 100 caratteri")]
        [Display(Name = "Nome Programma")]
        public string NomeProgramma { get; set; } = string.Empty;
        
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;
        
        [Display(Name = "Riattivato per Anno Fiscale")]
        public int? RiattivatoperAnno { get; set; }
        
        // Lista anni fiscali per dropdown (mock data per ora)
        public List<SelectListItem> AnniFiscali { get; set; } = new List<SelectListItem>();
        
        public bool IsEdit => Id > 0;
        public string PageTitle => IsEdit ? "Modifica Programma" : "Nuovo Programma";
        public string SubmitButtonText => IsEdit ? "Salva Modifiche" : "Crea Programma";
    }
}
