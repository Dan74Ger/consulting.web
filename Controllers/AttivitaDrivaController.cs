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
    public class AttivitaDrivaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttivitaDrivaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttivitaDriva
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

            // Carica TUTTE le attività DRIVA esistenti per l'anno selezionato
            var attivitaEsistenti = await _context.AttivitaDriva
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti che attivano DRIVA
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            var clientiSenzaAttivita = await _context.Clienti
                .Where(c => c.Driva && 
                           !_context.AttivitaDriva.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le attività mancanti
            foreach (var cliente in clientiSenzaAttivita)
            {
                var nuovaAttivita = new AttivitaDriva
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista, // Usa il professionista del cliente
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AttivitaDriva.Add(nuovaAttivita);
            }

            if (clientiSenzaAttivita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le attività dopo aver aggiunto quelle nuove
                attivitaEsistenti = await _context.AttivitaDriva
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

        // POST: AttivitaDriva/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<AttivitaDriva> attivita, int? annoSelezionato, IFormCollection form)
        {
            if (attivita == null || !attivita.Any())
            {
                // Auto-save senza modifiche - ritorna OK senza fare nulla
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Nessuna modifica da salvare" });
                }
                TempData["ErrorMessage"] = "Nessun dato da aggiornare.";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }

            var aggiornamenti = 0;
            var errori = 0;

            foreach (var item in attivita.Where(a => a.IdAttivitaDriva > 0))
            {
                try
                {
                    var esistente = await _context.AttivitaDriva.FindAsync(item.IdAttivitaDriva);
                    if (esistente != null)
                    {
                        // Aggiorna solo i campi modificabili
                        esistente.IdProfessionista = item.IdProfessionista;
                        esistente.CodiceDrIva = item.CodiceDrIva;
                        esistente.AppuntamentoDataOra = item.AppuntamentoDataOra;
                        esistente.AccontoIvaTipo = item.AccontoIvaTipo;
                        esistente.AccontoIvaCreditoDebito = item.AccontoIvaCreditoDebito;
                        esistente.ImportoAccontoIva = item.ImportoAccontoIva;
                        esistente.F24AccontoIvaStato = item.F24AccontoIvaStato;
                        esistente.RaccoltaDocumenti = item.RaccoltaDocumenti;
                        esistente.DrivaInserita = item.DrivaInserita;
                        esistente.DrivaInseritaData = item.DrivaInseritaData;
                        esistente.DrivaControllata = item.DrivaControllata;
                        esistente.DrivaControllataData = item.DrivaControllataData;
                        esistente.DrivaCreditoDebito = item.DrivaCreditoDebito;
                        esistente.ImportoDrIva = item.ImportoDrIva;
                        esistente.F24DrivaConsegnato = item.F24DrivaConsegnato;
                        esistente.F24DrivaData = item.F24DrivaData;
                        esistente.DrVisto = item.DrVisto;
                        esistente.RicevutaDriva = item.RicevutaDriva;
                        esistente.DrivaSpedita = item.DrivaSpedita;
                        esistente.DrivaSpeditaData = item.DrivaSpeditaData;
                        esistente.TcgData = item.TcgData;
                        esistente.Note = item.Note;
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
                
                // Per richieste AJAX (auto-save), ritorna JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new {
                        success = true,
                        message = $"Aggiornate {aggiornamenti} attività",
                        updates = aggiornamenti,
                        errors = errori
                    });
                }

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
                // Per richieste AJAX, ritorna errore JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = $"Errore: {ex.Message}" });
                }
                
                TempData["ErrorMessage"] = $"Errore durante il salvataggio: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { annoSelezionato });
        }

        // POST: AttivitaDriva/DeleteSingle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(int id)
        {
            try
            {
                // Trova l'attività da eliminare con relazioni
                var attivita = await _context.AttivitaDriva
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .FirstOrDefaultAsync(a => a.IdAttivitaDriva == id);

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
                _context.AttivitaDriva.Remove(attivita);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminata attività DRIVA per cliente: {nomeCliente} (Anno: {anno})");

                return Json(new { 
                    success = true, 
                    message = $"Attività DRIVA per il cliente \"{nomeCliente}\" eliminata con successo."
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

        // POST: AttivitaDriva/DeleteAllForYear
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
                var attivitaDaEliminare = await _context.AttivitaDriva
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.AttivitaDriva.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione (opzionale)
                Console.WriteLine($"Eliminati {count} inserimenti DRIVA per l'anno {anno}");

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

        // GET: AttivitaDriva/ExportExcel
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

                // Ottieni tutti i dati delle attività DRIVA per l'anno
                var attivita = await _context.AttivitaDriva
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"DRIVA - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:T1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers
                var headers = new[] { "Cliente", "ATECO", "Professionista", "Codice DR IVA", "Raccolta Doc.",
                    "Acconto IVA Tipo", "Acconto IVA C/D", "Importo Acconto", "F24 Acconto Stato", 
                    "DRIVA Inserita", "DRIVA Controllata", "DRIVA Spedita", "DRIVA C/D", "Importo DR IVA",
                    "F24 DRIVA", "DR Visto", "Ricevuta", "TCG Data", "Note", "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni
                using (var accontoRange = worksheet.Cells["F1:I1"]) // Acconto IVA
                {
                    accontoRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    accontoRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    accontoRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var drivaRange = worksheet.Cells["J1:L1"]) // DRIVA Stati
                {
                    drivaRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    drivaRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    drivaRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var altriStatiRange = worksheet.Cells["M1:Q1"]) // Altri Stati
                {
                    altriStatiRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    altriStatiRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    altriStatiRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Dati
                for (int i = 0; i < attivita.Count; i++)
                {
                    var item = attivita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.Cliente?.CodiceAteco ?? "";
                    worksheet.Cells[row, 3].Value = item.Professionista?.NomeCompleto ?? "";
                    worksheet.Cells[row, 4].Value = item.CodiceDrIva ?? "";
                    worksheet.Cells[row, 5].Value = GetRaccoltaDocumentiText(item.RaccoltaDocumenti);

                    // Acconto IVA
                    worksheet.Cells[row, 6].Value = GetAccontoIvaTipoText(item.AccontoIvaTipo);
                    worksheet.Cells[row, 7].Value = GetCreditoDebitoText(item.AccontoIvaCreditoDebito);
                    worksheet.Cells[row, 8].Value = item.ImportoAccontoIva?.ToString("C") ?? "";
                    worksheet.Cells[row, 9].Value = GetF24StatoText(item.F24AccontoIvaStato);

                    // DRIVA Stati con simboli colorati
                    worksheet.Cells[row, 10].Value = item.DrivaInserita ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.DrivaControllata ? "✓" : "✗";
                    worksheet.Cells[row, 12].Value = item.DrivaSpedita ? "✓" : "✗";

                    // Altri campi
                    worksheet.Cells[row, 13].Value = GetCreditoDebitoText(item.DrivaCreditoDebito);
                    worksheet.Cells[row, 14].Value = item.ImportoDrIva?.ToString("C") ?? "";
                    worksheet.Cells[row, 15].Value = item.F24DrivaConsegnato ? "✓" : "✗";
                    worksheet.Cells[row, 16].Value = item.DrVisto ? "✓" : "✗";
                    worksheet.Cells[row, 17].Value = item.RicevutaDriva ? "✓" : "✗";
                    worksheet.Cells[row, 18].Value = item.TcgData?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[row, 19].Value = item.Note ?? "";

                    // Calcola percentuale completamento
                    var totalSteps = 8; // DRIVA Inserita, Controllata, Spedita, F24, DR Visto, Ricevuta, etc.
                    var completedSteps = 0;
                    if (item.DrivaInserita) completedSteps++;
                    if (item.DrivaControllata) completedSteps++;
                    if (item.DrivaSpedita) completedSteps++;
                    if (item.F24DrivaConsegnato) completedSteps++;
                    if (item.DrVisto) completedSteps++;
                    if (item.RicevutaDriva) completedSteps++;
                    if (!string.IsNullOrEmpty(item.AccontoIvaTipo)) completedSteps++;
                    if (item.TcgData.HasValue) completedSteps++;

                    var percentage = (int)((double)completedSteps / totalSteps * 100);
                    worksheet.Cells[row, 20].Value = $"{percentage}%";

                    // Applica formattazione ai simboli
                    ApplySymbolFormatting(worksheet, row, 10, item.DrivaInserita);
                    ApplySymbolFormatting(worksheet, row, 11, item.DrivaControllata);
                    ApplySymbolFormatting(worksheet, row, 12, item.DrivaSpedita);
                    ApplySymbolFormatting(worksheet, row, 15, item.F24DrivaConsegnato);
                    ApplySymbolFormatting(worksheet, row, 16, item.DrVisto);
                    ApplySymbolFormatting(worksheet, row, 17, item.RicevutaDriva);

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
                worksheet.Tables.Add(dataRange, "DrivaData");
                var table = worksheet.Tables["DrivaData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"DRIVA_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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

        private static string GetAccontoIvaTipoText(string? accontoIvaTipo)
        {
            return accontoIvaTipo switch
            {
                "storico" => "Storico",
                "analitico" => "Analitico",
                "al_20_12" => "Al 20/12",
                _ => ""
            };
        }

        private static string GetCreditoDebitoText(string? creditoDebito)
        {
            return creditoDebito switch
            {
                "credito" => "Credito",
                "debito" => "Debito",
                "zero" => "Zero",
                _ => ""
            };
        }

        private static string GetF24StatoText(string? f24Stato)
        {
            return f24Stato switch
            {
                "non_spedito" => "Non Spedito",
                "mail" => "Mail",
                "entratel" => "Entratel",
                _ => ""
            };
        }
    }
}
