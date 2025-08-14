using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultingGroup.ViewModels
{
    public class StudioViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Il nome dello studio è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome dello studio non può superare i 50 caratteri")]
        [Display(Name = "Nome Studio")]
        public string NomeStudio { get; set; } = string.Empty;
        
        [Display(Name = "Stato")]
        public bool Attivo { get; set; } = true;
        
        [Display(Name = "Data Attivazione")]
        public DateTime DataAttivazione { get; set; }
        
        [Display(Name = "Data Modifica")]
        public DateTime DataModifica { get; set; }
        
        [Display(Name = "Data Cessazione")]
        public DateTime? DataCessazione { get; set; }
        
        [Display(Name = "Anno Fiscale Riattivazione")]
        public int? RiattivatoperAnno { get; set; }
        
        [Display(Name = "Data Riattivazione")]
        public DateTime? DataRiattivazione { get; set; }
        
        [Display(Name = "Creato il")]
        public DateTime CreatedAt { get; set; }
        
        [Display(Name = "Aggiornato il")]
        public DateTime UpdatedAt { get; set; }
        
        // Proprietà calcolate per il display
        public string StatoDescrizione => Attivo ? "Attivo" : "Cessato";
        public string StatoClasse => Attivo ? "success" : "danger";
        public string StatoIcona => Attivo ? "fa-check-circle" : "fa-times-circle";
        
        public bool IsRiattivato => RiattivatoperAnno.HasValue && DataRiattivazione.HasValue;
        public string StatoCompleto => IsRiattivato ? $"Riattivato (Anno {RiattivatoperAnno})" : StatoDescrizione;
    }
    
    public class StudioIndexViewModel
    {
        public List<StudioViewModel> Studios { get; set; } = new();
        public int TotaleAttivi { get; set; }
        public int TotaleCessati { get; set; }
        public int TotaleRiattivati { get; set; }
        public string FiltroStato { get; set; } = "tutti"; // tutti, attivi, cessati, riattivati
        public string Ricerca { get; set; } = string.Empty;
    }
    
    public class StudioCreateEditViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Il nome dello studio è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome dello studio non può superare i 50 caratteri")]
        [Display(Name = "Nome Studio")]
        public string NomeStudio { get; set; } = string.Empty;
        
        [Display(Name = "Stato")]
        public bool Attivo { get; set; } = true;
        
        [Display(Name = "Anno Fiscale per Riattivazione")]
        public int? RiattivatoperAnno { get; set; }
        
        public bool IsEdit { get; set; }
        public string Azione => IsEdit ? "Modifica" : "Nuovo";
        
        // Lista anni fiscali per dropdown (quando avremo la tabella)
        public List<SelectListItem> AnniFiscali { get; set; } = new();
    }
}
