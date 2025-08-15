using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("attivita_cu")]
    public class AttivitaCu
    {
        [Key]
        [Column("id_attivita_cu")]
        public int IdAttivitaCu { get; set; }

        [Required]
        [Column("id_anno")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("id_professionista")]
        public int? IdProfessionista { get; set; }

        [Column("cu_lav_autonomo")]
        [Display(Name = "CU Lav. Autonomo")]
        public bool CuLavAutonomo { get; set; } = false;

        [Column("cu_utili")]
        [Display(Name = "CU Utili")]
        public bool CuUtili { get; set; } = false;

        [Column("invio_cu")]
        [Display(Name = "Invio CU")]
        public bool InvioCu { get; set; } = false;

        [Column("num_cu")]
        [Display(Name = "Numero CU")]
        public int? NumCu { get; set; }

        [Column("ricevuta_cu")]
        [Display(Name = "Ricevuta CU")]
        public bool RicevutaCu { get; set; } = false;

        [Column("invio_cliente_mail")]
        [Display(Name = "Invio Cliente Mail")]
        public bool InvioClienteMail { get; set; } = false;

        [Column("invio_cliente_mail_data")]
        [Display(Name = "Data Invio Cliente Mail")]
        public DateTime? InvioClienteMailData { get; set; }

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
                if (InvioClienteMail) return "Completato";
                if (RicevutaCu) return "Ricevuta CU";
                if (InvioCu) return "Invio CU";
                if (CuUtili || CuLavAutonomo) return "CU Preparato";
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
                    "Ricevuta CU" => "text-info",
                    "Invio CU" => "text-warning",
                    "CU Preparato" => "text-primary",
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
                    "Ricevuta CU" => "📄",
                    "Invio CU" => "📤",
                    "CU Preparato" => "📝",
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
                int totale = 6; // Numero totale di step principali

                if (CuLavAutonomo) completato++;
                if (CuUtili) completato++;
                if (InvioCu) completato++;
                if (RicevutaCu) completato++;
                if (InvioClienteMail) completato++;
                if (NumCu.HasValue && NumCu > 0) completato++;

                return (completato * 100) / totale;
            }
        }

        [NotMapped]
        public string TipoCu
        {
            get
            {
                if (CuLavAutonomo && CuUtili) return "Entrambi";
                if (CuLavAutonomo) return "Lav. Autonomo";
                if (CuUtili) return "Utili";
                return "Nessuno";
            }
        }
    }
}
