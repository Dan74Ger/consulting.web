using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_730")]
    public class Attivita730
    {
        [Key]
        [Column("id_attivita_730")]
        public int IdAttivita730 { get; set; }

        [Required]
        [Column("id_anno")]
        [Display(Name = "Anno Fiscale")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        [Display(Name = "Cliente")]
        public int IdCliente { get; set; }

        [Column("id_professionista")]
        [Display(Name = "Professionista")]
        public int? IdProfessionista { get; set; }

        [Column("codice_dr")]
        [StringLength(20)]
        [Display(Name = "Codice DR")]
        public string? CodiceDr { get; set; }

        [Column("raccolta_documenti")]
        [StringLength(20)]
        [Display(Name = "Raccolta Documenti")]
        public string RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("dr_inserita")]
        [Display(Name = "DR Inserita")]
        public bool DrInserita { get; set; } = false;

        [Column("dr_inserita_data")]
        [DataType(DataType.Date)]
        [Display(Name = "Data DR Inserita")]
        public DateTime? DrInseritaData { get; set; }

        [Column("dr_controllata")]
        [Display(Name = "DR Controllata")]
        public bool DrControllata { get; set; } = false;

        [Column("dr_controllata_data")]
        [DataType(DataType.Date)]
        [Display(Name = "Data DR Controllata")]
        public DateTime? DrControllataData { get; set; }

        [Column("dr_spedita")]
        [Display(Name = "DR Spedita")]
        public bool DrSpedita { get; set; } = false;

        [Column("dr_spedita_data")]
        [DataType(DataType.Date)]
        [Display(Name = "Data DR Spedita")]
        public DateTime? DrSpeditaData { get; set; }

        [Column("ricevuta_dr_730")]
        [Display(Name = "Ricevuta DR 730")]
        public bool RicevutaDr730 { get; set; } = false;

        [Column("pec_invio_dr")]
        [Display(Name = "PEC Invio DR")]
        public bool PecInvioDr { get; set; } = false;

        [Column("dr_firmata")]
        [Display(Name = "DR Firmata")]
        public bool DrFirmata { get; set; } = false;

        [Column("note")]
        [StringLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigazioni
        [ForeignKey("IdAnno")]
        public virtual AnnoFiscale? AnnoFiscale { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdProfessionista")]
        public virtual Professionista? Professionista { get; set; }

        // Proprietà calcolate
        [NotMapped]
        [Display(Name = "Stato Lavorazione")]
        public string StatoLavorazione
        {
            get
            {
                if (DrSpedita) return "Spedita";
                if (DrControllata) return "Controllata";
                if (DrInserita) return "Inserita";
                return "Da Lavorare";
            }
        }

        [NotMapped]
        [Display(Name = "CSS Stato")]
        public string StatoCssClass
        {
            get
            {
                if (DrSpedita) return "success";
                if (DrControllata) return "warning";
                if (DrInserita) return "info";
                return "secondary";
            }
        }

        [NotMapped]
        [Display(Name = "Icona Stato")]
        public string StatoIcona
        {
            get
            {
                if (DrSpedita) return "fas fa-check-circle";
                if (DrControllata) return "fas fa-eye";
                if (DrInserita) return "fas fa-edit";
                return "fas fa-clock";
            }
        }

        [NotMapped]
        [Display(Name = "Percentuale Completamento")]
        public int PercentualeCompletamento
        {
            get
            {
                int completati = 0;
                int totali = 3;

                if (DrInserita) completati++;
                if (DrControllata) completati++;
                if (DrSpedita) completati++;

                return (int)Math.Round((double)completati / totali * 100);
            }
        }
    }
}
