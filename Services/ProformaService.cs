using ConsultingGroup.Models;
using ConsultingGroup.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsultingGroup.Services
{
    public class ProformaService
    {
        private readonly ApplicationDbContext _context;

        public ProformaService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Genera automaticamente le proforma per un cliente basandosi sui dati del mandato
        /// </summary>
        public async Task<List<ProformaGenerata>> GeneraProformeAsync(
            int idCliente, 
            DateTime? dataMandato, 
            decimal? importoMandatoAnnuo, 
            string tipoProforma)
        {
            // Validazioni
            if (!dataMandato.HasValue || !importoMandatoAnnuo.HasValue || importoMandatoAnnuo <= 0)
            {
                return new List<ProformaGenerata>();
            }

            // Ottieni l'anno fatturazione corrente
            var annoFatturazioneCorrente = await _context.AnniFatturazione
                .FirstOrDefaultAsync(a => a.AnnoCorrente);

            if (annoFatturazioneCorrente == null)
            {
                throw new InvalidOperationException("Nessun anno fatturazione corrente impostato.");
            }

            // Rimuovi eventuali proforma esistenti per questo cliente e anno
            var proformeEsistenti = await _context.ProformeGenerate
                .Where(p => p.IdCliente == idCliente && p.IdAnnoFatturazione == annoFatturazioneCorrente.IdAnnoFatturazione)
                .ToListAsync();

            if (proformeEsistenti.Any())
            {
                _context.ProformeGenerate.RemoveRange(proformeEsistenti);
            }

            var proformeGenerate = new List<ProformaGenerata>();

            if (tipoProforma.ToLower() == "trimestrale")
            {
                proformeGenerate = GeneraProformeTrimestrale(
                    idCliente, 
                    annoFatturazioneCorrente.IdAnnoFatturazione,
                    annoFatturazioneCorrente.Anno,
                    dataMandato.Value, 
                    importoMandatoAnnuo.Value, 
                    tipoProforma);
            }
            else if (tipoProforma.ToLower() == "bimestrale")
            {
                proformeGenerate = GeneraProformeBimestrale(
                    idCliente, 
                    annoFatturazioneCorrente.IdAnnoFatturazione,
                    annoFatturazioneCorrente.Anno,
                    dataMandato.Value, 
                    importoMandatoAnnuo.Value, 
                    tipoProforma);
            }
            else if (tipoProforma.ToLower() == "mensile")
            {
                proformeGenerate = GeneraProformeMensile(
                    idCliente, 
                    annoFatturazioneCorrente.IdAnnoFatturazione,
                    annoFatturazioneCorrente.Anno,
                    dataMandato.Value, 
                    importoMandatoAnnuo.Value, 
                    tipoProforma);
            }

            if (proformeGenerate.Any())
            {
                try
                {
                    _context.ProformeGenerate.AddRange(proformeGenerate);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Log dettagliato dell'errore
                    var dettaglio = $"Errore nel salvataggio proforma: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        dettaglio += $" Inner: {ex.InnerException.Message}";
                    }
                    throw new InvalidOperationException(dettaglio, ex);
                }
            }

            return proformeGenerate;
        }

        /// <summary>
        /// Genera 4 proforma trimestrali
        /// </summary>
        private List<ProformaGenerata> GeneraProformeTrimestrale(
            int idCliente, 
            int idAnnoFatturazione,
            int anno,
            DateTime dataMandato, 
            decimal importoAnnuo, 
            string tipoProforma)
        {
            var proforms = new List<ProformaGenerata>();
            var importoTrimestrale = Math.Round(importoAnnuo / 4, 2);

            // Date trimestrali: 31/03, 30/06, 30/09, 31/12
            var dateScadenze = new List<DateTime>
            {
                new DateTime(anno, 3, 31),   // 31/03/xxxx
                new DateTime(anno, 6, 30),   // 30/06/xxxx
                new DateTime(anno, 9, 30),   // 30/09/xxxx
                new DateTime(anno, 12, 31)   // 31/12/xxxx
            };

            for (int i = 0; i < 4; i++)
            {
                // Per l'ultima rata, aggiusta l'importo per compensare eventuali arrotondamenti
                var importoRata = (i == 3) ? 
                    importoAnnuo - (importoTrimestrale * 3) : 
                    importoTrimestrale;

                proforms.Add(new ProformaGenerata
                {
                    IdCliente = idCliente,
                    IdAnnoFatturazione = idAnnoFatturazione,
                    DataMandato = dataMandato,
                    ImportoMandatoAnnuo = importoAnnuo,
                    TipoProforma = tipoProforma,
                    NumeroRata = i + 1,
                    DataScadenza = dateScadenze[i],
                    ImportoRata = importoRata,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return proforms;
        }

        /// <summary>
        /// Genera 2 proforma bimestrali
        /// </summary>
        private List<ProformaGenerata> GeneraProformeBimestrale(
            int idCliente, 
            int idAnnoFatturazione,
            int anno,
            DateTime dataMandato, 
            decimal importoAnnuo, 
            string tipoProforma)
        {
            var proforms = new List<ProformaGenerata>();
            var importoBimestrale = Math.Round(importoAnnuo / 2, 2);

            // Date bimestrali: 30/06 e 31/12
            var dateScadenze = new List<DateTime>
            {
                new DateTime(anno, 6, 30),   // 30/06/xxxx
                new DateTime(anno, 12, 31)   // 31/12/xxxx
            };

            for (int i = 0; i < 2; i++)
            {
                // Per la seconda rata, aggiusta l'importo per compensare eventuali arrotondamenti
                var importoRata = (i == 1) ? 
                    importoAnnuo - importoBimestrale : 
                    importoBimestrale;

                proforms.Add(new ProformaGenerata
                {
                    IdCliente = idCliente,
                    IdAnnoFatturazione = idAnnoFatturazione,
                    DataMandato = dataMandato,
                    ImportoMandatoAnnuo = importoAnnuo,
                    TipoProforma = tipoProforma,
                    NumeroRata = i + 1,
                    DataScadenza = dateScadenze[i],
                    ImportoRata = importoRata,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return proforms;
        }

        /// <summary>
        /// Genera 12 proforma mensili
        /// </summary>
        private List<ProformaGenerata> GeneraProformeMensile(
            int idCliente, 
            int idAnnoFatturazione,
            int anno,
            DateTime dataMandato, 
            decimal importoAnnuo, 
            string tipoProforma)
        {
            var proforms = new List<ProformaGenerata>();
            var importoMensile = Math.Round(importoAnnuo / 12, 2);

            // Date mensili: ultimo giorno di ogni mese
            var dateScadenze = new List<DateTime>();
            
            for (int mese = 1; mese <= 12; mese++)
            {
                // Calcola l'ultimo giorno del mese
                var ultimoGiorno = DateTime.DaysInMonth(anno, mese);
                dateScadenze.Add(new DateTime(anno, mese, ultimoGiorno));
            }

            for (int i = 0; i < 12; i++)
            {
                // Per l'ultima rata, aggiusta l'importo per compensare eventuali arrotondamenti
                var importoRata = (i == 11) ? 
                    importoAnnuo - (importoMensile * 11) : 
                    importoMensile;

                proforms.Add(new ProformaGenerata
                {
                    IdCliente = idCliente,
                    IdAnnoFatturazione = idAnnoFatturazione,
                    DataMandato = dataMandato,
                    ImportoMandatoAnnuo = importoAnnuo,
                    TipoProforma = tipoProforma,
                    NumeroRata = i + 1,
                    DataScadenza = dateScadenze[i],
                    ImportoRata = importoRata,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            return proforms;
        }

        /// <summary>
        /// Ottiene le proforma per un cliente
        /// </summary>
        public async Task<List<ProformaGenerata>> GetProformeClienteAsync(int idCliente, int? idAnnoFatturazione = null)
        {
            var query = _context.ProformeGenerate
                .Where(p => p.IdCliente == idCliente);

            if (idAnnoFatturazione.HasValue)
            {
                query = query.Where(p => p.IdAnnoFatturazione == idAnnoFatturazione.Value);
            }

            return await query
                .OrderBy(p => p.DataScadenza)
                .ToListAsync();
        }

        /// <summary>
        /// Elimina tutte le proforma per un cliente
        /// </summary>
        public async Task EliminaProformeClienteAsync(int idCliente, int? idAnnoFatturazione = null)
        {
            var query = _context.ProformeGenerate.Where(p => p.IdCliente == idCliente);

            if (idAnnoFatturazione.HasValue)
            {
                query = query.Where(p => p.IdAnnoFatturazione == idAnnoFatturazione.Value);
            }

            var proformeDaEliminare = await query.ToListAsync();
            
            if (proformeDaEliminare.Any())
            {
                _context.ProformeGenerate.RemoveRange(proformeDaEliminare);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Rigenera le proforma per un cliente dopo modifica dati mandato
        /// </summary>
        public async Task<List<ProformaGenerata>> RigeneraProformeAsync(
            int idCliente, 
            DateTime? nuovaDataMandato, 
            decimal? nuovoImportoMandatoAnnuo, 
            string nuovoTipoProforma)
        {
            try
            {
                // Prima elimina le proforma esistenti
                await EliminaProformeClienteAsync(idCliente);
                
                // Poi genera le nuove proforma
                return await GeneraProformeAsync(idCliente, nuovaDataMandato, nuovoImportoMandatoAnnuo, nuovoTipoProforma);
            }
            catch (Exception ex)
            {
                var dettaglio = $"Errore nella rigenerazione proforma per cliente {idCliente}: {ex.Message}";
                if (ex.InnerException != null)
                {
                    dettaglio += $" Inner: {ex.InnerException.Message}";
                }
                throw new InvalidOperationException(dettaglio, ex);
            }
        }

        /// <summary>
        /// Verifica se esistono proforma per un cliente
        /// </summary>
        public async Task<bool> HasProformeEsistentiAsync(int idCliente)
        {
            return await _context.ProformeGenerate
                .AnyAsync(p => p.IdCliente == idCliente);
        }

        /// <summary>
        /// Aggiorna lo stato di una proforma
        /// </summary>
        public async Task AggiornaStatoProformaAsync(int idProforma, bool? inviata = null, bool? pagata = null, DateTime? dataInvio = null, DateTime? dataPagamento = null)
        {
            var proforma = await _context.ProformeGenerate.FindAsync(idProforma);
            
            if (proforma != null)
            {
                if (inviata.HasValue) proforma.Inviata = inviata.Value;
                if (pagata.HasValue) proforma.Pagata = pagata.Value;
                if (dataInvio.HasValue) proforma.DataInvio = dataInvio.Value;
                if (dataPagamento.HasValue) proforma.DataPagamento = dataPagamento.Value;
                
                proforma.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
            }
        }
    }
}
