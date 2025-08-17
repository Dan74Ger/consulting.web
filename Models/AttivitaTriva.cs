using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_mod_tr_iva")]
    public class AttivitaTriva
    {
        [Key]
        [Column("id_mod_tr_iva")]
        public int IdModTrIva { get; set; }

        [Required]
        [Column("id_anno")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        // Primo Trimestre
        [Column("primo_trimestre")]
        [Display(Name = "1° Trimestre")]
        public bool PrimoTrimestre { get; set; } = false;

        [Column("primo_trimestre_compilato")]
        [Display(Name = "1° Trim. Compilato")]
        public bool PrimoTrimestreCompilato { get; set; } = false;

        [Column("primo_trimestre_spedito")]
        [Display(Name = "1° Trim. Spedito")]
        public bool PrimoTrimestreSpedito { get; set; } = false;

        [Column("primo_trimestre_ricevuta")]
        [Display(Name = "1° Trim. Ricevuta")]
        public bool PrimoTrimestreRicevuta { get; set; } = false;

        [Column("primo_trimestre_mail_cliente")]
        [Display(Name = "1° Trim. Mail Cliente")]
        public bool PrimoTrimestreMailCliente { get; set; } = false;

        // Secondo Trimestre
        [Column("secondo_trimestre")]
        [Display(Name = "2° Trimestre")]
        public bool SecondoTrimestre { get; set; } = false;

        [Column("secondo_trimestre_compilato")]
        [Display(Name = "2° Trim. Compilato")]
        public bool SecondoTrimestreCompilato { get; set; } = false;

        [Column("secondo_trimestre_spedito")]
        [Display(Name = "2° Trim. Spedito")]
        public bool SecondoTrimestreSpedito { get; set; } = false;

        [Column("secondo_trimestre_ricevuta")]
        [Display(Name = "2° Trim. Ricevuta")]
        public bool SecondoTrimestreRicevuta { get; set; } = false;

        [Column("secondo_trimestre_mail_cliente")]
        [Display(Name = "2° Trim. Mail Cliente")]
        public bool SecondoTrimestreMailCliente { get; set; } = false;

        // Terzo Trimestre
        [Column("terzo_trimestre")]
        [Display(Name = "3° Trimestre")]
        public bool TerzoTrimestre { get; set; } = false;

        [Column("terzo_trimestre_compilato")]
        [Display(Name = "3° Trim. Compilato")]
        public bool TerzoTrimestreCompilato { get; set; } = false;

        [Column("terzo_trimestre_spedito")]
        [Display(Name = "3° Trim. Spedito")]
        public bool TerzoTrimestreSpedito { get; set; } = false;

        [Column("terzo_trimestre_ricevuta")]
        [Display(Name = "3° Trim. Ricevuta")]
        public bool TerzoTrimestreRicevuta { get; set; } = false;

        [Column("terzo_trimestre_mail_cliente")]
        [Display(Name = "3° Trim. Mail Cliente")]
        public bool TerzoTrimestreMailCliente { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("IdAnno")]
        public virtual AnnoFiscale? AnnoFiscale { get; set; }

        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }
    }
}
