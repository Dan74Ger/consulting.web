using ConsultingGroup.Models;

namespace ConsultingGroup.ViewModels
{
    public class DatiUtenzaViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Collezioni per ogni sezione
        public List<Banche> Banche { get; set; } = new();
        public List<CarteCredito> CarteCredito { get; set; } = new();
        public List<Utenze> Utenze { get; set; } = new();
        public List<Cancelleria> Cancelleria { get; set; } = new();
        public List<Mail> Mail { get; set; } = new();
        public List<UtentiPC> UtentiPC { get; set; } = new();
        public List<AltriDati> AltriDati { get; set; } = new();
        // Contatori per dashboard
        public int TotaleBanche => Banche.Count;
        public int TotaleCarteCredito => CarteCredito.Count;
        public int TotaleUtenze => Utenze.Count;
        public int TotaleCancelleria => Cancelleria.Count;
        public int TotaleMail => Mail.Count;
        public int TotaleUtentiPC => UtentiPC.Count;
        public int TotaleAltriDati => AltriDati.Count;
        
        // Entratel ora è gestito da DatiUtenzaExtra, calcolato separatamente
        public int TotaleEntratel { get; set; } = 0;

        public int TotaleRecords => TotaleBanche + TotaleCarteCredito + TotaleUtenze + 
                                   TotaleCancelleria + TotaleMail + TotaleUtentiPC + 
                                   TotaleAltriDati + TotaleEntratel;
    }

    public enum SezioneDatiUtenza
    {
        Banche,
        CarteCredito,
        Utenze,
        Cancelleria,
        Mail,
        UtentiPC,
        AltriDati,
        Entratel
    }
}