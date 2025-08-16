using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_lipe")]
    public class AttivitaLipe
    {
        [Key]
        [Column("id_attivita_lipe")]
        public int IdAttivitaLipe { get; set; }

        [Required]
        [Column("id_anno")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_professionista")]
        public int? IdProfessionista { get; set; }

        // Primo Trimestre
        [Column("t1_raccolta_documenti")]
        [MaxLength(20)]
        [Display(Name = "T1 Raccolta Documenti")]
        public string T1RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("t1_lipe_inserita")]
        [Display(Name = "T1 LIPE Inserita")]
        public bool T1LipeInserita { get; set; } = false;

        [Column("t1_lipe_inserita_data")]
        [Display(Name = "T1 Data Inserimento")]
        [DataType(DataType.Date)]
        public DateTime? T1LipeInseritaData { get; set; }

        [Column("t1_lipe_controllata")]
        [Display(Name = "T1 LIPE Controllata")]
        public bool T1LipeControllata { get; set; } = false;

        [Column("t1_lipe_controllata_data")]
        [Display(Name = "T1 Data Controllo")]
        [DataType(DataType.Date)]
        public DateTime? T1LipeControllataData { get; set; }

        [Column("t1_lipe_spedita")]
        [Display(Name = "T1 LIPE Spedita")]
        public bool T1LipeSpedita { get; set; } = false;

        [Column("t1_lipe_spedita_data")]
        [Display(Name = "T1 Data Spedizione")]
        [DataType(DataType.Date)]
        public DateTime? T1LipeSpeditaData { get; set; }

        [Column("t1_created_at")]
        public DateTime T1CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("t1_updated_at")]
        public DateTime T1UpdatedAt { get; set; } = DateTime.UtcNow;

        // Secondo Trimestre
        [Column("t2_raccolta_documenti")]
        [MaxLength(20)]
        [Display(Name = "T2 Raccolta Documenti")]
        public string T2RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("t2_lipe_inserita")]
        [Display(Name = "T2 LIPE Inserita")]
        public bool T2LipeInserita { get; set; } = false;

        [Column("t2_lipe_inserita_data")]
        [Display(Name = "T2 Data Inserimento")]
        [DataType(DataType.Date)]
        public DateTime? T2LipeInseritaData { get; set; }

        [Column("t2_lipe_controllata")]
        [Display(Name = "T2 LIPE Controllata")]
        public bool T2LipeControllata { get; set; } = false;

        [Column("t2_lipe_controllata_data")]
        [Display(Name = "T2 Data Controllo")]
        [DataType(DataType.Date)]
        public DateTime? T2LipeControllataData { get; set; }

        [Column("t2_lipe_spedita")]
        [Display(Name = "T2 LIPE Spedita")]
        public bool T2LipeSpedita { get; set; } = false;

        [Column("t2_lipe_spedita_data")]
        [Display(Name = "T2 Data Spedizione")]
        [DataType(DataType.Date)]
        public DateTime? T2LipeSpeditaData { get; set; }

        [Column("t2_created_at")]
        public DateTime T2CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("t2_updated_at")]
        public DateTime T2UpdatedAt { get; set; } = DateTime.UtcNow;

        // Terzo Trimestre
        [Column("t3_raccolta_documenti")]
        [MaxLength(20)]
        [Display(Name = "T3 Raccolta Documenti")]
        public string T3RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("t3_lipe_inserita")]
        [Display(Name = "T3 LIPE Inserita")]
        public bool T3LipeInserita { get; set; } = false;

        [Column("t3_lipe_inserita_data")]
        [Display(Name = "T3 Data Inserimento")]
        [DataType(DataType.Date)]
        public DateTime? T3LipeInseritaData { get; set; }

        [Column("t3_lipe_controllata")]
        [Display(Name = "T3 LIPE Controllata")]
        public bool T3LipeControllata { get; set; } = false;

        [Column("t3_lipe_controllata_data")]
        [Display(Name = "T3 Data Controllo")]
        [DataType(DataType.Date)]
        public DateTime? T3LipeControllataData { get; set; }

        [Column("t3_lipe_spedita")]
        [Display(Name = "T3 LIPE Spedita")]
        public bool T3LipeSpedita { get; set; } = false;

        [Column("t3_lipe_spedita_data")]
        [Display(Name = "T3 Data Spedizione")]
        [DataType(DataType.Date)]
        public DateTime? T3LipeSpeditaData { get; set; }

        [Column("t3_created_at")]
        public DateTime T3CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("t3_updated_at")]
        public DateTime T3UpdatedAt { get; set; } = DateTime.UtcNow;

        // Quarto Trimestre
        [Column("t4_raccolta_documenti")]
        [MaxLength(20)]
        [Display(Name = "T4 Raccolta Documenti")]
        public string T4RaccoltaDocumenti { get; set; } = "da_richiedere";

        [Column("t4_lipe_inserita")]
        [Display(Name = "T4 LIPE Inserita")]
        public bool T4LipeInserita { get; set; } = false;

        [Column("t4_lipe_inserita_data")]
        [Display(Name = "T4 Data Inserimento")]
        [DataType(DataType.Date)]
        public DateTime? T4LipeInseritaData { get; set; }

        [Column("t4_lipe_controllata")]
        [Display(Name = "T4 LIPE Controllata")]
        public bool T4LipeControllata { get; set; } = false;

        [Column("t4_lipe_controllata_data")]
        [Display(Name = "T4 Data Controllo")]
        [DataType(DataType.Date)]
        public DateTime? T4LipeControllataData { get; set; }

        [Column("t4_lipe_spedita")]
        [Display(Name = "T4 LIPE Spedita")]
        public bool T4LipeSpedita { get; set; } = false;

        [Column("t4_lipe_spedita_data")]
        [Display(Name = "T4 Data Spedizione")]
        [DataType(DataType.Date)]
        public DateTime? T4LipeSpeditaData { get; set; }

        [Column("t4_created_at")]
        public DateTime T4CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("t4_updated_at")]
        public DateTime T4UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("IdAnno")]
        public virtual AnnoFiscale? AnnoFiscale { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdProfessionista")]
        public virtual Professionista? Professionista { get; set; }
    }
}
