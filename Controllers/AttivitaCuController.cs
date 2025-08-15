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
    public class AttivitaCuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttivitaCuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttivitaCu
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

            // Carica TUTTE le attività CU esistenti per l'anno selezionato
            var attivitaEsistenti = await _context.AttivitaCu
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti che attivano ModCu
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            var clientiSenzaAttivita = await _context.Clienti
                .Where(c => c.ModCu && 
                           !_context.AttivitaCu.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le attività mancanti
            foreach (var cliente in clientiSenzaAttivita)
            {
                var nuovaAttivita = new AttivitaCu
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista, // Usa il professionista del cliente
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AttivitaCu.Add(nuovaAttivita);
            }

            if (clientiSenzaAttivita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le attività dopo aver aggiunto quelle nuove
                attivitaEsistenti = await _context.AttivitaCu
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

        // POST: AttivitaCu/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate([FromForm] List<AttivitaCu> attivita, int? annoSelezionato, IFormCollection form)
        {
            Console.WriteLine($"BulkUpdate CU called with {attivita?.Count ?? 0} items");
            
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

            foreach (var item in attivita.Where(a => a.IdAttivitaCu > 0))
            {
                try
                {
                    Console.WriteLine($"Processing CU item ID: {item.IdAttivitaCu}");
                    var esistente = await _context.AttivitaCu.FindAsync(item.IdAttivitaCu);
                    if (esistente != null)
                    {
                        // Aggiorna solo i campi modificabili
                        esistente.IdProfessionista = item.IdProfessionista;
                        esistente.CuLavAutonomo = item.CuLavAutonomo;
                        esistente.CuUtili = item.CuUtili;
                        esistente.InvioCu = item.InvioCu;
                        esistente.NumCu = item.NumCu;
                        esistente.RicevutaCu = item.RicevutaCu;
                        esistente.InvioClienteMail = item.InvioClienteMail;
                        esistente.InvioClienteMailData = item.InvioClienteMailData;
                        esistente.Note = item.Note;
                        esistente.UpdatedAt = DateTime.UtcNow;

                        aggiornamenti++;
                        Console.WriteLine($"Updated CU item ID: {item.IdAttivitaCu}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating CU item ID {item.IdAttivitaCu}: {ex.Message}");
                    errori++;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Successfully saved {aggiornamenti} CU updates");

                if (aggiornamenti > 0)
                {
                    TempData["SuccessMessage"] = $"Aggiornate {aggiornamenti} attività CU con successo.";
                }
                
                if (errori > 0)
                {
                    TempData["WarningMessage"] = $"{errori} aggiornamenti falliti.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving CU changes: {ex.Message}");
                TempData["ErrorMessage"] = $"Errore durante il salvataggio: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new { annoSelezionato });
        }

        private List<AttivitaCu> ParseAttivitaFromForm(IFormCollection form)
        {
            var attivitaList = new List<AttivitaCu>();
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

            Console.WriteLine($"Found {indices.Count} indices in form for CU");

            foreach (var index in indices)
            {
                try
                {
                    var idKey = $"[{index}].IdAttivitaCu";
                    if (form.ContainsKey(idKey) && int.TryParse(form[idKey], out int id))
                    {
                        var attivita = new AttivitaCu { IdAttivitaCu = id };

                        // Parse altri campi
                        if (form.ContainsKey($"[{index}].IdProfessionista") && int.TryParse(form[$"[{index}].IdProfessionista"], out int profId))
                            attivita.IdProfessionista = profId;

                        if (form.ContainsKey($"[{index}].NumCu") && int.TryParse(form[$"[{index}].NumCu"], out int numCu))
                            attivita.NumCu = numCu;

                        attivita.Note = form[$"[{index}].Note"].FirstOrDefault();

                        // Parse boolean fields
                        attivita.CuLavAutonomo = form[$"[{index}].CuLavAutonomo"].ToString().Contains("true");
                        attivita.CuUtili = form[$"[{index}].CuUtili"].ToString().Contains("true");
                        attivita.InvioCu = form[$"[{index}].InvioCu"].ToString().Contains("true");
                        attivita.RicevutaCu = form[$"[{index}].RicevutaCu"].ToString().Contains("true");
                        attivita.InvioClienteMail = form[$"[{index}].InvioClienteMail"].ToString().Contains("true");

                        // Parse date fields
                        if (form.ContainsKey($"[{index}].InvioClienteMailData") && DateTime.TryParse(form[$"[{index}].InvioClienteMailData"], out DateTime mailData))
                            attivita.InvioClienteMailData = mailData;

                        attivitaList.Add(attivita);
                        Console.WriteLine($"Parsed CU activity ID: {id}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing CU form data for index {index}: {ex.Message}");
                }
            }

            return attivitaList;
        }

        // POST: AttivitaCu/DeleteAllForYear
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
                var attivitaDaEliminare = await _context.AttivitaCu
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.AttivitaCu.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminati {count} inserimenti Mod. CU per l'anno {anno}");

                return Json(new { 
                    success = true, 
                    deletedCount = count,
                    message = $"Eliminati con successo {count} inserimenti dell'anno {anno}."
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante eliminazione di massa CU: {ex.Message}");
                return Json(new { 
                    success = false, 
                    message = $"Errore durante l'eliminazione: {ex.Message}" 
                });
            }
        }

        // GET: AttivitaCu/ExportExcel
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

                // Ottieni tutti i dati delle attività CU per l'anno
                var attivita = await _context.AttivitaCu
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Mod CU - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:L1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers per CU
                var headers = new[] { "Cliente", "ATECO", "Professionista", "CU Lav. Autonomo", "CU Utili",
                    "Invio CU", "Numero CU", "Ricevuta CU", "Invio Cliente Mail", "Data Invio Cliente", 
                    "Note", "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni CU
                using (var cuRange = worksheet.Cells["D1:E1"]) // CU Lav/Utili
                {
                    cuRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cuRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    cuRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var invioRange = worksheet.Cells["F1:I1"]) // Invio e Ricevuta
                {
                    invioRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    invioRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    invioRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Dati
                for (int i = 0; i < attivita.Count; i++)
                {
                    var item = attivita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.Cliente?.CodiceAteco ?? "";
                    worksheet.Cells[row, 3].Value = item.Professionista?.NomeCompleto ?? "";

                    // Stati CU con simboli colorati
                    worksheet.Cells[row, 4].Value = item.CuLavAutonomo ? "✓" : "✗";
                    worksheet.Cells[row, 5].Value = item.CuUtili ? "✓" : "✗";
                    worksheet.Cells[row, 6].Value = item.InvioCu ? "✓" : "✗";
                    worksheet.Cells[row, 7].Value = item.NumCu?.ToString() ?? "";
                    worksheet.Cells[row, 8].Value = item.RicevutaCu ? "✓" : "✗";
                    worksheet.Cells[row, 9].Value = item.InvioClienteMail ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.InvioClienteMailData?.ToString("dd/MM/yyyy") ?? "";

                    worksheet.Cells[row, 11].Value = item.Note ?? "";
                    worksheet.Cells[row, 12].Value = $"{item.PercentualeCompletamento}%";

                    // Applica formattazione ai simboli
                    for (int col = 4; col <= 9; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Value?.ToString();
                        if (cellValue == "✓" || cellValue == "✗")
                        {
                            ApplySymbolFormatting(worksheet, row, col, cellValue == "✓");
                        }
                    }

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 12];
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
                worksheet.Tables.Add(dataRange, "ModCuData");
                var table = worksheet.Tables["ModCuData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"ModCU_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
    }
}
