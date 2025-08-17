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
    public class AttivitaTrivaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttivitaTrivaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttivitaTriva
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

            // Carica TUTTE le attività TRIVA esistenti per l'anno selezionato
            var attivitaEsistenti = await _context.AttivitaTriva
                .Include(a => a.Cliente)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti che attivano MOD TR IVA
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            var clientiSenzaAttivita = await _context.Clienti
                .Where(c => c.ModTrIva && 
                           !_context.AttivitaTriva.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le attività mancanti
            foreach (var cliente in clientiSenzaAttivita)
            {
                var nuovaAttivita = new AttivitaTriva
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AttivitaTriva.Add(nuovaAttivita);
            }

            if (clientiSenzaAttivita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le attività dopo aver aggiunto quelle nuove
                attivitaEsistenti = await _context.AttivitaTriva
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();
            }

            // Prepara ViewBag per il dropdown anni
            ViewBag.AnniFiscali = new SelectList(
                await _context.AnniFiscali.OrderByDescending(a => a.Anno).ToListAsync(),
                "Anno", "AnnoDescrizione", annoSelezionato);

            ViewBag.AnnoSelezionato = annoSelezionato;
            ViewBag.AnnoFiscale = annoFiscale;

            return View(attivitaEsistenti);
        }

        // POST: AttivitaTriva/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<AttivitaTriva> attivita, int? annoSelezionato, IFormCollection form)
        {
            // Debug
            Console.WriteLine($"BulkUpdate chiamato - attivita count: {attivita?.Count ?? 0}");
            
            if (attivita == null || !attivita.Any())
            {
                Console.WriteLine("Nessuna attività ricevuta");
                TempData["ErrorMessage"] = "Nessun dato da aggiornare.";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }

            var aggiornamenti = 0;
            var errori = 0;

            foreach (var item in attivita.Where(a => a.IdModTrIva > 0))
            {
                try
                {
                    var esistente = await _context.AttivitaTriva.FindAsync(item.IdModTrIva);
                    if (esistente != null)
                    {
                        // Primo Trimestre
                        esistente.PrimoTrimestre = item.PrimoTrimestre;
                        esistente.PrimoTrimestreCompilato = item.PrimoTrimestreCompilato;
                        esistente.PrimoTrimestreSpedito = item.PrimoTrimestreSpedito;
                        esistente.PrimoTrimestreRicevuta = item.PrimoTrimestreRicevuta;
                        esistente.PrimoTrimestreMailCliente = item.PrimoTrimestreMailCliente;

                        // Secondo Trimestre
                        esistente.SecondoTrimestre = item.SecondoTrimestre;
                        esistente.SecondoTrimestreCompilato = item.SecondoTrimestreCompilato;
                        esistente.SecondoTrimestreSpedito = item.SecondoTrimestreSpedito;
                        esistente.SecondoTrimestreRicevuta = item.SecondoTrimestreRicevuta;
                        esistente.SecondoTrimestreMailCliente = item.SecondoTrimestreMailCliente;

                        // Terzo Trimestre
                        esistente.TerzoTrimestre = item.TerzoTrimestre;
                        esistente.TerzoTrimestreCompilato = item.TerzoTrimestreCompilato;
                        esistente.TerzoTrimestreSpedito = item.TerzoTrimestreSpedito;
                        esistente.TerzoTrimestreRicevuta = item.TerzoTrimestreRicevuta;
                        esistente.TerzoTrimestreMailCliente = item.TerzoTrimestreMailCliente;

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
                    TempData["SuccessMessage"] = $"Aggiornate {aggiornamenti} attività con successo.";
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

        // POST: AttivitaTriva/DeleteSingle - NON USATO (gestito tramite checkbox anagrafica)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(int id)
        {
            try
            {
                // Trova l'attività da eliminare con relazioni
                var attivita = await _context.AttivitaTriva
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .FirstOrDefaultAsync(a => a.IdModTrIva == id);

                if (attivita == null)
                {
                    return Json(new { success = false, message = "Attività non trovata." });
                }

                // Verifica il vincolo: controllo se il cliente è attivo
                if (attivita.Cliente?.Attivo == true)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Impossibile eliminare l'attività per il cliente \"{attivita.Cliente.NomeCliente}\" perché risulta ancora attivo. Disattivare prima il cliente dalla gestione clienti." 
                    });
                }

                var nomeCliente = attivita.Cliente?.NomeCliente ?? "Cliente sconosciuto";
                var anno = attivita.AnnoFiscale?.Anno ?? 0;

                // Elimina l'attività
                _context.AttivitaTriva.Remove(attivita);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminata attività TRIVA per cliente: {nomeCliente} (Anno: {anno})");

                return Json(new { 
                    success = true, 
                    message = $"Attività TRIVA per il cliente \"{nomeCliente}\" eliminata con successo."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante eliminazione singola attività: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = $"Errore durante l'eliminazione: {ex.Message}" 
                });
            }
        }

        // POST: AttivitaTriva/DeleteAllForYear
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

                // Trova tutti gli inserimenti per quell'anno
                var attivitaDaEliminare = await _context.AttivitaTriva
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.AttivitaTriva.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione (opzionale)
                Console.WriteLine($"Eliminati {count} inserimenti TRIVA per l'anno {anno}");

                return Json(new { 
                    success = true, 
                    deletedCount = count,
                    message = $"Eliminati con successo {count} inserimenti dell'anno {anno}."
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

        // GET: AttivitaTriva/ExportExcel
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

                // Ottieni tutti i dati delle attività TRIVA per l'anno
                var attivita = await _context.AttivitaTriva
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"TRIVA - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:Q1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers
                var headers = new[] { "Cliente", "ATECO",
                    "T1 Base", "T1 Compilato", "T1 Spedito", "T1 Ricevuta", "T1 Mail",
                    "T2 Base", "T2 Compilato", "T2 Spedito", "T2 Ricevuta", "T2 Mail",
                    "T3 Base", "T3 Compilato", "T3 Spedito", "T3 Ricevuta", "T3 Mail",
                    "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni per trimestre
                using (var t1Range = worksheet.Cells["C1:G1"]) // T1
                {
                    t1Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t1Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    t1Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t2Range = worksheet.Cells["H1:L1"]) // T2
                {
                    t2Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t2Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    t2Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t3Range = worksheet.Cells["M1:Q1"]) // T3
                {
                    t3Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t3Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    t3Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Dati
                for (int i = 0; i < attivita.Count; i++)
                {
                    var item = attivita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.Cliente?.CodiceAteco ?? "";

                    // T1
                    worksheet.Cells[row, 3].Value = item.PrimoTrimestre ? "✓" : "✗";
                    worksheet.Cells[row, 4].Value = item.PrimoTrimestreCompilato ? "✓" : "✗";
                    worksheet.Cells[row, 5].Value = item.PrimoTrimestreSpedito ? "✓" : "✗";
                    worksheet.Cells[row, 6].Value = item.PrimoTrimestreRicevuta ? "✓" : "✗";
                    worksheet.Cells[row, 7].Value = item.PrimoTrimestreMailCliente ? "✓" : "✗";

                    // T2
                    worksheet.Cells[row, 8].Value = item.SecondoTrimestre ? "✓" : "✗";
                    worksheet.Cells[row, 9].Value = item.SecondoTrimestreCompilato ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.SecondoTrimestreSpedito ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.SecondoTrimestreRicevuta ? "✓" : "✗";
                    worksheet.Cells[row, 12].Value = item.SecondoTrimestreMailCliente ? "✓" : "✗";

                    // T3
                    worksheet.Cells[row, 13].Value = item.TerzoTrimestre ? "✓" : "✗";
                    worksheet.Cells[row, 14].Value = item.TerzoTrimestreCompilato ? "✓" : "✗";
                    worksheet.Cells[row, 15].Value = item.TerzoTrimestreSpedito ? "✓" : "✗";
                    worksheet.Cells[row, 16].Value = item.TerzoTrimestreRicevuta ? "✓" : "✗";
                    worksheet.Cells[row, 17].Value = item.TerzoTrimestreMailCliente ? "✓" : "✗";

                    // Calcola percentuale completamento (3 trimestri x 5 passi = 15 totali)
                    var totalSteps = 15;
                    var completedSteps = 0;
                    if (item.PrimoTrimestre) completedSteps++;
                    if (item.PrimoTrimestreCompilato) completedSteps++;
                    if (item.PrimoTrimestreSpedito) completedSteps++;
                    if (item.PrimoTrimestreRicevuta) completedSteps++;
                    if (item.PrimoTrimestreMailCliente) completedSteps++;
                    if (item.SecondoTrimestre) completedSteps++;
                    if (item.SecondoTrimestreCompilato) completedSteps++;
                    if (item.SecondoTrimestreSpedito) completedSteps++;
                    if (item.SecondoTrimestreRicevuta) completedSteps++;
                    if (item.SecondoTrimestreMailCliente) completedSteps++;
                    if (item.TerzoTrimestre) completedSteps++;
                    if (item.TerzoTrimestreCompilato) completedSteps++;
                    if (item.TerzoTrimestreSpedito) completedSteps++;
                    if (item.TerzoTrimestreRicevuta) completedSteps++;
                    if (item.TerzoTrimestreMailCliente) completedSteps++;

                    var percentage = (int)((double)completedSteps / totalSteps * 100);
                    worksheet.Cells[row, 18].Value = $"{percentage}%";

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 18];
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
                var dataRange = worksheet.Cells[1, 1, attivita.Count + 1, headers.Length];
                worksheet.Tables.Add(dataRange, "TrivaData");
                var table = worksheet.Tables["TrivaData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"TRIVA_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
