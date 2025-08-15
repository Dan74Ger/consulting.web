using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_irap")]
    public class AttivitaIrap
    {
        [Key]
        [Column("id_attivita_irap")]
        public int IdAttivitaIrap { get; set; }

        [Required]
        [Column("id_anno")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_professionista")]
        public int? IdProfessionista { get; set; }

        [Column("appuntamento_data_ora")]
        public DateTime? AppuntamentoDataOra { get; set; }

        [Column("codice_dr")]
        [MaxLength(20)]
        [Display(Name = "Codice DR")]
        public string? CodiceDr { get; set; }

        [Column("raccolta_documenti")]
        [MaxLength(20)]
        [Display(Name = "Raccolta Documenti")]
        public string RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("file_isa")]
        [Display(Name = "File ISA")]
        public bool FileIsa { get; set; } = false;

        [Column("ricevuta")]
        [Display(Name = "Ricevuta")]
        public bool Ricevuta { get; set; } = false;

        [Column("cciaa")]
        [Display(Name = "CCIAA")]
        public bool Cciaa { get; set; } = false;

        [Column("pec_invio_dr")]
        [Display(Name = "PEC Invio DR")]
        public bool PecInvioDr { get; set; } = false;

        [Column("dr_firmata")]
        [Display(Name = "DR Firmata")]
        public bool DrFirmata { get; set; } = false;

        [Column("dr_inserita")]
        [Display(Name = "DR Inserita")]
        public bool DrInserita { get; set; } = false;

        [Column("dr_inserita_data")]
        [Display(Name = "Data DR Inserita")]
        public DateTime? DrInseritaData { get; set; }

        [Column("isa_dr_inseriti")]
        [Display(Name = "ISA DR Inseriti")]
        public bool IsaDrInseriti { get; set; } = false;

        [Column("isa_dr_inseriti_data")]
        [Display(Name = "Data ISA DR Inseriti")]
        public DateTime? IsaDrInseritiData { get; set; }

        [Column("dr_controllata")]
        [Display(Name = "DR Controllata")]
        public bool DrControllata { get; set; } = false;

        [Column("dr_controllata_data")]
        [Display(Name = "Data DR Controllata")]
        public DateTime? DrControllataData { get; set; }

        [Column("dr_spedita")]
        [Display(Name = "DR Spedita")]
        public bool DrSpedita { get; set; } = false;

        [Column("dr_spedita_data")]
        [Display(Name = "Data DR Spedita")]
        public DateTime? DrSpeditaData { get; set; }

        [Column("numero_rate_f24_primo_acconto_saldo")]
        [Display(Name = "N° Rate F24 Primo Acconto/Saldo")]
        public int NumeroRateF24PrimoAccontoSaldo { get; set; } = 0;

        [Column("f24_primo_acconto_saldo_consegnato")]
        [Display(Name = "F24 Primo Acconto/Saldo Consegnato")]
        public bool F24PrimoAccontoSaldoConsegnato { get; set; } = false;

        [Column("f24_secondo_acconto")]
        [Display(Name = "F24 Secondo Acconto")]
        public int F24SecondoAcconto { get; set; } = 0;

        [Column("f24_secondo_acconto_consegnato")]
        [Display(Name = "F24 Secondo Acconto Consegnato")]
        public bool F24SecondoAccontoConsegnato { get; set; } = false;

        [Column("note")]
        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("IdAnno")]
        public virtual AnnoFiscale? AnnoFiscale { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdProfessionista")]
        public virtual Professionista? Professionista { get; set; }

        // Computed properties per filtri e stato (NotMapped)
        [NotMapped]
        public string StatoLavorazione
        {
            get
            {
                if (DrSpedita) return "Completato";
                if (DrControllata) return "In controllo";
                if (DrInserita) return "DR inserita";
                if (IsaDrInseriti) return "ISA inseriti";
                if (FileIsa) return "File ISA";
                if (RaccoltaDocumenti == "completata") return "Documenti pronti";
                return "In attesa";
            }
        }

        [NotMapped]
        public string StatoCssClass
        {
            get
            {
                return StatoLavorazione switch
                {
                    "Completato" => "text-success",
                    "In controllo" => "text-warning",
                    "DR inserita" => "text-info",
                    "ISA inseriti" => "text-primary",
                    "File ISA" => "text-secondary",
                    "Documenti pronti" => "text-info",
                    _ => "text-muted"
                };
            }
        }

        [NotMapped]
        public string StatoIcona
        {
            get
            {
                return StatoLavorazione switch
                {
                    "Completato" => "✅",
                    "In controllo" => "⚠️",
                    "DR inserita" => "📝",
                    "ISA inseriti" => "📊",
                    "File ISA" => "📄",
                    "Documenti pronti" => "📋",
                    _ => "⏳"
                };
            }
        }

        [NotMapped]
        public int PercentualeCompletamento
        {
            get
            {
                int completato = 0;
                int totale = 10; // Numero totale di step principali

                if (RaccoltaDocumenti == "completata") completato++;
                if (FileIsa) completato++;
                if (Ricevuta) completato++;
                if (Cciaa) completato++;
                if (PecInvioDr) completato++;
                if (DrFirmata) completato++;
                if (DrInserita) completato++;
                if (IsaDrInseriti) completato++;
                if (DrControllata) completato++;
                if (DrSpedita) completato++;

                return (completato * 100) / totale;
            }
        }
    }
}
