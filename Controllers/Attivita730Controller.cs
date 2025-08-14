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
    public class Attivita730Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Attivita730Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attivita730
        public async Task<IActionResult> Index(int? annoSelezionato)
        {
            // Se nessun anno è selezionato, usa l'anno corrente
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
                    // Se non c'è un anno corrente, usa l'ultimo anno disponibile
                    var ultimoAnno = await _context.AnniFiscali
                        .OrderByDescending(a => a.Anno)
                        .FirstOrDefaultAsync();
                    annoSelezionato = ultimoAnno?.Anno ?? DateTime.Now.Year;
                }
            }

            // Ottieni l'anno fiscale selezionato
            var annoFiscale = await _context.AnniFiscali
                .FirstOrDefaultAsync(a => a.Anno == annoSelezionato);

            if (annoFiscale == null)
            {
                TempData["ErrorMessage"] = "Anno fiscale non trovato.";
                return RedirectToAction(nameof(Index));
            }

            // Carica TUTTE le attività 730 esistenti per l'anno selezionato
            // IMPORTANTE: Non filtriamo per Mod730 = true, carichiamo tutte le attività esistenti
            var attivita730 = await _context.Attivita730
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            Console.WriteLine($"DEBUG: Caricate {attivita730.Count} attività 730 per l'anno {annoFiscale.Anno}");

            // Popolamento automatico per NUOVI clienti che attivano Mod730
            // IMPORTANTE: Crea solo nuove attività, non rimuove quelle esistenti
            await PopolaDaClienti(annoFiscale.IdAnno);

            // Ricarica dopo il popolamento per includere le nuove attività
            attivita730 = await _context.Attivita730
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            Console.WriteLine($"DEBUG: Dopo popolamento: {attivita730.Count} attività 730 totali");

            // Prepara i dati per la view
            ViewData["AnnoSelezionato"] = annoSelezionato;
            ViewData["AnnoFiscale"] = annoFiscale;
            ViewData["AnniFiscali"] = new SelectList(
                await _context.AnniFiscali.OrderByDescending(a => a.Anno).ToListAsync(),
                "Anno", "AnnoDescrizione", annoSelezionato);

            // Lista professionisti per la dropdown inline
            ViewBag.Professionisti = new SelectList(
                await _context.Professionisti
                    .Where(p => p.Attivo)
                    .OrderBy(p => p.Nome)
                    .ThenBy(p => p.Cognome)
                    .ToListAsync(),
                "IdProfessionista", "NomeCompleto");

            ViewData["Title"] = $"Mod. 730 - Anno {annoSelezionato}";
            
            return View(attivita730);
        }

        // GET: Attivita730/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attivita730 = await _context.Attivita730
                .Include(a => a.AnnoFiscale)
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .FirstOrDefaultAsync(m => m.IdAttivita730 == id);

            if (attivita730 == null)
            {
                return NotFound();
            }

            return View(attivita730);
        }

        // GET: Attivita730/Create
        public async Task<IActionResult> Create(int? annoId)
        {
            // Prepara le dropdown
            await PreparaDropdowns(annoId);
            
            // Imposta l'anno se fornito
            var model = new Attivita730();
            if (annoId.HasValue)
            {
                model.IdAnno = annoId.Value;
            }

            return View(model);
        }

        // POST: Attivita730/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAnno,IdCliente,IdProfessionista,CodiceDr,RaccoltaDocumenti,DrInserita,DrInseritaData,DrControllata,DrControllataData,DrSpedita,DrSpeditaData,RicevutaDr730,PecInvioDr,DrFirmata,Note")] Attivita730 attivita730)
        {
            if (ModelState.IsValid)
            {
                // Controlla che il cliente abbia Mod730 = true
                var cliente = await _context.Clienti.FindAsync(attivita730.IdCliente);
                if (cliente == null || !cliente.Mod730)
                {
                    ModelState.AddModelError("IdCliente", "Il cliente selezionato non ha il servizio Mod. 730 attivo.");
                    await PreparaDropdowns(attivita730.IdAnno);
                    return View(attivita730);
                }

                // Controlla duplicati
                var esisteGia = await _context.Attivita730
                    .AnyAsync(a => a.IdAnno == attivita730.IdAnno && a.IdCliente == attivita730.IdCliente);

                if (esisteGia)
                {
                    ModelState.AddModelError("IdCliente", "Esiste già un'attività 730 per questo cliente nell'anno selezionato.");
                    await PreparaDropdowns(attivita730.IdAnno);
                    return View(attivita730);
                }

                attivita730.CreatedAt = DateTime.UtcNow;
                attivita730.UpdatedAt = DateTime.UtcNow;

                _context.Add(attivita730);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Attività 730 creata con successo.";
                return RedirectToAction(nameof(Index), new { annoSelezionato = attivita730.AnnoFiscale?.Anno });
            }

            await PreparaDropdowns(attivita730.IdAnno);
            return View(attivita730);
        }

        // GET: Attivita730/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attivita730 = await _context.Attivita730
                .Include(a => a.AnnoFiscale)
                .FirstOrDefaultAsync(a => a.IdAttivita730 == id);

            if (attivita730 == null)
            {
                return NotFound();
            }

            await PreparaDropdowns(attivita730.IdAnno);
            return View(attivita730);
        }

        // POST: Attivita730/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAttivita730,IdAnno,IdCliente,IdProfessionista,CodiceDr,RaccoltaDocumenti,DrInserita,DrInseritaData,DrControllata,DrControllataData,DrSpedita,DrSpeditaData,RicevutaDr730,PecInvioDr,DrFirmata,Note,CreatedAt")] Attivita730 attivita730)
        {
            if (id != attivita730.IdAttivita730)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    attivita730.UpdatedAt = DateTime.UtcNow;
                    _context.Update(attivita730);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Attività 730 aggiornata con successo.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Attivita730Exists(attivita730.IdAttivita730))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var anno = await _context.Attivita730
                    .Where(a => a.IdAttivita730 == id)
                    .Include(a => a.AnnoFiscale)
                    .Select(a => a.AnnoFiscale!.Anno)
                    .FirstOrDefaultAsync();

                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }

            await PreparaDropdowns(attivita730.IdAnno);
            return View(attivita730);
        }

        // GET: Attivita730/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attivita730 = await _context.Attivita730
                .Include(a => a.AnnoFiscale)
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .FirstOrDefaultAsync(m => m.IdAttivita730 == id);

            if (attivita730 == null)
            {
                return NotFound();
            }

            return View(attivita730);
        }

        // POST: Attivita730/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attivita730 = await _context.Attivita730
                .Include(a => a.AnnoFiscale)
                .FirstOrDefaultAsync(a => a.IdAttivita730 == id);

            if (attivita730 != null)
            {
                var anno = attivita730.AnnoFiscale?.Anno;
                _context.Attivita730.Remove(attivita730);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Attività 730 eliminata con successo.";
                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool Attivita730Exists(int id)
        {
            return _context.Attivita730.Any(e => e.IdAttivita730 == id);
        }

        private async Task PreparaDropdowns(int? annoSelezionato = null)
        {
            // Anni fiscali
            ViewData["IdAnno"] = new SelectList(
                await _context.AnniFiscali.OrderByDescending(a => a.Anno).ToListAsync(),
                "IdAnno", "AnnoDescrizione", annoSelezionato);

            // TUTTI i clienti (non filtriamo più per Mod730 = true)
            // La gestione dei servizi attivi viene gestita manualmente
            var clientiQuery = _context.Clienti.AsQueryable();

            if (annoSelezionato.HasValue)
            {
                var annoFiscale = await _context.AnniFiscali
                    .FirstOrDefaultAsync(a => a.IdAnno == annoSelezionato);

                if (annoFiscale != null)
                {
                    // Filtra clienti attivi nell'anno (non cessati prima dell'anno)
                    clientiQuery = clientiQuery.Where(c => 
                        c.DataCessazione == null || 
                        c.DataCessazione.Value.Year >= annoFiscale.Anno);
                }
            }

            ViewData["IdCliente"] = new SelectList(
                await clientiQuery.OrderBy(c => c.NomeCliente).ToListAsync(),
                "IdCliente", "NomeCliente");

            // Professionisti attivi
            ViewData["IdProfessionista"] = new SelectList(
                await _context.Professionisti
                    .Where(p => p.Attivo)
                    .OrderBy(p => p.Nome)
                    .ThenBy(p => p.Cognome)
                    .ToListAsync(),
                "IdProfessionista", "NomeCompleto");
        }

        private async Task PopolaDaClienti(int idAnno)
        {
            // CREA SOLO NUOVE ATTIVITÀ per clienti che:
            // 1. Hanno Mod730 = true attualmente
            // 2. NON hanno ancora un'attività per quest'anno
            // IMPORTANTE: NON rimuove mai attività esistenti, anche se Mod730 diventa false
            var clientiDaPopolare = await _context.Clienti
                .Where(c => c.Mod730 && !_context.Attivita730.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == idAnno))
                .ToListAsync();

            // Filtra per clienti attivi nell'anno
            var annoFiscale = await _context.AnniFiscali.FindAsync(idAnno);
            if (annoFiscale != null)
            {
                clientiDaPopolare = clientiDaPopolare
                    .Where(c => c.DataCessazione == null || c.DataCessazione.Value.Year >= annoFiscale.Anno)
                    .ToList();
            }

            foreach (var cliente in clientiDaPopolare)
            {
                var nuovaAttivita = new Attivita730
                {
                    IdAnno = idAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista, // Usa il professionista del cliente come default
                    RaccoltaDocumenti = "da_richiedere",
                    DrInserita = false,
                    DrControllata = false,
                    DrSpedita = false,
                    RicevutaDr730 = false,
                    PecInvioDr = false,
                    DrFirmata = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Attivita730.Add(nuovaAttivita);
            }

            if (clientiDaPopolare.Any())
            {
                Console.WriteLine($"DEBUG: Creando {clientiDaPopolare.Count} nuove attività 730");
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("DEBUG: Nessuna nuova attività 730 da creare");
            }
        }

        // POST: Attivita730/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(List<Attivita730> attivita, int? annoSelezionato, IFormCollection form)
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

            foreach (var item in attivita.Where(a => a.IdAttivita730 > 0))
            {
                try
                {
                    var esistente = await _context.Attivita730.FindAsync(item.IdAttivita730);
                    if (esistente != null)
                    {
                        // Aggiorna solo i campi modificabili
                        esistente.IdProfessionista = item.IdProfessionista;
                        esistente.CodiceDr = item.CodiceDr;
                        esistente.RaccoltaDocumenti = item.RaccoltaDocumenti;
                        esistente.DrInserita = item.DrInserita;
                        esistente.DrInseritaData = item.DrInseritaData;
                        esistente.DrControllata = item.DrControllata;
                        esistente.DrControllataData = item.DrControllataData;
                        esistente.DrSpedita = item.DrSpedita;
                        esistente.DrSpeditaData = item.DrSpeditaData;
                        esistente.RicevutaDr730 = item.RicevutaDr730;
                        esistente.PecInvioDr = item.PecInvioDr;
                        esistente.DrFirmata = item.DrFirmata;
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

        // POST: Attivita730/DeleteSingle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSingle(int id)
        {
            try
            {
                // Trova l'attività da eliminare con relazioni
                var attivita = await _context.Attivita730
                    .Include(a => a.Cliente)
                    .Include(a => a.AnnoFiscale)
                    .FirstOrDefaultAsync(a => a.IdAttivita730 == id);

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
                _context.Attivita730.Remove(attivita);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminata attività Mod. 730 per cliente: {nomeCliente} (Anno: {anno})");

                return Json(new { 
                    success = true, 
                    message = $"Attività Mod. 730 per il cliente \"{nomeCliente}\" eliminata con successo."
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

        // POST: Attivita730/DeleteAllForYear
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
                var attivitaDaEliminare = await _context.Attivita730
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.Attivita730.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione (opzionale)
                Console.WriteLine($"Eliminati {count} inserimenti Mod. 730 per l'anno {anno}");

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

        // GET: Attivita730/ExportExcel
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

                // Ottieni tutti i dati delle attività 730 per l'anno
                var attivita = await _context.Attivita730
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Mod 730 - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:M1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkGreen);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers
                var headers = new[] { "Cliente", "ATECO", "Professionista", "Codice DR", "Raccolta Doc.",
                    "DR Inserita", "DR Controllata", "DR Spedita", "Ricevuta", "PEC", "Firmata", "Note", "Completamento %" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Colora le sezioni
                using (var drRange = worksheet.Cells["F1:H1"]) // DR Inserita, Controllata, Spedita
                {
                    drRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    drRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    drRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var altriStatiRange = worksheet.Cells["I1:K1"]) // Altri Stati
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
                    worksheet.Cells[row, 4].Value = item.CodiceDr ?? "";
                    worksheet.Cells[row, 5].Value = GetRaccoltaDocumentiText(item.RaccoltaDocumenti);

                    // DR Stati con simboli colorati
                    worksheet.Cells[row, 6].Value = item.DrInserita ? "✓" : "✗";
                    worksheet.Cells[row, 7].Value = item.DrControllata ? "✓" : "✗";
                    worksheet.Cells[row, 8].Value = item.DrSpedita ? "✓" : "✗";

                    // Altri Stati con simboli colorati
                    worksheet.Cells[row, 9].Value = item.RicevutaDr730 ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.PecInvioDr ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.DrFirmata ? "✓" : "✗";

                    worksheet.Cells[row, 12].Value = item.Note ?? "";

                    // Calcola percentuale completamento
                    var totalSteps = 6; // DR Inserita, Controllata, Spedita, Ricevuta, PEC, Firmata
                    var completedSteps = 0;
                    if (item.DrInserita) completedSteps++;
                    if (item.DrControllata) completedSteps++;
                    if (item.DrSpedita) completedSteps++;
                    if (item.RicevutaDr730) completedSteps++;
                    if (item.PecInvioDr) completedSteps++;
                    if (item.DrFirmata) completedSteps++;

                    var percentage = (int)((double)completedSteps / totalSteps * 100);
                    worksheet.Cells[row, 13].Value = $"{percentage}%";

                    // Applica formattazione ai simboli
                    ApplySymbolFormatting(worksheet, row, 6, item.DrInserita); // DR Inserita
                    ApplySymbolFormatting(worksheet, row, 7, item.DrControllata); // DR Controllata
                    ApplySymbolFormatting(worksheet, row, 8, item.DrSpedita); // DR Spedita
                    ApplySymbolFormatting(worksheet, row, 9, item.RicevutaDr730); // Ricevuta
                    ApplySymbolFormatting(worksheet, row, 10, item.PecInvioDr); // PEC
                    ApplySymbolFormatting(worksheet, row, 11, item.DrFirmata); // Firmata

                    // Colora la percentuale
                    var percentageCell = worksheet.Cells[row, 13];
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
                worksheet.Tables.Add(dataRange, "Mod730Data");
                var table = worksheet.Tables["Mod730Data"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"Mod730_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
