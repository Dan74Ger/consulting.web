using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Data;
using ConsultingGroup.Models;
using ConsultingGroup.Attributes;
using OfficeOpenXml;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ConsultingGroup.Controllers
{
    [UserPermission("GestioneAttivita")]
    public class ContabilitaInternaMensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContabilitaInternaMensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContabilitaInternaMens
        public async Task<IActionResult> Index(int? annoSelezionato)
        {
            // Se non è specificato un anno, usa l'anno corrente
            annoSelezionato ??= DateTime.Now.Year;

            // Trova l'anno fiscale corrispondente
            var annoFiscale = await _context.AnniFiscali
                .FirstOrDefaultAsync(a => a.Anno == annoSelezionato);

            if (annoFiscale == null)
            {
                TempData["ErrorMessage"] = $"Anno fiscale {annoSelezionato} non trovato.";
                return RedirectToAction("Index", "GestioneAttivita");
            }

            // Recupera tutte le contabilità esistenti per l'anno selezionato
            var contabilitaEsistenti = await _context.ContabilitaInternaMensile
                .Include(c => c.Cliente)
                .Include(c => c.AnnoFiscale)
                .Where(c => c.IdAnno == annoFiscale.IdAnno)
                .OrderBy(c => c.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti con contabilità MENSILE
            // IMPORTANTE: Solo clienti con ContabilitaInternaMensile = true
            var clientiSenzaContabilita = await _context.Clienti
                .Where(c => c.ContabilitaInternaMensile && // Solo contabilità interna mensile
                           c.Attivo && // Solo clienti attivi
                           !_context.ContabilitaInternaMensile.Any(cm => cm.IdCliente == c.IdCliente && cm.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le contabilità mancanti
            foreach (var cliente in clientiSenzaContabilita)
            {
                var nuovaContabilita = new ContabilitaInternaMensile
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    CodiceContabilita = $"CONT-M-{cliente.IdCliente}-{annoSelezionato}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ContabilitaInternaMensile.Add(nuovaContabilita);
            }

            if (clientiSenzaContabilita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le contabilità dopo aver aggiunto quelle nuove
                contabilitaEsistenti = await _context.ContabilitaInternaMensile
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

        // GET: ExportExcel
        public async Task<IActionResult> ExportExcel(int? annoSelezionato)
        {
            annoSelezionato ??= DateTime.Now.Year;

            var annoFiscale = await _context.AnniFiscali
                .FirstOrDefaultAsync(a => a.Anno == annoSelezionato);

            if (annoFiscale == null)
            {
                TempData["ErrorMessage"] = $"Anno fiscale {annoSelezionato} non trovato.";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }

            var contabilita = await _context.ContabilitaInternaMensile
                .Include(c => c.Cliente)
                .Include(c => c.AnnoFiscale)
                .Where(c => c.IdAnno == annoFiscale.IdAnno)
                .OrderBy(c => c.Cliente!.NomeCliente)
                .ToListAsync();

            try
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Contabilità Mensile {annoSelezionato}");

                // Headers
                var headers = new[] { 
                    "Cliente", "Codice Contabilità",
                    // GENNAIO
                    "Gen Ultima FT", "Gen Data FT", "Gen Liq IVA", "Gen Deb/Cred", "Gen F24", "Gen Cred.Anno Prec", "Gen Imp.Credito", "Gen Imp.Debito",
                    // FEBBRAIO  
                    "Feb Ultima FT", "Feb Data FT", "Feb Liq IVA", "Feb Deb/Cred", "Feb F24", "Feb Cred.Mese Prec", "Feb Imp.Credito", "Feb Imp.Debito",
                    // MARZO
                    "Mar Ultima FT", "Mar Data FT", "Mar Liq IVA", "Mar Deb/Cred", "Mar F24", "Mar Cred.Mese Prec", "Mar Imp.Credito", "Mar Imp.Debito",
                    // APRILE
                    "Apr Ultima FT", "Apr Data FT", "Apr Liq IVA", "Apr Deb/Cred", "Apr F24", "Apr Cred.Mese Prec", "Apr Imp.Credito", "Apr Imp.Debito",
                    // DICEMBRE  
                    "Dic Ultima FT", "Dic Data FT", "Dic Liq IVA", "Dic Deb/Cred", "Dic F24", "Dic Acconto IVA", "Dic Cred.Mese Prec", "Dic Imp.Credito", "Dic Imp.Debito"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Data
                int row = 2;
                foreach (var item in contabilita)
                {
                    int col = 1;
                    worksheet.Cells[row, col++].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, col++].Value = item.CodiceContabilita ?? "";

                    // GENNAIO
                    worksheet.Cells[row, col++].Value = item.GennaioUltimaFtVendita ?? "";
                    worksheet.Cells[row, col++].Value = item.GennaioDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, col++].Value = item.GennaioLiqIvaImporto;
                    worksheet.Cells[row, col++].Value = item.GennaioDebitoCredito ?? "";
                    worksheet.Cells[row, col++].Value = item.GennaioF24Consegnato ?? "";
                    worksheet.Cells[row, col++].Value = item.GennaioCreditoAnnoPrecedente;
                    worksheet.Cells[row, col++].Value = item.GennaioImportoCredito;
                    worksheet.Cells[row, col++].Value = item.GennaioImportoDebito;

                    // FEBBRAIO
                    worksheet.Cells[row, col++].Value = item.FebbraioUltimaFtVendita ?? "";
                    worksheet.Cells[row, col++].Value = item.FebbraioDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, col++].Value = item.FebbraioLiqIvaImporto;
                    worksheet.Cells[row, col++].Value = item.FebbraioDebitoCredito ?? "";
                    worksheet.Cells[row, col++].Value = item.FebbraioF24Consegnato ?? "";
                    worksheet.Cells[row, col++].Value = item.FebbraioCreditoMesePrecedente;
                    worksheet.Cells[row, col++].Value = item.FebbraioImportoCredito;
                    worksheet.Cells[row, col++].Value = item.FebbraioImportoDebito;

                    // MARZO
                    worksheet.Cells[row, col++].Value = item.MarzoUltimaFtVendita ?? "";
                    worksheet.Cells[row, col++].Value = item.MarzoDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, col++].Value = item.MarzoLiqIvaImporto;
                    worksheet.Cells[row, col++].Value = item.MarzoDebitoCredito ?? "";
                    worksheet.Cells[row, col++].Value = item.MarzoF24Consegnato ?? "";
                    worksheet.Cells[row, col++].Value = item.MarzoCreditoMesePrecedente;
                    worksheet.Cells[row, col++].Value = item.MarzoImportoCredito;
                    worksheet.Cells[row, col++].Value = item.MarzoImportoDebito;

                    // APRILE
                    worksheet.Cells[row, col++].Value = item.AprileUltimaFtVendita ?? "";
                    worksheet.Cells[row, col++].Value = item.AprileDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, col++].Value = item.AprileLiqIvaImporto;
                    worksheet.Cells[row, col++].Value = item.AprileDebitoCredito ?? "";
                    worksheet.Cells[row, col++].Value = item.AprileF24Consegnato ?? "";
                    worksheet.Cells[row, col++].Value = item.AprileCreditoMesePrecedente;
                    worksheet.Cells[row, col++].Value = item.AprileImportoCredito;
                    worksheet.Cells[row, col++].Value = item.AprileImportoDebito;

                    // DICEMBRE
                    worksheet.Cells[row, col++].Value = item.DicembreUltimaFtVendita ?? "";
                    worksheet.Cells[row, col++].Value = item.DicembreDataFt?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, col++].Value = item.DicembreLiqIvaImporto;
                    worksheet.Cells[row, col++].Value = item.DicembreDebitoCredito ?? "";
                    worksheet.Cells[row, col++].Value = item.DicembreF24Consegnato ?? "";
                    worksheet.Cells[row, col++].Value = item.DicembreAccontoIva;
                    worksheet.Cells[row, col++].Value = item.DicembreCreditoMesePrecedente;
                    worksheet.Cells[row, col++].Value = item.DicembreImportoCredito;
                    worksheet.Cells[row, col++].Value = item.DicembreImportoDebito;

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileName = $"ContabilitaInternaMensile_{annoSelezionato}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Errore durante l'export Excel: {ex.Message}";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }
        }

        // POST: ContabilitaInternaMens/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<ContabilitaInternaMensile> contabilita, int? annoSelezionato, IFormCollection form)
        {
            try
            {
                // Debug dettagliato
                Console.WriteLine($"BulkUpdate chiamato - contabilita count: {contabilita?.Count ?? 0}");
                Console.WriteLine($"Anno selezionato: {annoSelezionato}");
                
                if (contabilita != null && contabilita.Any())
                {
                    foreach (var item in contabilita)
                    {
                        Console.WriteLine($"Ricevuto: ID={item.IdContabilitaInternaMensile}, Codice='{item.CodiceContabilita}'");
                    }
                }
                
                if (contabilita == null || !contabilita.Any())
                {
                    Console.WriteLine("Nessuna contabilità ricevuta");
                    
                    // Se è una richiesta AJAX, ritorna JSON
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = true, message = "Nessuna modifica da salvare" });
                    }
                    
                    TempData["ErrorMessage"] = "Nessun dato da aggiornare.";
                    return RedirectToAction(nameof(Index), new { annoSelezionato });
                }

                var aggiornamenti = 0;
                var errori = 0;

                foreach (var item in contabilita.Where(c => c.IdContabilitaInternaMensile > 0))
                {
                    try
                    {
                        Console.WriteLine($"DEBUG: Processing ID {item.IdContabilitaInternaMensile}, CodiceContabilita ricevuto: '{item.CodiceContabilita}'");
                        
                        var esistente = await _context.ContabilitaInternaMensile.FindAsync(item.IdContabilitaInternaMensile);
                        if (esistente != null)
                        {
                            Console.WriteLine($"DEBUG: CodiceContabilita PRIMA: '{esistente.CodiceContabilita}' -> DOPO: '{item.CodiceContabilita}'");
                            
                            // Codice contabilità
                            esistente.CodiceContabilita = item.CodiceContabilita;

                            // GENNAIO
                            esistente.GennaioUltimaFtVendita = item.GennaioUltimaFtVendita;
                            esistente.GennaioDataFt = item.GennaioDataFt;
                            esistente.GennaioLiqIvaImporto = item.GennaioLiqIvaImporto;
                            esistente.GennaioDebitoCredito = item.GennaioDebitoCredito;
                            esistente.GennaioF24Consegnato = item.GennaioF24Consegnato;
                            esistente.GennaioCreditoAnnoPrecedente = item.GennaioCreditoAnnoPrecedente;
                            esistente.GennaioImportoCredito = item.GennaioImportoCredito;
                            esistente.GennaioImportoDebito = item.GennaioImportoDebito;

                            // FEBBRAIO
                            esistente.FebbraioUltimaFtVendita = item.FebbraioUltimaFtVendita;
                            esistente.FebbraioDataFt = item.FebbraioDataFt;
                            esistente.FebbraioLiqIvaImporto = item.FebbraioLiqIvaImporto;
                            esistente.FebbraioDebitoCredito = item.FebbraioDebitoCredito;
                            esistente.FebbraioF24Consegnato = item.FebbraioF24Consegnato;
                            esistente.FebbraioCreditoMesePrecedente = item.FebbraioCreditoMesePrecedente;
                            esistente.FebbraioImportoCredito = item.FebbraioImportoCredito;
                            esistente.FebbraioImportoDebito = item.FebbraioImportoDebito;

                            // MARZO
                            esistente.MarzoUltimaFtVendita = item.MarzoUltimaFtVendita;
                            esistente.MarzoDataFt = item.MarzoDataFt;
                            esistente.MarzoLiqIvaImporto = item.MarzoLiqIvaImporto;
                            esistente.MarzoDebitoCredito = item.MarzoDebitoCredito;
                            esistente.MarzoF24Consegnato = item.MarzoF24Consegnato;
                            esistente.MarzoCreditoMesePrecedente = item.MarzoCreditoMesePrecedente;
                            esistente.MarzoImportoCredito = item.MarzoImportoCredito;
                            esistente.MarzoImportoDebito = item.MarzoImportoDebito;

                            // APRILE
                            esistente.AprileUltimaFtVendita = item.AprileUltimaFtVendita;
                            esistente.AprileDataFt = item.AprileDataFt;
                            esistente.AprileLiqIvaImporto = item.AprileLiqIvaImporto;
                            esistente.AprileDebitoCredito = item.AprileDebitoCredito;
                            esistente.AprileF24Consegnato = item.AprileF24Consegnato;
                            esistente.AprileCreditoMesePrecedente = item.AprileCreditoMesePrecedente;
                            esistente.AprileImportoCredito = item.AprileImportoCredito;
                            esistente.AprileImportoDebito = item.AprileImportoDebito;

                            // DICEMBRE (con Acconto IVA)
                            esistente.DicembreUltimaFtVendita = item.DicembreUltimaFtVendita;
                            esistente.DicembreDataFt = item.DicembreDataFt;
                            esistente.DicembreLiqIvaImporto = item.DicembreLiqIvaImporto;
                            esistente.DicembreDebitoCredito = item.DicembreDebitoCredito;
                            esistente.DicembreF24Consegnato = item.DicembreF24Consegnato;
                            esistente.DicembreAccontoIva = item.DicembreAccontoIva;
                            esistente.DicembreCreditoMesePrecedente = item.DicembreCreditoMesePrecedente;
                            esistente.DicembreImportoCredito = item.DicembreImportoCredito;
                            esistente.DicembreImportoDebito = item.DicembreImportoDebito;

                            esistente.UpdatedAt = DateTime.UtcNow;
                            aggiornamenti++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Errore aggiornamento contabilità ID {item.IdContabilitaInternaMensile}: {ex.Message}");
                        errori++;
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Salvati {aggiornamenti} aggiornamenti, {errori} errori");
                    
                    // Se è una richiesta AJAX, ritorna JSON
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        if (aggiornamenti > 0)
                        {
                            return Json(new { success = true, message = $"Salvati {aggiornamenti} aggiornamenti con successo!" });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Nessuna modifica da salvare" });
                        }
                    }
                    
                    if (aggiornamenti > 0)
                    {
                        TempData["SuccessMessage"] = $"Salvati {aggiornamenti} aggiornamenti con successo!";
                    }
                    else
                    {
                        TempData["InfoMessage"] = "Nessuna modifica da salvare.";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Errore durante il salvataggio: {ex.Message}");
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, message = "Errore durante il salvataggio." });
                    }
                    
                    TempData["ErrorMessage"] = "Errore durante il salvataggio.";
                }

                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore generale nel BulkUpdate: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = $"Errore durante il salvataggio: {ex.Message}" });
                }
                
                TempData["ErrorMessage"] = $"Errore durante il salvataggio: {ex.Message}";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }
        }

        // POST: DeleteAllForYear
        [HttpPost]
        public async Task<IActionResult> DeleteAllForYear(int anno)
        {
            try
            {
                var annoFiscale = await _context.AnniFiscali
                    .FirstOrDefaultAsync(a => a.Anno == anno);

                if (annoFiscale == null)
                {
                    return Json(new { success = false, message = "Anno fiscale non trovato." });
                }

                var contabilitaDaEliminare = await _context.ContabilitaInternaMensile
                    .Where(c => c.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                _context.ContabilitaInternaMensile.RemoveRange(contabilitaDaEliminare);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Eliminate {contabilitaDaEliminare.Count} attività per l'anno {anno}." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore durante l'eliminazione: {ex.Message}" });
            }
        }
    }
}
