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
    public class AttivitaIrapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttivitaIrapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttivitaIrap
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

            // Carica TUTTE le attività IRAP esistenti per l'anno selezionato
            var attivitaEsistenti = await _context.AttivitaIrap
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti che attivano ModIrap
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            var clientiSenzaAttivita = await _context.Clienti
                .Where(c => c.ModIrap && 
                           !_context.AttivitaIrap.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le attività mancanti
            foreach (var cliente in clientiSenzaAttivita)
            {
                var nuovaAttivita = new AttivitaIrap
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista, // Usa il professionista del cliente
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AttivitaIrap.Add(nuovaAttivita);
            }

            if (clientiSenzaAttivita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le attività dopo aver aggiunto quelle nuove
                attivitaEsistenti = await _context.AttivitaIrap
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

        // POST: AttivitaIrap/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate([FromForm] List<AttivitaIrap> attivita, int? annoSelezionato, IFormCollection form)
        {
            Console.WriteLine($"BulkUpdate IRAP called with {attivita?.Count ?? 0} items");
            
            if (attivita == null || !attivita.Any())
            {
                // Fallback: prova a parsare manualmente i dati dal form
                attivita = ParseAttivitaFromForm(form);
                Console.WriteLine($"Manual parsing resulted in {attivita.Count} items");
            }

            if (attivita == null || !attivita.Any())
            {
                Console.WriteLine("No data to update after all attempts");
                TempData["ErrorMessage"] = "Nessun dato da aggiornare.";
                return RedirectToAction(nameof(Index), new { annoSelezionato });
            }

            var aggiornamenti = 0;
            var errori = 0;

            foreach (var item in attivita.Where(a => a.IdAttivitaIrap > 0))
            {
                try
                {
                    Console.WriteLine($"Processing IRAP item ID: {item.IdAttivitaIrap}");
                    var esistente = await _context.AttivitaIrap.FindAsync(item.IdAttivitaIrap);
                    if (esistente != null)
                    {
                        // Aggiorna solo i campi modificabili
                        esistente.IdProfessionista = item.IdProfessionista;
                        esistente.CodiceDr = item.CodiceDr;
                        esistente.RaccoltaDocumenti = item.RaccoltaDocumenti;
                        esistente.FileIsa = item.FileIsa;
                        esistente.Ricevuta = item.Ricevuta;
                        esistente.Cciaa = item.Cciaa;
                        esistente.PecInvioDr = item.PecInvioDr;
                        esistente.DrFirmata = item.DrFirmata;
                        esistente.DrInserita = item.DrInserita;
                        esistente.DrInseritaData = item.DrInseritaData;
                        esistente.IsaDrInseriti = item.IsaDrInseriti;
                        esistente.IsaDrInseritiData = item.IsaDrInseritiData;
                        esistente.DrControllata = item.DrControllata;
                        esistente.DrControllataData = item.DrControllataData;
                        esistente.DrSpedita = item.DrSpedita;
                        esistente.DrSpeditaData = item.DrSpeditaData;
                        esistente.NumeroRateF24PrimoAccontoSaldo = item.NumeroRateF24PrimoAccontoSaldo;
                        esistente.F24PrimoAccontoSaldoConsegnato = item.F24PrimoAccontoSaldoConsegnato;
                        esistente.F24SecondoAcconto = item.F24SecondoAcconto;
                        esistente.F24SecondoAccontoConsegnato = item.F24SecondoAccontoConsegnato;
                        esistente.Note = item.Note;
                        esistente.UpdatedAt = DateTime.UtcNow;

                        aggiornamenti++;
                        Console.WriteLine($"Updated IRAP item ID: {item.IdAttivitaIrap}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating IRAP item ID {item.IdAttivitaIrap}: {ex.Message}");
                    errori++;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully saved {aggiornamenti} IRAP updates");

                if (aggiornamenti > 0)
                {
                    TempData["SuccessMessage"] = $"Aggiornate {aggiornamenti} attività IRAP con successo.";
                }
                
                if (errori > 0)
                {
                    TempData["WarningMessage"] = $"{errori} aggiornamenti falliti.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving IRAP changes: {ex.Message}");
                TempData["ErrorMessage"] = $"Errore durante il salvataggio: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { annoSelezionato });
        }

        private List<AttivitaIrap> ParseAttivitaFromForm(IFormCollection form)
        {
            var attivitaList = new List<AttivitaIrap>();
            var indices = new HashSet<int>();

            // Trova tutti gli indici disponibili
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("[") && key.Contains("]."))
                {
                    var indexStr = key.Substring(1, key.IndexOf(']') - 1);
                    if (int.TryParse(indexStr, out int index))
                    {
                        indices.Add(index);
                    }
                }
            }

            Console.WriteLine($"Found {indices.Count} indices in form for IRAP");

            foreach (var index in indices)
            {
                try
                {
                    var idKey = $"[{index}].IdAttivitaIrap";
                    if (form.ContainsKey(idKey) && int.TryParse(form[idKey], out int id))
                    {
                        var attivita = new AttivitaIrap { IdAttivitaIrap = id };

                        // Parse altri campi
                        if (form.ContainsKey($"[{index}].IdProfessionista") && int.TryParse(form[$"[{index}].IdProfessionista"], out int profId))
                            attivita.IdProfessionista = profId;

                        attivita.CodiceDr = form[$"[{index}].CodiceDr"].FirstOrDefault();
                        attivita.RaccoltaDocumenti = form[$"[{index}].RaccoltaDocumenti"].FirstOrDefault() ?? "da_richiedere";
                        attivita.Note = form[$"[{index}].Note"].FirstOrDefault();

                        // Parse boolean fields
                        attivita.FileIsa = form[$"[{index}].FileIsa"].ToString().Contains("true");
                        attivita.Ricevuta = form[$"[{index}].Ricevuta"].ToString().Contains("true");
                        attivita.Cciaa = form[$"[{index}].Cciaa"].ToString().Contains("true");
                        attivita.PecInvioDr = form[$"[{index}].PecInvioDr"].ToString().Contains("true");
                        attivita.DrFirmata = form[$"[{index}].DrFirmata"].ToString().Contains("true");
                        attivita.DrInserita = form[$"[{index}].DrInserita"].ToString().Contains("true");
                        attivita.IsaDrInseriti = form[$"[{index}].IsaDrInseriti"].ToString().Contains("true");
                        attivita.DrControllata = form[$"[{index}].DrControllata"].ToString().Contains("true");
                        attivita.DrSpedita = form[$"[{index}].DrSpedita"].ToString().Contains("true");
                        attivita.F24PrimoAccontoSaldoConsegnato = form[$"[{index}].F24PrimoAccontoSaldoConsegnato"].ToString().Contains("true");
                        attivita.F24SecondoAccontoConsegnato = form[$"[{index}].F24SecondoAccontoConsegnato"].ToString().Contains("true");

                        // Parse numeric fields
                        if (int.TryParse(form[$"[{index}].NumeroRateF24PrimoAccontoSaldo"], out int rate1))
                            attivita.NumeroRateF24PrimoAccontoSaldo = rate1;
                        if (int.TryParse(form[$"[{index}].F24SecondoAcconto"], out int rate2))
                            attivita.F24SecondoAcconto = rate2;

                        attivitaList.Add(attivita);
                        Console.WriteLine($"Parsed IRAP activity ID: {id}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing IRAP form data for index {index}: {ex.Message}");
                }
            }

            return attivitaList;
        }

        // POST: AttivitaIrap/DeleteAllForYear
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
                var attivitaDaEliminare = await _context.AttivitaIrap
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.AttivitaIrap.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminati {count} inserimenti Mod. IRAP per l'anno {anno}");

                return Json(new { 
                    success = true, 
                    deletedCount = count,
                    message = $"Eliminati con successo {count} inserimenti dell'anno {anno}."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante eliminazione di massa IRAP: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = $"Errore durante l'eliminazione: {ex.Message}" 
                });
            }
        }

        // GET: AttivitaIrap/ExportExcel
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

                // Ottieni tutti i dati delle attività IRAP per l'anno
                var attivita = await _context.AttivitaIrap
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Mod IRAP - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:U1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers per IRAP
                var headers = new[] { "Cliente", "ATECO", "Professionista", "Codice DR", "Raccolta Doc.",
                    "File ISA", "Ricevuta", "CCIAA", "PEC Invio DR", "DR Firmata", "DR Inserita", 
                    "ISA DR Inseriti", "DR Controllata", "DR Spedita", "Rate F24 1°", "F24 1° Consegnato", 
                    "F24 2° Acconto", "F24 2° Consegnato", "Note", "Stato", "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni IRAP
                using (var fileRange = worksheet.Cells["F1:H1"]) // File ISA, Ricevuta, CCIAA
                {
                    fileRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    fileRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    fileRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var drRange = worksheet.Cells["I1:N1"]) // PEC, DR Firmata, Inserita, ISA, Controllata, Spedita
                {
                    drRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    drRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    drRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var f24Range = worksheet.Cells["O1:R1"]) // F24
                {
                    f24Range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    f24Range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    f24Range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Dati
                for (int i = 0; i < attivita.Count; i++)
                {
                    var item = attivita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.Cliente?.CodiceAteco ?? "";
                    worksheet.Cells[row, 3].Value = item.Professionista?.NomeCompleto ?? "";
                    worksheet.Cells[row, 4].Value = item.CodiceDr ?? "";
                    worksheet.Cells[row, 5].Value = GetRaccoltaDocumentiText(item.RaccoltaDocumenti);

                    // Stati IRAP con simboli colorati
                    worksheet.Cells[row, 6].Value = item.FileIsa ? "✓" : "✗";
                    worksheet.Cells[row, 7].Value = item.Ricevuta ? "✓" : "✗";
                    worksheet.Cells[row, 8].Value = item.Cciaa ? "✓" : "✗";
                    worksheet.Cells[row, 9].Value = item.PecInvioDr ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.DrFirmata ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.DrInserita ? "✓" : "✗";
                    worksheet.Cells[row, 12].Value = item.IsaDrInseriti ? "✓" : "✗";
                    worksheet.Cells[row, 13].Value = item.DrControllata ? "✓" : "✗";
                    worksheet.Cells[row, 14].Value = item.DrSpedita ? "✓" : "✗";

                    // F24
                    worksheet.Cells[row, 15].Value = item.NumeroRateF24PrimoAccontoSaldo.ToString();
                    worksheet.Cells[row, 16].Value = item.F24PrimoAccontoSaldoConsegnato ? "✓" : "✗";
                    worksheet.Cells[row, 17].Value = item.F24SecondoAcconto.ToString();
                    worksheet.Cells[row, 18].Value = item.F24SecondoAccontoConsegnato ? "✓" : "✗";

                    worksheet.Cells[row, 19].Value = item.Note ?? "";
                    worksheet.Cells[row, 20].Value = item.StatoLavorazione;
                    worksheet.Cells[row, 21].Value = $"{item.PercentualeCompletamento}%";

                    // Applica formattazione ai simboli
                    for (int col = 6; col <= 18; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value?.ToString();
                        if (cellValue == "✓" || cellValue == "✗")
                        {
                            ApplySymbolFormatting(worksheet, row, col, cellValue == "✓");
                        }
                    }

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 21];
                    var percentage = item.PercentualeCompletamento;
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
                worksheet.Tables.Add(dataRange, "ModIrapData");
                var table = worksheet.Tables["ModIrapData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"ModIRAP_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
                "completata" => "Completata",
                _ => ""
            };
        }
    }
}
