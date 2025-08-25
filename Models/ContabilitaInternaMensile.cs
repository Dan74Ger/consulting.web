using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultingGroup.Models
{
    [Table("contabilita_interna_mensile")]
    public class ContabilitaInternaMensile
    {
        [Key]
        [Column("id_contabilita_interna_mensile")]
        public int IdContabilitaInternaMensile { get; set; }

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

        // GENNAIO
        [Column("gennaio_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Gen. Ultima FT Vendita")]
        public string? GennaioUltimaFtVendita { get; set; }

        [Column("gennaio_data_ft")]
        [Display(Name = "Gen. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? GennaioDataFt { get; set; }

        [Column("gennaio_liq_iva_importo")]
        [Display(Name = "Gen. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? GennaioLiqIvaImporto { get; set; }

        [Column("gennaio_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Gen. Debito/Credito")]
        public string? GennaioDebitoCredito { get; set; }

        [Column("gennaio_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Gen. F24 Consegnato")]
        public string? GennaioF24Consegnato { get; set; }

        [Column("gennaio_importo_credito")]
        [Display(Name = "Gen. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? GennaioImportoCredito { get; set; }

        [Column("gennaio_importo_debito")]
        [Display(Name = "Gen. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? GennaioImportoDebito { get; set; }

        [Column("gennaio_credito_anno_precedente")]
        [Display(Name = "Gen. Credito Anno Precedente")]
        [DataType(DataType.Currency)]
        public decimal? GennaioCreditoAnnoPrecedente { get; set; }

        // FEBBRAIO
        [Column("febbraio_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Feb. Ultima FT Vendita")]
        public string? FebbraioUltimaFtVendita { get; set; }

        [Column("febbraio_data_ft")]
        [Display(Name = "Feb. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? FebbraioDataFt { get; set; }

        [Column("febbraio_liq_iva_importo")]
        [Display(Name = "Feb. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? FebbraioLiqIvaImporto { get; set; }

        [Column("febbraio_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Feb. Debito/Credito")]
        public string? FebbraioDebitoCredito { get; set; }

        [Column("febbraio_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Feb. F24 Consegnato")]
        public string? FebbraioF24Consegnato { get; set; }

        [Column("febbraio_importo_credito")]
        [Display(Name = "Feb. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? FebbraioImportoCredito { get; set; }

        [Column("febbraio_importo_debito")]
        [Display(Name = "Feb. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? FebbraioImportoDebito { get; set; }

        [Column("febbraio_credito_mese_precedente")]
        [Display(Name = "Feb. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? FebbraioCreditoMesePrecedente { get; set; }

        // MARZO
        [Column("marzo_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Mar. Ultima FT Vendita")]
        public string? MarzoUltimaFtVendita { get; set; }

        [Column("marzo_data_ft")]
        [Display(Name = "Mar. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? MarzoDataFt { get; set; }

        [Column("marzo_liq_iva_importo")]
        [Display(Name = "Mar. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? MarzoLiqIvaImporto { get; set; }

        [Column("marzo_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Mar. Debito/Credito")]
        public string? MarzoDebitoCredito { get; set; }

        [Column("marzo_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Mar. F24 Consegnato")]
        public string? MarzoF24Consegnato { get; set; }

        [Column("marzo_importo_credito")]
        [Display(Name = "Mar. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? MarzoImportoCredito { get; set; }

        [Column("marzo_importo_debito")]
        [Display(Name = "Mar. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? MarzoImportoDebito { get; set; }

        [Column("marzo_credito_mese_precedente")]
        [Display(Name = "Mar. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? MarzoCreditoMesePrecedente { get; set; }

        // APRILE
        [Column("aprile_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Apr. Ultima FT Vendita")]
        public string? AprileUltimaFtVendita { get; set; }

        [Column("aprile_data_ft")]
        [Display(Name = "Apr. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? AprileDataFt { get; set; }

        [Column("aprile_liq_iva_importo")]
        [Display(Name = "Apr. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? AprileLiqIvaImporto { get; set; }

        [Column("aprile_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Apr. Debito/Credito")]
        public string? AprileDebitoCredito { get; set; }

        [Column("aprile_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Apr. F24 Consegnato")]
        public string? AprileF24Consegnato { get; set; }

        [Column("aprile_importo_credito")]
        [Display(Name = "Apr. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? AprileImportoCredito { get; set; }

        [Column("aprile_importo_debito")]
        [Display(Name = "Apr. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? AprileImportoDebito { get; set; }

        [Column("aprile_credito_mese_precedente")]
        [Display(Name = "Apr. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? AprileCreditoMesePrecedente { get; set; }

        // MAGGIO
        [Column("maggio_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Mag. Ultima FT Vendita")]
        public string? MaggioUltimaFtVendita { get; set; }

        [Column("maggio_data_ft")]
        [Display(Name = "Mag. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? MaggioDataFt { get; set; }

        [Column("maggio_liq_iva_importo")]
        [Display(Name = "Mag. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? MaggioLiqIvaImporto { get; set; }

        [Column("maggio_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Mag. Debito/Credito")]
        public string? MaggioDebitoCredito { get; set; }

        [Column("maggio_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Mag. F24 Consegnato")]
        public string? MaggioF24Consegnato { get; set; }

        [Column("maggio_importo_credito")]
        [Display(Name = "Mag. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? MaggioImportoCredito { get; set; }

        [Column("maggio_importo_debito")]
        [Display(Name = "Mag. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? MaggioImportoDebito { get; set; }

        [Column("maggio_credito_mese_precedente")]
        [Display(Name = "Mag. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? MaggioCreditoMesePrecedente { get; set; }

        // GIUGNO
        [Column("giugno_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Giu. Ultima FT Vendita")]
        public string? GiugnoUltimaFtVendita { get; set; }

        [Column("giugno_data_ft")]
        [Display(Name = "Giu. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? GiugnoDataFt { get; set; }

        [Column("giugno_liq_iva_importo")]
        [Display(Name = "Giu. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? GiugnoLiqIvaImporto { get; set; }

        [Column("giugno_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Giu. Debito/Credito")]
        public string? GiugnoDebitoCredito { get; set; }

        [Column("giugno_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Giu. F24 Consegnato")]
        public string? GiugnoF24Consegnato { get; set; }

        [Column("giugno_importo_credito")]
        [Display(Name = "Giu. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? GiugnoImportoCredito { get; set; }

        [Column("giugno_importo_debito")]
        [Display(Name = "Giu. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? GiugnoImportoDebito { get; set; }

        [Column("giugno_credito_mese_precedente")]
        [Display(Name = "Giu. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? GiugnoCreditoMesePrecedente { get; set; }

        // LUGLIO
        [Column("luglio_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Lug. Ultima FT Vendita")]
        public string? LuglioUltimaFtVendita { get; set; }

        [Column("luglio_data_ft")]
        [Display(Name = "Lug. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? LuglioDataFt { get; set; }

        [Column("luglio_liq_iva_importo")]
        [Display(Name = "Lug. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? LuglioLiqIvaImporto { get; set; }

        [Column("luglio_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Lug. Debito/Credito")]
        public string? LuglioDebitoCredito { get; set; }

        [Column("luglio_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Lug. F24 Consegnato")]
        public string? LuglioF24Consegnato { get; set; }

        [Column("luglio_importo_credito")]
        [Display(Name = "Lug. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? LuglioImportoCredito { get; set; }

        [Column("luglio_importo_debito")]
        [Display(Name = "Lug. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? LuglioImportoDebito { get; set; }

        [Column("luglio_credito_mese_precedente")]
        [Display(Name = "Lug. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? LuglioCreditoMesePrecedente { get; set; }

        // AGOSTO
        [Column("agosto_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Ago. Ultima FT Vendita")]
        public string? AgostoUltimaFtVendita { get; set; }

        [Column("agosto_data_ft")]
        [Display(Name = "Ago. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? AgostoDataFt { get; set; }

        [Column("agosto_liq_iva_importo")]
        [Display(Name = "Ago. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? AgostoLiqIvaImporto { get; set; }

        [Column("agosto_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Ago. Debito/Credito")]
        public string? AgostoDebitoCredito { get; set; }

        [Column("agosto_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Ago. F24 Consegnato")]
        public string? AgostoF24Consegnato { get; set; }

        [Column("agosto_importo_credito")]
        [Display(Name = "Ago. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? AgostoImportoCredito { get; set; }

        [Column("agosto_importo_debito")]
        [Display(Name = "Ago. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? AgostoImportoDebito { get; set; }

        [Column("agosto_credito_mese_precedente")]
        [Display(Name = "Ago. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? AgostoCreditoMesePrecedente { get; set; }

        // SETTEMBRE
        [Column("settembre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Set. Ultima FT Vendita")]
        public string? SettembreUltimaFtVendita { get; set; }

        [Column("settembre_data_ft")]
        [Display(Name = "Set. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? SettembreDataFt { get; set; }

        [Column("settembre_liq_iva_importo")]
        [Display(Name = "Set. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? SettembreLiqIvaImporto { get; set; }

        [Column("settembre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Set. Debito/Credito")]
        public string? SettembreDebitoCredito { get; set; }

        [Column("settembre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Set. F24 Consegnato")]
        public string? SettembreF24Consegnato { get; set; }

        [Column("settembre_importo_credito")]
        [Display(Name = "Set. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? SettembreImportoCredito { get; set; }

        [Column("settembre_importo_debito")]
        [Display(Name = "Set. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? SettembreImportoDebito { get; set; }

        [Column("settembre_credito_mese_precedente")]
        [Display(Name = "Set. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? SettembreCreditoMesePrecedente { get; set; }

        // OTTOBRE
        [Column("ottobre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Ott. Ultima FT Vendita")]
        public string? OttobreUltimaFtVendita { get; set; }

        [Column("ottobre_data_ft")]
        [Display(Name = "Ott. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? OttobreDataFt { get; set; }

        [Column("ottobre_liq_iva_importo")]
        [Display(Name = "Ott. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? OttobreLiqIvaImporto { get; set; }

        [Column("ottobre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Ott. Debito/Credito")]
        public string? OttobreDebitoCredito { get; set; }

        [Column("ottobre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Ott. F24 Consegnato")]
        public string? OttobreF24Consegnato { get; set; }

        [Column("ottobre_importo_credito")]
        [Display(Name = "Ott. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? OttobreImportoCredito { get; set; }

        [Column("ottobre_importo_debito")]
        [Display(Name = "Ott. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? OttobreImportoDebito { get; set; }

        [Column("ottobre_credito_mese_precedente")]
        [Display(Name = "Ott. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? OttobreCreditoMesePrecedente { get; set; }

        // NOVEMBRE
        [Column("novembre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Nov. Ultima FT Vendita")]
        public string? NovembreUltimaFtVendita { get; set; }

        [Column("novembre_data_ft")]
        [Display(Name = "Nov. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? NovembreDataFt { get; set; }

        [Column("novembre_liq_iva_importo")]
        [Display(Name = "Nov. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? NovembreLiqIvaImporto { get; set; }

        [Column("novembre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Nov. Debito/Credito")]
        public string? NovembreDebitoCredito { get; set; }

        [Column("novembre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Nov. F24 Consegnato")]
        public string? NovembreF24Consegnato { get; set; }

        [Column("novembre_importo_credito")]
        [Display(Name = "Nov. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? NovembreImportoCredito { get; set; }

        [Column("novembre_importo_debito")]
        [Display(Name = "Nov. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? NovembreImportoDebito { get; set; }

        [Column("novembre_credito_mese_precedente")]
        [Display(Name = "Nov. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? NovembreCreditoMesePrecedente { get; set; }

        // DICEMBRE (con Acconto IVA)
        [Column("dicembre_ultima_ft_vendita")]
        [StringLength(50)]
        [Display(Name = "Dic. Ultima FT Vendita")]
        public string? DicembreUltimaFtVendita { get; set; }

        [Column("dicembre_data_ft")]
        [Display(Name = "Dic. Data FT")]
        [DataType(DataType.Date)]
        public DateTime? DicembreDataFt { get; set; }

        [Column("dicembre_liq_iva_importo")]
        [Display(Name = "Dic. Liq. IVA Importo")]
        [DataType(DataType.Currency)]
        public decimal? DicembreLiqIvaImporto { get; set; }

        [Column("dicembre_debito_credito")]
        [StringLength(10)]
        [Display(Name = "Dic. Debito/Credito")]
        public string? DicembreDebitoCredito { get; set; }

        [Column("dicembre_f24_consegnato")]
        [StringLength(20)]
        [Display(Name = "Dic. F24 Consegnato")]
        public string? DicembreF24Consegnato { get; set; }

        [Column("dicembre_acconto_iva")]
        [Display(Name = "Dic. Acconto IVA")]
        [DataType(DataType.Currency)]
        public decimal? DicembreAccontoIva { get; set; }

        [Column("dicembre_importo_credito")]
        [Display(Name = "Dic. Importo Credito")]
        [DataType(DataType.Currency)]
        public decimal? DicembreImportoCredito { get; set; }

        [Column("dicembre_importo_debito")]
        [Display(Name = "Dic. Importo Debito")]
        [DataType(DataType.Currency)]
        public decimal? DicembreImportoDebito { get; set; }

        [Column("dicembre_credito_mese_precedente")]
        [Display(Name = "Dic. Credito Mese Precedente")]
        [DataType(DataType.Currency)]
        public decimal? DicembreCreditoMesePrecedente { get; set; }

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