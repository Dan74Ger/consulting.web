using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Data;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;
using OfficeOpenXml;

namespace ConsultingGroup.Controllers
{
    [UserPermission("GestioneAttivita")]
    public class ContabilitaInternaTrimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContabilitaInternaTrimController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContabilitaInternaTrim
        public async Task<IActionResult> Index(int? annoSelezionato)
        {
            // Se non viene specificato un anno, usa l'anno corrente
            if (!annoSelezionato.HasValue)
            {
                var annoCorrente = await _context.AnniFiscali
                    .Where(a => a.AnnoCorrente)
                    .FirstOrDefaultAsync();
                
                if (annoCorrente != null)
                {
                    annoSelezionato = annoCorrente.Anno;
                }
                else
                {
                    // Fallback all'anno più recente
                    var ultimoAnno = await _context.AnniFiscali
                        .OrderByDescending(a => a.Anno)
                        .FirstOrDefaultAsync();
                    annoSelezionato = ultimoAnno?.Anno ?? DateTime.Now.Year;
                }
            }

            // Trova l'anno fiscale selezionato
            var annoFiscale = await _context.AnniFiscali
                .FirstOrDefaultAsync(a => a.Anno == annoSelezionato.Value);

            if (annoFiscale == null)
            {
                TempData["ErrorMessage"] = $"Anno fiscale {annoSelezionato} non trovato.";
                return RedirectToAction(nameof(Index));
            }

            // Carica TUTTE le contabilità esistenti per l'anno selezionato
            var contabilitaEsistenti = await _context.ContabilitaInternaTrimestrale
                .Include(c => c.Cliente)
                .Include(c => c.AnnoFiscale)
                .Where(c => c.IdAnno == annoFiscale.IdAnno)
                .OrderBy(c => c.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti con contabilità INTERNA
            // IMPORTANTE: Solo clienti con Contabilita = true (interno)
            var clientiSenzaContabilita = await _context.Clienti
                .Where(c => c.ContabilitaInternaTrimestrale && // Solo contabilità interna trimestrale
                           c.Attivo && // Solo clienti attivi
                           !_context.ContabilitaInternaTrimestrale.Any(ct => ct.IdCliente == c.IdCliente && ct.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le contabilità mancanti
            foreach (var cliente in clientiSenzaContabilita)
            {
                var nuovaContabilita = new ContabilitaInternaTrimestrale
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    CodiceContabilita = $"CONT-{cliente.IdCliente}-{annoSelezionato}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ContabilitaInternaTrimestrale.Add(nuovaContabilita);
            }

            if (clientiSenzaContabilita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le contabilità dopo aver aggiunto quelle nuove
                contabilitaEsistenti = await _context.ContabilitaInternaTrimestrale
                    .Include(c => c.Cliente)
                    .Include(c => c.AnnoFiscale)
                    .Where(c => c.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(c => c.Cliente!.NomeCliente)
                    .ToListAsync();
            }

            // Prepara ViewBag per il dropdown anni
            ViewBag.AnniFiscali = new SelectList(
                await _context.AnniFiscali.OrderByDescending(a => a.Anno).ToListAsync(),
                "Anno", "AnnoDescrizione", annoSelezionato);

            ViewBag.AnnoSelezionato = annoSelezionato;
            ViewBag.AnnoFiscale = annoFiscale;

            return View(contabilitaEsistenti);
        }

        // POST: ContabilitaInternaTrim/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<ContabilitaInternaTrimestrale> contabilita, int? annoSelezionato, IFormCollection form)
        {
            // Debug
            Console.WriteLine($"BulkUpdate chiamato - contabilita count: {contabilita?.Count ?? 0}");
            
            if (contabilita == null || !contabilita.Any())
            {
                Console.WriteLine("Nessuna contabilità ricevuta");
                TempData["ErrorMessage"] = "Nessun dato da aggiornare.";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }

            var aggiornamenti = 0;
            var errori = 0;

            foreach (var item in contabilita.Where(c => c.IdContabilitaInternaTrimestrale > 0))
            {
                try
                {
                    var esistente = await _context.ContabilitaInternaTrimestrale.FindAsync(item.IdContabilitaInternaTrimestrale);
                    if (esistente != null)
                    {
                        // Codice contabilità
                        esistente.CodiceContabilita = item.CodiceContabilita;

                        // Primo Trimestre
                        esistente.PrimoTrimestreUltimaFtVendita = item.PrimoTrimestreUltimaFtVendita;
                        esistente.PrimoTrimestreDataFt = item.PrimoTrimestreDataFt;
                        esistente.PrimoTrimestreLiqIvaImporto = item.PrimoTrimestreLiqIvaImporto;
                        esistente.PrimoTrimestreDebitoCredito = item.PrimoTrimestreDebitoCredito;
                        esistente.PrimoTrimestreF24Consegnato = item.PrimoTrimestreF24Consegnato;
                        esistente.PrimoTrimestreImportoCredito = item.PrimoTrimestreImportoCredito;
                        esistente.PrimoTrimestreImportoDebito = item.PrimoTrimestreImportoDebito;
                        esistente.PrimoTrimestreIvaVersare = item.PrimoTrimestreIvaVersare;
                        esistente.PrimoTrimestreCreditoAnnoPrecedente = item.PrimoTrimestreCreditoAnnoPrecedente;

                        // Secondo Trimestre
                        esistente.SecondoTrimestreUltimaFtVendita = item.SecondoTrimestreUltimaFtVendita;
                        esistente.SecondoTrimestreDataFt = item.SecondoTrimestreDataFt;
                        esistente.SecondoTrimestreLiqIvaImporto = item.SecondoTrimestreLiqIvaImporto;
                        esistente.SecondoTrimestreDebitoCredito = item.SecondoTrimestreDebitoCredito;
                        esistente.SecondoTrimestreF24Consegnato = item.SecondoTrimestreF24Consegnato;
                        esistente.SecondoTrimestreImportoCredito = item.SecondoTrimestreImportoCredito;
                        esistente.SecondoTrimestreImportoDebito = item.SecondoTrimestreImportoDebito;
                        esistente.SecondoTrimestreIvaVersare = item.SecondoTrimestreIvaVersare;
                        esistente.SecondoTrimestreCreditoTrimestrePrecedente = item.SecondoTrimestreCreditoTrimestrePrecedente;

                        // Terzo Trimestre
                        esistente.TerzoTrimestreUltimaFtVendita = item.TerzoTrimestreUltimaFtVendita;
                        esistente.TerzoTrimestreDataFt = item.TerzoTrimestreDataFt;
                        esistente.TerzoTrimestreLiqIvaImporto = item.TerzoTrimestreLiqIvaImporto;
                        esistente.TerzoTrimestreDebitoCredito = item.TerzoTrimestreDebitoCredito;
                        esistente.TerzoTrimestreF24Consegnato = item.TerzoTrimestreF24Consegnato;
                        esistente.TerzoTrimestreImportoCredito = item.TerzoTrimestreImportoCredito;
                        esistente.TerzoTrimestreImportoDebito = item.TerzoTrimestreImportoDebito;
                        esistente.TerzoTrimestreIvaVersare = item.TerzoTrimestreIvaVersare;
                        esistente.TerzoTrimestreCreditoTrimestrePrecedente = item.TerzoTrimestreCreditoTrimestrePrecedente;

                        // Quarto Trimestre
                        esistente.QuartoTrimestreUltimaFtVendita = item.QuartoTrimestreUltimaFtVendita;
                        esistente.QuartoTrimestreDataFt = item.QuartoTrimestreDataFt;
                        esistente.QuartoTrimestreLiqIvaImporto = item.QuartoTrimestreLiqIvaImporto;
                        esistente.QuartoTrimestreDebitoCredito = item.QuartoTrimestreDebitoCredito;
                        esistente.QuartoTrimestreF24Consegnato = item.QuartoTrimestreF24Consegnato;
                        esistente.QuartoTrimestreAccontoIva = item.QuartoTrimestreAccontoIva;
                        esistente.QuartoTrimestreImportoCredito = item.QuartoTrimestreImportoCredito;
                        esistente.QuartoTrimestreImportoDebito = item.QuartoTrimestreImportoDebito;
                        esistente.QuartoTrimestreIvaVersare = item.QuartoTrimestreIvaVersare;
                        esistente.QuartoTrimestreCreditoTrimestrePrecedente = item.QuartoTrimestreCreditoTrimestrePrecedente;

                        esistente.UpdatedAt = DateTime.UtcNow;
                        aggiornamenti++;
                    }
                }
                catch (Exception)
                {
                    errori++;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Salvati {aggiornamenti} aggiornamenti, {errori} errori");
                
                if (aggiornamenti > 0)
                {
                    TempData["SuccessMessage"] = $"Aggiornate {aggiornamenti} contabilità con successo.";
                }
                
                if (errori > 0)
                {
                    TempData["WarningMessage"] = $"{errori} aggiornamenti falliti.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante salvataggio: {ex.Message}");
                TempData["ErrorMessage"] = $"Errore durante il salvataggio: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { annoSelezionato });
        }

        // POST: ContabilitaInternaTrim/DeleteSingle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(int id)
        {
            try
            {
                // Trova la contabilità da eliminare con relazioni
                var contabilita = await _context.ContabilitaInternaTrimestrale
                    .Include(c => c.Cliente)
                    .Include(c => c.AnnoFiscale)
                    .FirstOrDefaultAsync(c => c.IdContabilitaInternaTrimestrale == id);

                if (contabilita == null)
                {
                    return Json(new { success = false, message = "Contabilità non trovata." });
                }

                // Verifica il vincolo: controllo se il cliente è attivo
                if (contabilita.Cliente?.Attivo == true)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Impossibile eliminare la contabilità per il cliente \"{contabilita.Cliente.NomeCliente}\" perché risulta ancora attivo. Disattivare prima il cliente dalla gestione clienti." 
                    });
                }

                var nomeCliente = contabilita.Cliente?.NomeCliente ?? "Cliente sconosciuto";
                var anno = contabilita.AnnoFiscale?.Anno ?? 0;

                // Elimina la contabilità
                _context.ContabilitaInternaTrimestrale.Remove(contabilita);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminata contabilità trimestrale per cliente: {nomeCliente} (Anno: {anno})");

                return Json(new { 
                    success = true, 
                    message = $"Contabilità trimestrale per il cliente \"{nomeCliente}\" eliminata con successo."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante eliminazione singola contabilità: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = $"Errore durante l'eliminazione: {ex.Message}" 
                });
            }
        }

        // POST: ContabilitaInternaTrim/DeleteAllForYear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllForYear(int anno)
        {
            try
            {
                // Trova l'anno fiscale
                var annoFiscale = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.Anno == anno);
                if (annoFiscale == null)
                {
                    return Json(new { success = false, message = $"Anno fiscale {anno} non trovato." });
                }

                // Trova tutte le contabilità per quell'anno
                var contabilitaDaEliminare = await _context.ContabilitaInternaTrimestrale
                    .Where(c => c.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = contabilitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessuna contabilità trovata per l'anno {anno}." });
                }

                // Elimina tutte le contabilità
                _context.ContabilitaInternaTrimestrale.RemoveRange(contabilitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminate {count} contabilità trimestrali per l'anno {anno}");

                return Json(new { 
                    success = true, 
                    deletedCount = count,
                    message = $"Eliminate con successo {count} contabilità dell'anno {anno}."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante eliminazione di massa: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = $"Errore durante l'eliminazione: {ex.Message}" 
                });
            }
        }

        // GET: ContabilitaInternaTrim/ExportExcel
        public async Task<IActionResult> ExportExcel(int anno)
        {
            try
            {
                // Trova l'anno fiscale
                var annoFiscale = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.Anno == anno);
                if (annoFiscale == null)
                {
                    TempData["ErrorMessage"] = $"Anno fiscale {anno} non trovato.";
                    return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
                }

                // Ottieni tutti i dati delle contabilità per l'anno
                var contabilita = await _context.ContabilitaInternaTrimestrale
                    .Include(c => c.Cliente)
                    .Include(c => c.AnnoFiscale)
                    .Where(c => c.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(c => c.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Contabilità Trimestrale - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:AJ1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers
                var headers = new[] { 
                    "Cliente", "Codice", "ATECO",
                    "T1 Ultima FT", "T1 Data FT", "T1 Liq IVA", "T1 Deb/Cred", "T1 F24", "T1 Cred.Anno Prec", "T1 Imp.Credito", "T1 Imp.Debito",
                    "T2 Ultima FT", "T2 Data FT", "T2 Liq IVA", "T2 Deb/Cred", "T2 F24", "T2 Cred.Trim Prec", "T2 Imp.Credito", "T2 Imp.Debito",
                    "T3 Ultima FT", "T3 Data FT", "T3 Liq IVA", "T3 Deb/Cred", "T3 F24", "T3 Cred.Trim Prec", "T3 Imp.Credito", "T3 Imp.Debito",
                    "T4 Ultima FT", "T4 Data FT", "T4 Liq IVA", "T4 Deb/Cred", "T4 F24", "T4 Acconto IVA", "T4 Cred.Trim Prec", "T4 Imp.Credito", "T4 Imp.Debito",
                    "Completamento %"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni per trimestre
                using (var t1Range = worksheet.Cells["D1:K1"]) // T1 (8 colonne: D, E, F, G, H, I, J, K)
                {
                    t1Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t1Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    t1Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t2Range = worksheet.Cells["L1:S1"]) // T2 (8 colonne: L, M, N, O, P, Q, R, S)
                {
                    t2Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t2Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    t2Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t3Range = worksheet.Cells["T1:AA1"]) // T3 (8 colonne: T, U, V, W, X, Y, Z, AA)
                {
                    t3Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t3Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    t3Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t4Range = worksheet.Cells["AB1:AI1"]) // T4 (8 colonne: AB, AC, AD, AE, AF, AG, AH, AI)
                {
                    t4Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t4Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    t4Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Dati
                for (int i = 0; i < contabilita.Count; i++)
                {
                    var item = contabilita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.CodiceContabilita ?? "";
                    worksheet.Cells[row, 3].Value = item.Cliente?.CodiceAteco ?? "";

                    // T1
                    worksheet.Cells[row, 4].Value = item.PrimoTrimestreUltimaFtVendita ?? "";
                    worksheet.Cells[row, 5].Value = item.PrimoTrimestreDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, 6].Value = item.PrimoTrimestreLiqIvaImporto;
                    worksheet.Cells[row, 7].Value = item.PrimoTrimestreDebitoCredito ?? "";
                    worksheet.Cells[row, 8].Value = item.PrimoTrimestreF24Consegnato ?? "";
                    worksheet.Cells[row, 9].Value = item.PrimoTrimestreCreditoAnnoPrecedente;
                    worksheet.Cells[row, 10].Value = item.PrimoTrimestreImportoCredito;
                    worksheet.Cells[row, 11].Value = item.PrimoTrimestreImportoDebito;
                    
                    // Formattazione T1 - Liq IVA Importo
                    if (item.PrimoTrimestreLiqIvaImporto.HasValue)
                    {
                        if (item.PrimoTrimestreLiqIvaImporto < 0)
                        {
                            worksheet.Cells[row, 6].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 6].Style.Font.Bold = true;
                        }
                        else if (item.PrimoTrimestreLiqIvaImporto > 0)
                        {
                            worksheet.Cells[row, 6].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T1 - Debito/Credito
                    if (item.PrimoTrimestreDebitoCredito == "debito")
                    {
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        worksheet.Cells[row, 7].Style.Font.Bold = true;
                    }
                    else if (item.PrimoTrimestreDebitoCredito == "credito")
                    {
                        worksheet.Cells[row, 7].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                    }
                    
                    // Formattazione T1 - Credito Anno Precedente
                    if (item.PrimoTrimestreCreditoAnnoPrecedente.HasValue)
                    {
                        if (item.PrimoTrimestreCreditoAnnoPrecedente < 0)
                        {
                            worksheet.Cells[row, 9].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 9].Style.Font.Bold = true;
                        }
                        else if (item.PrimoTrimestreCreditoAnnoPrecedente > 0)
                        {
                            worksheet.Cells[row, 9].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T1 - Importo Credito
                    if (item.PrimoTrimestreImportoCredito.HasValue)
                    {
                        if (item.PrimoTrimestreImportoCredito < 0)
                        {
                            worksheet.Cells[row, 10].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 10].Style.Font.Bold = true;
                        }
                        else if (item.PrimoTrimestreImportoCredito > 0)
                        {
                            worksheet.Cells[row, 10].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T1 - Importo Debito
                    if (item.PrimoTrimestreImportoDebito.HasValue)
                    {
                        if (item.PrimoTrimestreImportoDebito < 0)
                        {
                            worksheet.Cells[row, 11].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 11].Style.Font.Bold = true;
                        }
                        else if (item.PrimoTrimestreImportoDebito > 0)
                        {
                            worksheet.Cells[row, 11].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }

                    // T2
                    worksheet.Cells[row, 12].Value = item.SecondoTrimestreUltimaFtVendita ?? "";
                    worksheet.Cells[row, 13].Value = item.SecondoTrimestreDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, 14].Value = item.SecondoTrimestreLiqIvaImporto;
                    worksheet.Cells[row, 15].Value = item.SecondoTrimestreDebitoCredito ?? "";
                    worksheet.Cells[row, 16].Value = item.SecondoTrimestreF24Consegnato ?? "";
                    worksheet.Cells[row, 17].Value = item.SecondoTrimestreCreditoTrimestrePrecedente;
                    worksheet.Cells[row, 18].Value = item.SecondoTrimestreImportoCredito;
                    worksheet.Cells[row, 19].Value = item.SecondoTrimestreImportoDebito;
                    
                    // Formattazione T2 - Liq IVA Importo
                    if (item.SecondoTrimestreLiqIvaImporto.HasValue)
                    {
                        if (item.SecondoTrimestreLiqIvaImporto < 0)
                        {
                            worksheet.Cells[row, 13].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 13].Style.Font.Bold = true;
                        }
                        else if (item.SecondoTrimestreLiqIvaImporto > 0)
                        {
                            worksheet.Cells[row, 13].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T2 - Debito/Credito
                    if (item.SecondoTrimestreDebitoCredito == "debito")
                    {
                        worksheet.Cells[row, 14].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        worksheet.Cells[row, 14].Style.Font.Bold = true;
                    }
                    else if (item.SecondoTrimestreDebitoCredito == "credito")
                    {
                        worksheet.Cells[row, 14].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                    }
                    
                    // Formattazione T2 - Importo Credito
                    if (item.SecondoTrimestreImportoCredito.HasValue)
                    {
                        if (item.SecondoTrimestreImportoCredito < 0)
                        {
                            worksheet.Cells[row, 16].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 16].Style.Font.Bold = true;
                        }
                        else if (item.SecondoTrimestreImportoCredito > 0)
                        {
                            worksheet.Cells[row, 16].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T2 - Importo Debito
                    if (item.SecondoTrimestreImportoDebito.HasValue)
                    {
                        if (item.SecondoTrimestreImportoDebito < 0)
                        {
                            worksheet.Cells[row, 17].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 17].Style.Font.Bold = true;
                        }
                        else if (item.SecondoTrimestreImportoDebito > 0)
                        {
                            worksheet.Cells[row, 17].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }

                    // T3
                    worksheet.Cells[row, 18].Value = item.TerzoTrimestreUltimaFtVendita ?? "";
                    worksheet.Cells[row, 19].Value = item.TerzoTrimestreDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, 20].Value = item.TerzoTrimestreLiqIvaImporto;
                    worksheet.Cells[row, 21].Value = item.TerzoTrimestreDebitoCredito ?? "";
                    worksheet.Cells[row, 22].Value = item.TerzoTrimestreF24Consegnato ?? "";
                    worksheet.Cells[row, 23].Value = item.TerzoTrimestreImportoCredito;
                    worksheet.Cells[row, 24].Value = item.TerzoTrimestreImportoDebito;
                    
                    // Formattazione T3 - Liq IVA Importo
                    if (item.TerzoTrimestreLiqIvaImporto.HasValue)
                    {
                        if (item.TerzoTrimestreLiqIvaImporto < 0)
                        {
                            worksheet.Cells[row, 20].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 20].Style.Font.Bold = true;
                        }
                        else if (item.TerzoTrimestreLiqIvaImporto > 0)
                        {
                            worksheet.Cells[row, 20].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T3 - Debito/Credito
                    if (item.TerzoTrimestreDebitoCredito == "debito")
                    {
                        worksheet.Cells[row, 21].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        worksheet.Cells[row, 21].Style.Font.Bold = true;
                    }
                    else if (item.TerzoTrimestreDebitoCredito == "credito")
                    {
                        worksheet.Cells[row, 21].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                    }
                    
                    // Formattazione T3 - Importo Credito
                    if (item.TerzoTrimestreImportoCredito.HasValue)
                    {
                        if (item.TerzoTrimestreImportoCredito < 0)
                        {
                            worksheet.Cells[row, 23].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 23].Style.Font.Bold = true;
                        }
                        else if (item.TerzoTrimestreImportoCredito > 0)
                        {
                            worksheet.Cells[row, 23].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T3 - Importo Debito
                    if (item.TerzoTrimestreImportoDebito.HasValue)
                    {
                        if (item.TerzoTrimestreImportoDebito < 0)
                        {
                            worksheet.Cells[row, 24].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 24].Style.Font.Bold = true;
                        }
                        else if (item.TerzoTrimestreImportoDebito > 0)
                        {
                            worksheet.Cells[row, 24].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }

                    // T4
                    worksheet.Cells[row, 25].Value = item.QuartoTrimestreUltimaFtVendita ?? "";
                    worksheet.Cells[row, 26].Value = item.QuartoTrimestreDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, 27].Value = item.QuartoTrimestreLiqIvaImporto;
                    worksheet.Cells[row, 28].Value = item.QuartoTrimestreDebitoCredito ?? "";
                    worksheet.Cells[row, 29].Value = item.QuartoTrimestreF24Consegnato ?? "";
                    worksheet.Cells[row, 30].Value = item.QuartoTrimestreAccontoIva;
                    worksheet.Cells[row, 31].Value = item.QuartoTrimestreImportoCredito;
                    worksheet.Cells[row, 32].Value = item.QuartoTrimestreImportoDebito;
                    
                    // Formattazione T4 - Liq IVA Importo
                    if (item.QuartoTrimestreLiqIvaImporto.HasValue)
                    {
                        if (item.QuartoTrimestreLiqIvaImporto < 0)
                        {
                            worksheet.Cells[row, 27].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 27].Style.Font.Bold = true;
                        }
                        else if (item.QuartoTrimestreLiqIvaImporto > 0)
                        {
                            worksheet.Cells[row, 27].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T4 - Debito/Credito
                    if (item.QuartoTrimestreDebitoCredito == "debito")
                    {
                        worksheet.Cells[row, 28].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                        worksheet.Cells[row, 28].Style.Font.Bold = true;
                    }
                    else if (item.QuartoTrimestreDebitoCredito == "credito")
                    {
                        worksheet.Cells[row, 28].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                    }
                    
                    // Formattazione T4 - Importo Credito
                    if (item.QuartoTrimestreImportoCredito.HasValue)
                    {
                        if (item.QuartoTrimestreImportoCredito < 0)
                        {
                            worksheet.Cells[row, 30].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 30].Style.Font.Bold = true;
                        }
                        else if (item.QuartoTrimestreImportoCredito > 0)
                        {
                            worksheet.Cells[row, 30].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }
                    
                    // Formattazione T4 - Importo Debito
                    if (item.QuartoTrimestreImportoDebito.HasValue)
                    {
                        if (item.QuartoTrimestreImportoDebito < 0)
                        {
                            worksheet.Cells[row, 31].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                            worksheet.Cells[row, 31].Style.Font.Bold = true;
                        }
                        else if (item.QuartoTrimestreImportoDebito > 0)
                        {
                            worksheet.Cells[row, 31].Style.Font.Color.SetColor(System.Drawing.Color.DarkBlue);
                        }
                    }

                    // Calcola percentuale completamento (4 trimestri x 8 campi = 32 totali, il 4° trimestre ha un campo extra: Acconto IVA)
                    var totalFields = 29;
                    var completedFields = 0;
                    
                    // T1
                    if (!string.IsNullOrEmpty(item.PrimoTrimestreUltimaFtVendita)) completedFields++;
                    if (item.PrimoTrimestreDataFt.HasValue) completedFields++;
                    if (item.PrimoTrimestreLiqIvaImporto.HasValue) completedFields++;
                    if (!string.IsNullOrEmpty(item.PrimoTrimestreDebitoCredito)) completedFields++;
                    if (!string.IsNullOrEmpty(item.PrimoTrimestreF24Consegnato)) completedFields++;
                    if (item.PrimoTrimestreImportoCredito.HasValue) completedFields++;
                    if (item.PrimoTrimestreImportoDebito.HasValue) completedFields++;
                    
                    // T2
                    if (!string.IsNullOrEmpty(item.SecondoTrimestreUltimaFtVendita)) completedFields++;
                    if (item.SecondoTrimestreDataFt.HasValue) completedFields++;
                    if (item.SecondoTrimestreLiqIvaImporto.HasValue) completedFields++;
                    if (!string.IsNullOrEmpty(item.SecondoTrimestreDebitoCredito)) completedFields++;
                    if (!string.IsNullOrEmpty(item.SecondoTrimestreF24Consegnato)) completedFields++;
                    if (item.SecondoTrimestreImportoCredito.HasValue) completedFields++;
                    if (item.SecondoTrimestreImportoDebito.HasValue) completedFields++;
                    
                    // T3
                    if (!string.IsNullOrEmpty(item.TerzoTrimestreUltimaFtVendita)) completedFields++;
                    if (item.TerzoTrimestreDataFt.HasValue) completedFields++;
                    if (item.TerzoTrimestreLiqIvaImporto.HasValue) completedFields++;
                    if (!string.IsNullOrEmpty(item.TerzoTrimestreDebitoCredito)) completedFields++;
                    if (!string.IsNullOrEmpty(item.TerzoTrimestreF24Consegnato)) completedFields++;
                    if (item.TerzoTrimestreImportoCredito.HasValue) completedFields++;
                    if (item.TerzoTrimestreImportoDebito.HasValue) completedFields++;
                    
                    // T4
                    if (!string.IsNullOrEmpty(item.QuartoTrimestreUltimaFtVendita)) completedFields++;
                    if (item.QuartoTrimestreDataFt.HasValue) completedFields++;
                    if (item.QuartoTrimestreLiqIvaImporto.HasValue) completedFields++;
                    if (!string.IsNullOrEmpty(item.QuartoTrimestreDebitoCredito)) completedFields++;
                    if (!string.IsNullOrEmpty(item.QuartoTrimestreF24Consegnato)) completedFields++;
                    if (item.QuartoTrimestreAccontoIva.HasValue) completedFields++;
                    if (item.QuartoTrimestreImportoCredito.HasValue) completedFields++;
                    if (item.QuartoTrimestreImportoDebito.HasValue) completedFields++;

                    var percentage = (int)((double)completedFields / totalFields * 100);
                    worksheet.Cells[row, 33].Value = $"{percentage}%";

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 33];
                    if (percentage == 100)
                    {
                        percentageCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        percentageCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        percentageCell.Style.Font.Bold = true;
                    }
                    else if (percentage >= 50)
                    {
                        percentageCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        percentageCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }
                    else
                    {
                        percentageCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        percentageCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightPink);
                    }
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Aggiungi filtri automatici sulle intestazioni
                var dataRange = worksheet.Cells[1, 1, contabilita.Count + 1, headers.Length];
                worksheet.Tables.Add(dataRange, "ContabilitaTrimestraleData");
                var table = worksheet.Tables["ContabilitaTrimestraleData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"Contabilita_Trimestrale_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var fileBytes = package.GetAsByteArray();

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Errore durante l'export: {ex.Message}";
                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }
        }
    }
}
