using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("contabilita_interna_trimestrale")]
    public class ContabilitaInternaTrimestrale
    {
        [Key]
        [Column("id_contabilita_interna_trimestrale")]
        public int IdContabilitaInternaTrimestrale { get; set; }

        [Required]
        [Column("id_anno")]
        public int IdAnno { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("codice_contabilita")]
        [StringLength(50)]
        [Display(Name = "Codice Contabilità")]
        public string? CodiceContabilita { get; set; }

        // Primo Trimestre
        [Column("primo_trimestre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "1° Trim. Ultima FT Vendita")]
        public string? PrimoTrimestreUltimaFtVendita { get; set; }

        [Column("primo_trimestre_data_ft")]
        [Display(Name = "1° Trim. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? PrimoTrimestreDataFt { get; set; }

        [Column("primo_trimestre_liq_iva_importo")]
        [Display(Name = "1° Trim. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? PrimoTrimestreLiqIvaImporto { get; set; }

        [Column("primo_trimestre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "1° Trim. Debito/Credito")]
        public string? PrimoTrimestreDebitoCredito { get; set; }

        [Column("primo_trimestre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "1° Trim. F24 Consegnato")]
        public string? PrimoTrimestreF24Consegnato { get; set; }

        [Column("primo_trimestre_importo_credito")]
        [Display(Name = "1° Trim. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? PrimoTrimestreImportoCredito { get; set; }

        [Column("primo_trimestre_importo_debito")]
        [Display(Name = "1° Trim. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? PrimoTrimestreImportoDebito { get; set; }

        [Column("primo_trimestre_credito_anno_precedente")]
        [Display(Name = "1° Trim. Credito Anno Precedente")]
        [DataType(DataType.Currency)]
        public decimal? PrimoTrimestreCreditoAnnoPrecedente { get; set; }

        // Secondo Trimestre
        [Column("secondo_trimestre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "2° Trim. Ultima FT Vendita")]
        public string? SecondoTrimestreUltimaFtVendita { get; set; }

        [Column("secondo_trimestre_data_ft")]
        [Display(Name = "2° Trim. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? SecondoTrimestreDataFt { get; set; }

        [Column("secondo_trimestre_liq_iva_importo")]
        [Display(Name = "2° Trim. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? SecondoTrimestreLiqIvaImporto { get; set; }

        [Column("secondo_trimestre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "2° Trim. Debito/Credito")]
        public string? SecondoTrimestreDebitoCredito { get; set; }

        [Column("secondo_trimestre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "2° Trim. F24 Consegnato")]
        public string? SecondoTrimestreF24Consegnato { get; set; }

        [Column("secondo_trimestre_importo_credito")]
        [Display(Name = "2° Trim. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? SecondoTrimestreImportoCredito { get; set; }

        [Column("secondo_trimestre_importo_debito")]
        [Display(Name = "2° Trim. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? SecondoTrimestreImportoDebito { get; set; }

        [Column("secondo_trimestre_credito_trimestre_precedente")]
        [Display(Name = "2° Trim. Credito Trimestre Precedente")]
        [DataType(DataType.Currency)]
        public decimal? SecondoTrimestreCreditoTrimestrePrecedente { get; set; }

        // Terzo Trimestre
        [Column("terzo_trimestre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "3° Trim. Ultima FT Vendita")]
        public string? TerzoTrimestreUltimaFtVendita { get; set; }

        [Column("terzo_trimestre_data_ft")]
        [Display(Name = "3° Trim. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? TerzoTrimestreDataFt { get; set; }

        [Column("terzo_trimestre_liq_iva_importo")]
        [Display(Name = "3° Trim. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? TerzoTrimestreLiqIvaImporto { get; set; }

        [Column("terzo_trimestre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "3° Trim. Debito/Credito")]
        public string? TerzoTrimestreDebitoCredito { get; set; }

        [Column("terzo_trimestre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "3° Trim. F24 Consegnato")]
        public string? TerzoTrimestreF24Consegnato { get; set; }

        [Column("terzo_trimestre_importo_credito")]
        [Display(Name = "3° Trim. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? TerzoTrimestreImportoCredito { get; set; }

        [Column("terzo_trimestre_importo_debito")]
        [Display(Name = "3° Trim. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? TerzoTrimestreImportoDebito { get; set; }

        [Column("terzo_trimestre_credito_trimestre_precedente")]
        [Display(Name = "3° Trim. Credito Trimestre Precedente")]
        [DataType(DataType.Currency)]
        public decimal? TerzoTrimestreCreditoTrimestrePrecedente { get; set; }

        // Quarto Trimestre
        [Column("quarto_trimestre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "4° Trim. Ultima FT Vendita")]
        public string? QuartoTrimestreUltimaFtVendita { get; set; }

        [Column("quarto_trimestre_data_ft")]
        [Display(Name = "4° Trim. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? QuartoTrimestreDataFt { get; set; }

        [Column("quarto_trimestre_liq_iva_importo")]
        [Display(Name = "4° Trim. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? QuartoTrimestreLiqIvaImporto { get; set; }

        [Column("quarto_trimestre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "4° Trim. Debito/Credito")]
        public string? QuartoTrimestreDebitoCredito { get; set; }

        [Column("quarto_trimestre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "4° Trim. F24 Consegnato")]
        public string? QuartoTrimestreF24Consegnato { get; set; }

        [Column("quarto_trimestre_importo_credito")]
        [Display(Name = "4° Trim. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? QuartoTrimestreImportoCredito { get; set; }

        [Column("quarto_trimestre_importo_debito")]
        [Display(Name = "4° Trim. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? QuartoTrimestreImportoDebito { get; set; }

        [Column("quarto_trimestre_credito_trimestre_precedente")]
        [Display(Name = "4° Trim. Credito Trimestre Precedente")]
        [DataType(DataType.Currency)]
        public decimal? QuartoTrimestreCreditoTrimestrePrecedente { get; set; }

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
