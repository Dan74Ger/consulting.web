using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_ENC")]
    public class AttivitaEnc
    {
        [Key]
        [Column("id_attivita_enc")]
        public int IdAttivitaEnc { get; set; }

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
        public string? CodiceDr { get; set; }

        [Column("raccolta_documenti")]
        [MaxLength(20)]
        public string RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("file_isa")]
        public bool FileIsa { get; set; } = false;

        [Column("ricevuta")]
        public bool Ricevuta { get; set; } = false;

        [Column("cciaa")]
        public bool Cciaa { get; set; } = false;

        [Column("pec_invio_dr")]
        public bool PecInvioDr { get; set; } = false;

        [Column("dr_firmata")]
        public bool DrFirmata { get; set; } = false;

        [Column("dr_inserita")]
        public bool DrInserita { get; set; } = false;

        [Column("dr_inserita_data")]
        public DateTime? DrInseritaData { get; set; }

        [Column("isa_dr_inseriti")]
        public bool IsaDrInseriti { get; set; } = false;

        [Column("isa_dr_inseriti_data")]
        public DateTime? IsaDrInseritiData { get; set; }

        [Column("dr_controllata")]
        public bool DrControllata { get; set; } = false;

        [Column("dr_controllata_data")]
        public DateTime? DrControllataData { get; set; }

        [Column("dr_spedita")]
        public bool DrSpedita { get; set; } = false;

        [Column("dr_spedita_data")]
        public DateTime? DrSpeditaData { get; set; }

        [Column("numero_rate_f24_primo_acconto_saldo")]
        public int NumeroRateF24PrimoAccontoSaldo { get; set; } = 0;

        [Column("f24_primo_acconto_saldo_consegnato")]
        public bool F24PrimoAccontoSaldoConsegnato { get; set; } = false;

        [Column("f24_secondo_acconto")]
        public int F24SecondoAcconto { get; set; } = 0;

        [Column("f24_secondo_acconto_consegnato")]
        public bool F24SecondoAccontoConsegnato { get; set; } = false;

        [Column("note")]
        [MaxLength(500)]
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
    }
}


