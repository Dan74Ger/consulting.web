using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_770")]
    public class Attivita770
    {
        [Key]
        [Column("id_attivita_770")]
        public int IdAttivita770 { get; set; }

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

        [Column("mod_770_lav_autonomo")]
        [Display(Name = "MOD 770 Lav. Autonomo")]
        public bool Mod770LavAutonomo { get; set; } = false;

        [Column("mod_770_ordinario")]
        [Display(Name = "MOD 770 Ordinario")]
        public bool Mod770Ordinario { get; set; } = false;

        [Column("inserimento_dati_dr")]
        [Display(Name = "Inserimento Dati DR")]
        public bool InserimentoDatiDr { get; set; } = false;

        [Column("dr_invio")]
        [Display(Name = "DR Invio")]
        public bool DrInvio { get; set; } = false;

        [Column("ricevuta")]
        [Display(Name = "Ricevuta")]
        public bool Ricevuta { get; set; } = false;

        [Column("pec_invio_dr")]
        [Display(Name = "PEC Invio DR")]
        public bool PecInvioDr { get; set; } = false;

        [Column("mod_cu_fatte")]
        [Display(Name = "MOD CU Fatte")]
        public bool ModCuFatte { get; set; } = false;

        [Column("cu_utili_presenti")]
        [Display(Name = "CU Utili Presenti")]
        public bool CuUtiliPresenti { get; set; } = false;

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
                if (DrInvio) return "DR Inviata";
                if (InserimentoDatiDr) return "Dati Inseriti";
                if (Mod770LavAutonomo || Mod770Ordinario) return "MOD 770 Preparato";
                return "Da Lavorare";
            }
        }

        [NotMapped]
        [Display(Name = "CSS Stato")]
        public string StatoCssClass
        {
            get
            {
                if (DrInvio) return "success";
                if (InserimentoDatiDr) return "warning";
                if (Mod770LavAutonomo || Mod770Ordinario) return "info";
                return "secondary";
            }
        }

        [NotMapped]
        [Display(Name = "Icona Stato")]
        public string StatoIcona
        {
            get
            {
                if (DrInvio) return "fas fa-check-circle";
                if (InserimentoDatiDr) return "fas fa-edit";
                if (Mod770LavAutonomo || Mod770Ordinario) return "fas fa-file-alt";
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
                int totali = 4;

                if (Mod770LavAutonomo || Mod770Ordinario) completati++;
                if (InserimentoDatiDr) completati++;
                if (DrInvio) completati++;
                if (Ricevuta) completati++;

                return (int)Math.Round((double)completati / totali * 100);
            }
        }

        [NotMapped]
        [Display(Name = "Tipo MOD 770")]
        public string TipoMod770
        {
            get
            {
                if (Mod770LavAutonomo && Mod770Ordinario) return "Entrambi";
                if (Mod770Ordinario) return "Ordinario";
                if (Mod770LavAutonomo) return "Lav. Autonomo";
                return "Non Specificato";
            }
        }
    }
}
