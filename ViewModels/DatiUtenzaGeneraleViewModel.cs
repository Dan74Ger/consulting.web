using ConsultingGroup.Models;

namespace ConsultingGroup.ViewModels
{
    public class DatiUtenzaGeneraleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Collezioni per le sezioni generali
        public List<Cancelleria> Cancelleria { get; set; } = new();
        public List<UtentiPC> UtentiPC { get; set; } = new();
        public List<AltriDati> AltriDati { get; set; } = new();
        public List<Entratel> Entratel { get; set; } = new();

        // Contatori per dashboard
        public int TotaleCancelleria => Cancelleria.Count;
        public int TotaleUtentiPC => UtentiPC.Count;
        public int TotaleAltriDati => AltriDati.Count;
        public int TotaleEntratel => Entratel.Count;

        public int TotaleRecords => TotaleCancelleria + TotaleUtentiPC + 
                                   TotaleAltriDati + TotaleEntratel;
        
        // Statistiche rapide
        public int TotaleSezioni => 4; // Cancelleria, PC, AltriDati, Entratel
        public int SezioniAttive => (TotaleCancelleria > 0 ? 1 : 0) + 
                                   (TotaleUtentiPC > 0 ? 1 : 0) + 
                                   (TotaleAltriDati > 0 ? 1 : 0) + 
                                   (TotaleEntratel > 0 ? 1 : 0);
    }
}