namespace ConsultingGroup.ViewModels
{
    public class AnagraficheIndexViewModel
    {
        // Statistiche Studios
        public int StudiosAttivi { get; set; }
        public int StudiosCessati { get; set; }
        public int StudiosRiattivati { get; set; }
        
        // Statistiche Programmi
        public int ProgrammiAttivi { get; set; }
        public int ProgrammiCessati { get; set; }
        public int ProgrammiRiattivati { get; set; }
        
        // Statistiche Professionisti
        public int ProfessionistiAttivi { get; set; }
        public int ProfessionistiCessati { get; set; }
        public int ProfessionistiRiattivati { get; set; }
        
        // Statistiche Anni Fiscali
        public int AnniFiscaliAttivi { get; set; }
        public int AnniFiscaliCessati { get; set; }
        public string AnnoFiscaleCorrente { get; set; } = string.Empty;
        

        
        // Statistiche Clienti
        public int ClientiAttivi { get; set; }
        public int ClientiCessati { get; set; }
        public int ClientiRiattivati { get; set; }
        
        // Statistiche Regimi Contabili
        public int RegimiContabiliAttivi { get; set; }
        public int RegimiContabiliCessati { get; set; }
        public int RegimiContabiliRiattivati { get; set; }
        
        // Statistiche INPS
        public int TipologieInpsAttive { get; set; }
        public int TipologieInpsCessate { get; set; }
        public int TipologieInpsRiattivate { get; set; }
        
        // Totali generali
        public int TotaleElementiAttivi => StudiosAttivi + ProgrammiAttivi + ProfessionistiAttivi + 
                                          AnniFiscaliAttivi + ClientiAttivi + RegimiContabiliAttivi + TipologieInpsAttive;
                                          
        public int TotaleElementiCessati => StudiosCessati + ProgrammiCessati + ProfessionistiCessati + 
                                           AnniFiscaliCessati + ClientiCessati + RegimiContabiliCessati + TipologieInpsCessate;
    }
}

