using ConsultingGroup.Models;

namespace ConsultingGroup.ViewModels
{
    public class DatiUtenzaExtraViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Collezioni per le sezioni di Dati Generali (accessibili a tutti gli utenti)
        public List<Cancelleria> Cancelleria { get; set; } = new();
        public List<UtentiPC> UtentiPC { get; set; } = new();
        public List<AltriDati> AltriDati { get; set; } = new();
        public List<Entratel> Entratel { get; set; } = new();
        public List<UtentiTS> UtentiTS { get; set; } = new();

        // Contatori per dashboard
        public int TotaleCancelleria => Cancelleria.Count;
        public int TotaleUtentiPC => UtentiPC.Count;
        public int TotaleAltriDati => AltriDati.Count;
        public int TotaleEntratel => Entratel.Count;
        public int TotaleUtentiTS => UtentiTS.Count;

        public int TotaleRecords => TotaleCancelleria + TotaleUtentiPC + TotaleAltriDati + TotaleEntratel + TotaleUtentiTS;
    }

    public enum SezioneDatiGenerali
    {
        Cancelleria,
        UtentiPC,
        AltriDati,
        Entratel,
        UtentiTS
    }
}