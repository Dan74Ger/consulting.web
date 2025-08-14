using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultingGroup.ViewModels
{
    public class AnniFiscaliViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Anno")]
        public int Anno { get; set; }
        
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }
        
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; }
        
        [Display(Name = "Anno Corrente")]
        public bool AnnoCorrente { get; set; }
        
        // Scadenze fiscali
        [Display(Name = "Scadenza 730")]
        public DateTime? Scadenza730 { get; set; }
        
        [Display(Name = "Scadenza 740")]
        public DateTime? Scadenza740 { get; set; }
        
        [Display(Name = "Scadenza 750")]
        public DateTime? Scadenza750 { get; set; }
        
        [Display(Name = "Scadenza 760")]
        public DateTime? Scadenza760 { get; set; }
        
        [Display(Name = "Scadenza ENC")]
        public DateTime? ScadenzaENC { get; set; }
        
        [Display(Name = "Scadenza IRAP")]
        public DateTime? ScadenzaIRAP { get; set; }
        
        [Display(Name = "Scadenza 770")]
        public DateTime? Scadenza770 { get; set; }
        
        [Display(Name = "Scadenza CU")]
        public DateTime? ScadenzaCU { get; set; }
        
        [Display(Name = "Scadenza DRIVA")]
        public DateTime? ScadenzaDIVA { get; set; }
        
        [Display(Name = "Scadenza Lipe 1° Trimestre")]
        public DateTime? ScadenzaLipe1t { get; set; }
        
        [Display(Name = "Scadenza Lipe 2° Trimestre")]
        public DateTime? ScadenzaLipe2t { get; set; }
        
        [Display(Name = "Scadenza Lipe 3° Trimestre")]
        public DateTime? ScadenzaLipe3t { get; set; }
        
        [Display(Name = "Scadenza Lipe 4° Trimestre")]
        public DateTime? ScadenzaLipe4t { get; set; }
        
        [Display(Name = "Scadenza Mod TR IVA 1° Trimestre")]
        public DateTime? ScadenzaModTRIva1t { get; set; }
        
        [Display(Name = "Scadenza Mod TR IVA 2° Trimestre")]
        public DateTime? ScadenzaModTRIva2t { get; set; }
        
        [Display(Name = "Scadenza Mod TR IVA 3° Trimestre")]
        public DateTime? ScadenzaModTRIva3t { get; set; }
        
        [Display(Name = "Scadenza Mod TR IVA 4° Trimestre")]
        public DateTime? ScadenzaModTRIva4t { get; set; }
        
        [Display(Name = "Note")]
        public string? Note { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Proprietà calcolate per la visualizzazione
        public string AnnoDescrizione => $"{Anno}" + (!string.IsNullOrEmpty(Descrizione) ? $" - {Descrizione}" : "");
        public string StatoDescrizione => AnnoCorrente ? "Anno Corrente" : (Attivo ? "Attivo" : "Non Attivo");
        public string StatoClasse => AnnoCorrente ? "primary" : (Attivo ? "success" : "danger");
        public string StatoIcona => AnnoCorrente ? "fas fa-star" : (Attivo ? "fas fa-check-circle" : "fas fa-times-circle");
        
        // Proprietà per contare le scadenze impostate
        public int NumeroScadenzeImpostate => new[] { 
            Scadenza730, Scadenza740, Scadenza750, Scadenza760, ScadenzaENC, ScadenzaIRAP, 
            Scadenza770, ScadenzaCU, ScadenzaDIVA, ScadenzaLipe1t, ScadenzaLipe2t, 
            ScadenzaLipe3t, ScadenzaLipe4t, ScadenzaModTRIva1t, ScadenzaModTRIva2t,
            ScadenzaModTRIva3t, ScadenzaModTRIva4t 
        }.Count(d => d.HasValue);
        
        // Totale scadenze possibili (era 13, ora 17 con le nuove Mod TR IVA)
        public int TotaleScadenzePossibili => 17;
    }

    public class AnniFiscaliIndexViewModel
    {
        public List<AnniFiscaliViewModel> AnniFiscali { get; set; } = new List<AnniFiscaliViewModel>();
        public int TotaleAttivi { get; set; }
        public int TotaleNonAttivi { get; set; }
        public int? AnnoCorrente { get; set; }
        public string FiltroStato { get; set; } = "tutti";
        public string Ricerca { get; set; } = string.Empty;
    }

    public class AnniFiscaliCreateEditViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "L'anno è obbligatorio")]
        [Range(1900, 2100, ErrorMessage = "L'anno deve essere compreso tra 1900 e 2100")]
        [Display(Name = "Anno")]
        public int Anno { get; set; }
        
        [StringLength(100, ErrorMessage = "La descrizione non può superare i 100 caratteri")]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }
        
        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;
        
        [Display(Name = "Anno Corrente")]
        public bool AnnoCorrente { get; set; } = false;
        
        // Scadenze fiscali - Gruppo 1: Dichiarazioni
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 730")]
        public DateTime? Scadenza730 { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 740")]
        public DateTime? Scadenza740 { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 750")]
        public DateTime? Scadenza750 { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 760")]
        public DateTime? Scadenza760 { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza 770")]
        public DateTime? Scadenza770 { get; set; }
        
        // Scadenze fiscali - Gruppo 2: Imposte
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza ENC")]
        public DateTime? ScadenzaENC { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza IRAP")]
        public DateTime? ScadenzaIRAP { get; set; }
        
        // Scadenze fiscali - Gruppo 3: Certificazioni
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza CU")]
        public DateTime? ScadenzaCU { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza DRIVA")]
        public DateTime? ScadenzaDIVA { get; set; }
        
        // Scadenze fiscali - Gruppo 4: Lipe Trimestrali
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 1° Trimestre")]
        public DateTime? ScadenzaLipe1t { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 2° Trimestre")]
        public DateTime? ScadenzaLipe2t { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 3° Trimestre")]
        public DateTime? ScadenzaLipe3t { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Lipe 4° Trimestre")]
        public DateTime? ScadenzaLipe4t { get; set; }
        
        // Mod TR IVA Trimestrali
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 1° Trimestre")]
        public DateTime? ScadenzaModTRIva1t { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 2° Trimestre")]
        public DateTime? ScadenzaModTRIva2t { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 3° Trimestre")]
        public DateTime? ScadenzaModTRIva3t { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Scadenza Mod TR IVA 4° Trimestre")]
        public DateTime? ScadenzaModTRIva4t { get; set; }
        
        [StringLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
        [Display(Name = "Note")]
        public string? Note { get; set; }
        
        // Checkbox per duplicare scadenze dall'anno precedente
        [Display(Name = "Duplica scadenze dall'anno precedente")]
        public bool DuplicaScadenzeAnnoPrecedente { get; set; } = false;
        
        public bool IsEdit => Id > 0;
        public string PageTitle => IsEdit ? "Modifica Anno Fiscale" : "Nuovo Anno Fiscale";
        public string SubmitButtonText => IsEdit ? "Salva Modifiche" : "Crea Anno Fiscale";
    }
}
