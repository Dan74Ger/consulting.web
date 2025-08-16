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
    public class AttivitaLipeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttivitaLipeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttivitaLipe
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

            // Carica TUTTE le attività LIPE esistenti per l'anno selezionato
            var attivitaEsistenti = await _context.AttivitaLipe
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti che attivano LIPE
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            var clientiSenzaAttivita = await _context.Clienti
                .Where(c => c.Lipe && 
                           !_context.AttivitaLipe.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le attività mancanti
            foreach (var cliente in clientiSenzaAttivita)
            {
                var nuovaAttivita = new AttivitaLipe
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista, // Usa il professionista del cliente
                    T1CreatedAt = DateTime.UtcNow,
                    T1UpdatedAt = DateTime.UtcNow,
                    T2CreatedAt = DateTime.UtcNow,
                    T2UpdatedAt = DateTime.UtcNow,
                    T3CreatedAt = DateTime.UtcNow,
                    T3UpdatedAt = DateTime.UtcNow,
                    T4CreatedAt = DateTime.UtcNow,
                    T4UpdatedAt = DateTime.UtcNow
                };

                _context.AttivitaLipe.Add(nuovaAttivita);
            }

            if (clientiSenzaAttivita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le attività dopo aver aggiunto quelle nuove
                attivitaEsistenti = await _context.AttivitaLipe
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();
            }

            // Prepara ViewBag per il dropdown anni
            ViewBag.AnniFiscali = new SelectList(
                await _context.AnniFiscali.OrderByDescending(a => a.Anno).ToListAsync(),
                "Anno", "AnnoDescrizione", annoSelezionato);

            // Prepara ViewBag per il dropdown professionisti
            ViewBag.Professionisti = await _context.Professionisti
                .Where(p => p.Attivo)
                .OrderBy(p => p.Nome)
                .ThenBy(p => p.Cognome)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProfessionista.ToString(),
                    Text = $"{p.Nome} {p.Cognome}"
                })
                .ToListAsync();

            ViewBag.AnnoSelezionato = annoSelezionato;
            ViewBag.AnnoFiscale = annoFiscale;

            return View(attivitaEsistenti);
        }

        // POST: AttivitaLipe/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<AttivitaLipe> attivita, int? annoSelezionato, IFormCollection form)
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

            foreach (var item in attivita.Where(a => a.IdAttivitaLipe > 0))
            {
                try
                {
                    var esistente = await _context.AttivitaLipe.FindAsync(item.IdAttivitaLipe);
                    if (esistente != null)
                    {
                        // Aggiorna solo i campi modificabili
                        esistente.IdProfessionista = item.IdProfessionista;

                        // Trimestre 1
                        esistente.T1RaccoltaDocumenti = item.T1RaccoltaDocumenti;
                        esistente.T1LipeInserita = item.T1LipeInserita;
                        esistente.T1LipeControllata = item.T1LipeControllata;
                        esistente.T1LipeSpedita = item.T1LipeSpedita;
                        esistente.T1UpdatedAt = DateTime.UtcNow;

                        // Trimestre 2
                        esistente.T2RaccoltaDocumenti = item.T2RaccoltaDocumenti;
                        esistente.T2LipeInserita = item.T2LipeInserita;
                        esistente.T2LipeControllata = item.T2LipeControllata;
                        esistente.T2LipeSpedita = item.T2LipeSpedita;
                        esistente.T2UpdatedAt = DateTime.UtcNow;

                        // Trimestre 3
                        esistente.T3RaccoltaDocumenti = item.T3RaccoltaDocumenti;
                        esistente.T3LipeInserita = item.T3LipeInserita;
                        esistente.T3LipeControllata = item.T3LipeControllata;
                        esistente.T3LipeSpedita = item.T3LipeSpedita;
                        esistente.T3UpdatedAt = DateTime.UtcNow;

                        // Trimestre 4
                        esistente.T4RaccoltaDocumenti = item.T4RaccoltaDocumenti;
                        esistente.T4LipeInserita = item.T4LipeInserita;
                        esistente.T4LipeControllata = item.T4LipeControllata;
                        esistente.T4LipeSpedita = item.T4LipeSpedita;
                        esistente.T4UpdatedAt = DateTime.UtcNow;

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

        // POST: AttivitaLipe/DeleteSingle - NON USATO (gestito tramite checkbox anagrafica)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(int id)
        {
            try
            {
                // Trova l'attività da eliminare con relazioni
                var attivita = await _context.AttivitaLipe
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .FirstOrDefaultAsync(a => a.IdAttivitaLipe == id);

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
                _context.AttivitaLipe.Remove(attivita);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminata attività LIPE per cliente: {nomeCliente} (Anno: {anno})");

                return Json(new { 
                    success = true, 
                    message = $"Attività LIPE per il cliente \"{nomeCliente}\" eliminata con successo."
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

        // POST: AttivitaLipe/DeleteAllForYear
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
                var attivitaDaEliminare = await _context.AttivitaLipe
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.AttivitaLipe.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione (opzionale)
                Console.WriteLine($"Eliminati {count} inserimenti LIPE per l'anno {anno}");

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

        // GET: AttivitaLipe/ExportExcel
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

                // Ottieni tutti i dati delle attività LIPE per l'anno
                var attivita = await _context.AttivitaLipe
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"LIPE - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:N1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers
                var headers = new[] { "Cliente", "ATECO", "Professionista", 
                    "T1 Raccolta", "T1 Inserita", "T1 Controllata", "T1 Spedita",
                    "T2 Raccolta", "T2 Inserita", "T2 Controllata", "T2 Spedita",
                    "T3 Raccolta", "T3 Inserita", "T3 Controllata", "T3 Spedita",
                    "T4 Raccolta", "T4 Inserita", "T4 Controllata", "T4 Spedita",
                    "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni per trimestre
                using (var t1Range = worksheet.Cells["D1:G1"]) // T1
                {
                    t1Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t1Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    t1Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t2Range = worksheet.Cells["H1:K1"]) // T2
                {
                    t2Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t2Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    t2Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t3Range = worksheet.Cells["L1:O1"]) // T3
                {
                    t3Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t3Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    t3Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var t4Range = worksheet.Cells["P1:S1"]) // T4
                {
                    t4Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    t4Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    t4Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Dati
                for (int i = 0; i < attivita.Count; i++)
                {
                    var item = attivita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.Cliente?.CodiceAteco ?? "";
                    worksheet.Cells[row, 3].Value = item.Professionista?.NomeCompleto ?? "";

                    // T1
                    worksheet.Cells[row, 4].Value = GetRaccoltaDocumentiText(item.T1RaccoltaDocumenti);
                    worksheet.Cells[row, 5].Value = item.T1LipeInserita ? "✓" : "✗";
                    worksheet.Cells[row, 6].Value = item.T1LipeControllata ? "✓" : "✗";
                    worksheet.Cells[row, 7].Value = item.T1LipeSpedita ? "✓" : "✗";

                    // T2
                    worksheet.Cells[row, 8].Value = GetRaccoltaDocumentiText(item.T2RaccoltaDocumenti);
                    worksheet.Cells[row, 9].Value = item.T2LipeInserita ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.T2LipeControllata ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.T2LipeSpedita ? "✓" : "✗";

                    // T3
                    worksheet.Cells[row, 12].Value = GetRaccoltaDocumentiText(item.T3RaccoltaDocumenti);
                    worksheet.Cells[row, 13].Value = item.T3LipeInserita ? "✓" : "✗";
                    worksheet.Cells[row, 14].Value = item.T3LipeControllata ? "✓" : "✗";
                    worksheet.Cells[row, 15].Value = item.T3LipeSpedita ? "✓" : "✗";

                    // T4
                    worksheet.Cells[row, 16].Value = GetRaccoltaDocumentiText(item.T4RaccoltaDocumenti);
                    worksheet.Cells[row, 17].Value = item.T4LipeInserita ? "✓" : "✗";
                    worksheet.Cells[row, 18].Value = item.T4LipeControllata ? "✓" : "✗";
                    worksheet.Cells[row, 19].Value = item.T4LipeSpedita ? "✓" : "✗";

                    // Calcola percentuale completamento (4 trimestri x 3 passi = 12 totali)
                    var totalSteps = 12;
                    var completedSteps = 0;
                    if (item.T1LipeInserita) completedSteps++;
                    if (item.T1LipeControllata) completedSteps++;
                    if (item.T1LipeSpedita) completedSteps++;
                    if (item.T2LipeInserita) completedSteps++;
                    if (item.T2LipeControllata) completedSteps++;
                    if (item.T2LipeSpedita) completedSteps++;
                    if (item.T3LipeInserita) completedSteps++;
                    if (item.T3LipeControllata) completedSteps++;
                    if (item.T3LipeSpedita) completedSteps++;
                    if (item.T4LipeInserita) completedSteps++;
                    if (item.T4LipeControllata) completedSteps++;
                    if (item.T4LipeSpedita) completedSteps++;

                    var percentage = (int)((double)completedSteps / totalSteps * 100);
                    worksheet.Cells[row, 20].Value = $"{percentage}%";

                    // Applica formattazione ai simboli
                    ApplySymbolFormatting(worksheet, row, 5, item.T1LipeInserita);
                    ApplySymbolFormatting(worksheet, row, 6, item.T1LipeControllata);
                    ApplySymbolFormatting(worksheet, row, 7, item.T1LipeSpedita);
                    ApplySymbolFormatting(worksheet, row, 9, item.T2LipeInserita);
                    ApplySymbolFormatting(worksheet, row, 10, item.T2LipeControllata);
                    ApplySymbolFormatting(worksheet, row, 11, item.T2LipeSpedita);
                    ApplySymbolFormatting(worksheet, row, 13, item.T3LipeInserita);
                    ApplySymbolFormatting(worksheet, row, 14, item.T3LipeControllata);
                    ApplySymbolFormatting(worksheet, row, 15, item.T3LipeSpedita);
                    ApplySymbolFormatting(worksheet, row, 17, item.T4LipeInserita);
                    ApplySymbolFormatting(worksheet, row, 18, item.T4LipeControllata);
                    ApplySymbolFormatting(worksheet, row, 19, item.T4LipeSpedita);

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 20];
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
                worksheet.Tables.Add(dataRange, "LipeData");
                var table = worksheet.Tables["LipeData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"LIPE_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var fileBytes = package.GetAsByteArray();

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Errore durante l'export: {ex.Message}";
                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }
        }

        private static void ApplySymbolFormatting(OfficeOpenXml.ExcelWorksheet worksheet, int row, int col, bool isCompleted)
        {
            var cell = worksheet.Cells[row, col];
            cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cell.Style.Font.Size = isCompleted ? 14 : 10;
            cell.Style.Font.Bold = isCompleted;
            cell.Style.Font.Color.SetColor(isCompleted ? System.Drawing.Color.Green : System.Drawing.Color.Red);
        }

        private static string GetRaccoltaDocumentiText(string? raccoltaDocumenti)
        {
            return raccoltaDocumenti switch
            {
                "da_richiedere" => "Da Richiedere",
                "richiesti" => "Richiesti",
                "ricevuti" => "Ricevuti",
                _ => ""
            };
        }
    }
}
