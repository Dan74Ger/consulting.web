using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("proforma_generate")]
    public class ProformaGenerata
    {
        [Key]
        [Column("id_proforma")]
        public int IdProforma { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("id_anno_fatturazione")]
        public int IdAnnoFatturazione { get; set; }

        // Dati del mandato di riferimento
        [Column("data_mandato")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Mandato")]
        public DateTime? DataMandato { get; set; }

        [Column("importo_mandato_annuo")]
        [Display(Name = "Importo Mandato Annuo")]
        [DataType(DataType.Currency)]
        public decimal? ImportoMandatoAnnuo { get; set; }

        [Required]
        [Column("tipo_proforma")]
        [MaxLength(20)]
        [Display(Name = "Tipo Proforma")]
        public string TipoProforma { get; set; } = "trimestrale";

        // Dati della singola proforma
        [Required]
        [Column("numero_rata")]
        [Display(Name = "Numero Rata")]
        public int NumeroRata { get; set; }

        [Required]
        [Column("data_scadenza")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Scadenza")]
        public DateTime DataScadenza { get; set; }

        [Required]
        [Column("importo_rata")]
        [Display(Name = "Importo Rata")]
        [DataType(DataType.Currency)]
        public decimal ImportoRata { get; set; }

        // Stato della proforma
        [Column("inviata")]
        [Display(Name = "Inviata")]
        public bool Inviata { get; set; } = false;

        [Column("pagata")]
        [Display(Name = "Pagata")]
        public bool Pagata { get; set; } = false;

        [Column("data_invio")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Invio")]
        public DateTime? DataInvio { get; set; }

        [Column("data_pagamento")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Pagamento")]
        public DateTime? DataPagamento { get; set; }

        [Column("note")]
        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        // Campi di audit
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties con foreign key explicit
        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }
        
        [ForeignKey("IdAnnoFatturazione")]
        public virtual AnnoFatturazione? AnnoFatturazione { get; set; }

        // Proprietà calcolate
        [NotMapped]
        public string StatoDescrizione
        {
            get
            {
                if (Pagata) return "Pagata";
                if (Inviata) return "Inviata";
                return "Da Inviare";
            }
        }

        [NotMapped]
        public string StatoClasse
        {
            get
            {
                if (Pagata) return "success";
                if (Inviata) return "warning";
                return "secondary";
            }
        }

        [NotMapped]
        public string StatoIcona
        {
            get
            {
                if (Pagata) return "fas fa-check-circle";
                if (Inviata) return "fas fa-paper-plane";
                return "fas fa-clock";
            }
        }

        [NotMapped]
        public string DescrizioneCompleta => $"Rata {NumeroRata}/{GetNumeroTotaleRate()} - {DataScadenza:dd/MM/yyyy} - €{ImportoRata:F2}";

        private int GetNumeroTotaleRate()
        {
            return TipoProforma?.ToLower() switch
            {
                "trimestrale" => 4,
                "bimestrale" => 2,
                "mensile" => 12,
                _ => 1
            };
        }

        [NotMapped]
        public bool InRitardo => DataScadenza < DateTime.Today && !Pagata;
    }
}
