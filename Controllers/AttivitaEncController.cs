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
    public class AttivitaEncController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttivitaEncController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttivitaEnc
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

            // Carica TUTTE le attività ENC esistenti per l'anno selezionato
            var attivitaEsistenti = await _context.AttivitaEnc
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            // Popolamento automatico per NUOVI clienti che attivano ModEnc
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            var clientiSenzaAttivita = await _context.Clienti
                .Where(c => c.ModEnc && 
                           !_context.AttivitaEnc.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == annoFiscale.IdAnno))
                .ToListAsync();

            // Crea automaticamente le attività mancanti
            foreach (var cliente in clientiSenzaAttivita)
            {
                var nuovaAttivita = new AttivitaEnc
                {
                    IdAnno = annoFiscale.IdAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista, // Usa il professionista del cliente
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AttivitaEnc.Add(nuovaAttivita);
            }

            if (clientiSenzaAttivita.Any())
            {
                await _context.SaveChangesAsync();
                
                // Ricarica le attività dopo aver aggiunto quelle nuove
                attivitaEsistenti = await _context.AttivitaEnc
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

        // POST: AttivitaEnc/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<AttivitaEnc> attivita, int? annoSelezionato, IFormCollection form)
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

            foreach (var item in attivita.Where(a => a.IdAttivitaEnc > 0))
            {
                try
                {
                    var esistente = await _context.AttivitaEnc.FindAsync(item.IdAttivitaEnc);
                    if (esistente != null)
                    {
                        // Aggiorna solo i campi modificabili secondo schema ENC
                        esistente.IdProfessionista = item.IdProfessionista;
                        esistente.AppuntamentoDataOra = item.AppuntamentoDataOra;
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

        // POST: AttivitaEnc/DeleteSingle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(int id)
        {
            try
            {
                // Trova l'attività da eliminare con relazioni
                var attivita = await _context.AttivitaEnc
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .FirstOrDefaultAsync(a => a.IdAttivitaEnc == id);

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
                _context.AttivitaEnc.Remove(attivita);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminata attività Mod. ENC per cliente: {nomeCliente} (Anno: {anno})");

                return Json(new { 
                    success = true, 
                    message = $"Attività Mod. ENC per il cliente \"{nomeCliente}\" eliminata con successo."
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

        // POST: AttivitaEnc/EliminaTuttiPerAnno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminaTuttiPerAnno(int anno)
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

                // Trova tutti gli inserimenti per quell'anno
                var attivitaDaEliminare = await _context.AttivitaEnc
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    TempData["WarningMessage"] = $"Nessun inserimento trovato per l'anno {anno}. La pagina è già pulita.";
                    return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
                }

                // Elimina tutti gli inserimenti
                _context.AttivitaEnc.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione (opzionale)
                Console.WriteLine($"Eliminati {count} inserimenti Mod. ENC per l'anno {anno}");

                TempData["SuccessMessage"] = $"✅ Eliminati con successo {count} inserimenti dell'anno {anno}. La pagina è stata ripulita e ricreata automaticamente per i clienti attivi.";
                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore durante eliminazione di massa: {ex.Message}");
                TempData["ErrorMessage"] = $"Errore durante l'eliminazione: {ex.Message}";
                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }
        }

        // GET: AttivitaEnc/ExportExcel
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

                // Ottieni tutti i dati delle attività ENC per l'anno
                var attivita = await _context.AttivitaEnc
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Mod ENC - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:S1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers per ENC
                var headers = new[] { "Cliente", "ATECO", "Professionista", "Appuntamento", "Codice DR", "Raccolta Doc.",
                    "File ISA", "ISA DR Inseriti", "DR Inserita", "DR Controllata", "DR Spedita", 
                    "Ricevuta", "PEC", "Firmata", "CCIAA", "Rate F24 1°", "F24 1° Consegnato", 
                    "F24 2° Acconto", "F24 2° Consegnato", "Note", "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni
                using (var isaRange = worksheet.Cells["G1:H1"]) // File ISA, ISA DR
                {
                    isaRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    isaRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Purple);
                    isaRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var drRange = worksheet.Cells["I1:K1"]) // DR Inserita, Controllata, Spedita
                {
                    drRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    drRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    drRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var altriStatiRange = worksheet.Cells["L1:O1"]) // Altri Stati
                {
                    altriStatiRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    altriStatiRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    altriStatiRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var f24Range = worksheet.Cells["P1:T1"]) // F24
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
                    worksheet.Cells[row, 4].Value = item.AppuntamentoDataOra?.ToString("dd/MM/yyyy HH:mm") ?? "";
                    worksheet.Cells[row, 5].Value = item.CodiceDr ?? "";
                    worksheet.Cells[row, 6].Value = GetRaccoltaDocumentiText(item.RaccoltaDocumenti);

                    // ISA con simboli colorati
                    worksheet.Cells[row, 7].Value = item.FileIsa ? "✓" : "✗";
                    worksheet.Cells[row, 8].Value = item.IsaDrInseriti ? "✓" : "✗";

                    // DR Stati con simboli colorati
                    worksheet.Cells[row, 9].Value = item.DrInserita ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.DrControllata ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.DrSpedita ? "✓" : "✗";

                    // Altri Stati con simboli colorati
                    worksheet.Cells[row, 12].Value = item.Ricevuta ? "✓" : "✗";
                    worksheet.Cells[row, 13].Value = item.PecInvioDr ? "✓" : "✗";
                    worksheet.Cells[row, 14].Value = item.DrFirmata ? "✓" : "✗";
                    worksheet.Cells[row, 15].Value = item.Cciaa ? "✓" : "✗";

                    // F24
                    worksheet.Cells[row, 16].Value = item.NumeroRateF24PrimoAccontoSaldo.ToString();
                    worksheet.Cells[row, 17].Value = item.F24PrimoAccontoSaldoConsegnato ? "✓" : "✗";
                    worksheet.Cells[row, 18].Value = item.F24SecondoAcconto.ToString();
                    worksheet.Cells[row, 19].Value = item.F24SecondoAccontoConsegnato ? "✓" : "✗";

                    worksheet.Cells[row, 20].Value = item.Note ?? "";

                    // Calcola percentuale completamento per ENC
                    var totalSteps = 10; // File ISA, ISA DR, DR Inserita, Controllata, Spedita, Ricevuta, PEC, Firmata, CCIAA, F24
                    var completedSteps = 0;
                    if (item.FileIsa) completedSteps++;
                    if (item.IsaDrInseriti) completedSteps++;
                    if (item.DrInserita) completedSteps++;
                    if (item.DrControllata) completedSteps++;
                    if (item.DrSpedita) completedSteps++;
                    if (item.Ricevuta) completedSteps++;
                    if (item.PecInvioDr) completedSteps++;
                    if (item.DrFirmata) completedSteps++;
                    if (item.Cciaa) completedSteps++;
                    if (item.F24PrimoAccontoSaldoConsegnato && item.F24SecondoAccontoConsegnato) completedSteps++;

                    var percentage = (int)((double)completedSteps / totalSteps * 100);
                    worksheet.Cells[row, 21].Value = $"{percentage}%";

                    // Applica formattazione ai simboli
                    ApplySymbolFormatting(worksheet, row, 7, item.FileIsa); // File ISA
                    ApplySymbolFormatting(worksheet, row, 8, item.IsaDrInseriti); // ISA DR
                    ApplySymbolFormatting(worksheet, row, 9, item.DrInserita); // DR Inserita
                    ApplySymbolFormatting(worksheet, row, 10, item.DrControllata); // DR Controllata
                    ApplySymbolFormatting(worksheet, row, 11, item.DrSpedita); // DR Spedita
                    ApplySymbolFormatting(worksheet, row, 12, item.Ricevuta); // Ricevuta
                    ApplySymbolFormatting(worksheet, row, 13, item.PecInvioDr); // PEC
                    ApplySymbolFormatting(worksheet, row, 14, item.DrFirmata); // Firmata
                    ApplySymbolFormatting(worksheet, row, 15, item.Cciaa); // CCIAA
                    ApplySymbolFormatting(worksheet, row, 17, item.F24PrimoAccontoSaldoConsegnato); // F24 1°
                    ApplySymbolFormatting(worksheet, row, 19, item.F24SecondoAccontoConsegnato); // F24 2°

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 21];
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
                worksheet.Tables.Add(dataRange, "ModEncData");
                var table = worksheet.Tables["ModEncData"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"ModENC_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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

