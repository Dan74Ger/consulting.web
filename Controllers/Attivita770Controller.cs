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
    public class Attivita770Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Attivita770Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attivita770
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

            // Carica TUTTE le attività 770 esistenti per l'anno selezionato
            var attivita770 = await _context.Attivita770
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            Console.WriteLine($"DEBUG: Caricate {attivita770.Count} attività 770 per l'anno {annoFiscale.Anno}");

            // Popolamento automatico per NUOVI clienti che attivano Mod770
            await PopolaDaClienti(annoFiscale.IdAnno);

            // Ricarica dopo il popolamento per includere le nuove attività
            attivita770 = await _context.Attivita770
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .Include(a => a.AnnoFiscale)
                .Where(a => a.IdAnno == annoFiscale.IdAnno)
                .OrderBy(a => a.Cliente!.NomeCliente)
                .ToListAsync();

            Console.WriteLine($"DEBUG: Dopo popolamento: {attivita770.Count} attività 770 totali");

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

            ViewData["Title"] = $"Mod. 770 - Anno {annoSelezionato}";
            
            return View(attivita770);
        }

        // GET: Attivita770/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attivita770 = await _context.Attivita770
                .Include(a => a.AnnoFiscale)
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .FirstOrDefaultAsync(m => m.IdAttivita770 == id);

            if (attivita770 == null)
            {
                return NotFound();
            }

            return View(attivita770);
        }

        // GET: Attivita770/Create
        public async Task<IActionResult> Create(int? annoId)
        {
            // Prepara le dropdown
            await PreparaDropdowns(annoId);
            
            // Imposta l'anno se fornito
            var model = new Attivita770();
            if (annoId.HasValue)
            {
                model.IdAnno = annoId.Value;
            }

            return View(model);
        }

        // POST: Attivita770/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAnno,IdCliente,IdProfessionista,Mod770LavAutonomo,Mod770Ordinario,InserimentoDatiDr,DrInvio,Ricevuta,PecInvioDr,ModCuFatte,CuUtiliPresenti,Note")] Attivita770 attivita770)
        {
            if (ModelState.IsValid)
            {
                // Controlla che il cliente abbia Mod770 = true
                var cliente = await _context.Clienti.FindAsync(attivita770.IdCliente);
                if (cliente == null || !cliente.Mod770)
                {
                    ModelState.AddModelError("IdCliente", "Il cliente selezionato non ha il servizio Mod. 770 attivo.");
                    await PreparaDropdowns(attivita770.IdAnno);
                    return View(attivita770);
                }

                // Controlla duplicati
                var esisteGia = await _context.Attivita770
                    .AnyAsync(a => a.IdAnno == attivita770.IdAnno && a.IdCliente == attivita770.IdCliente);

                if (esisteGia)
                {
                    ModelState.AddModelError("IdCliente", "Esiste già un'attività 770 per questo cliente nell'anno selezionato.");
                    await PreparaDropdowns(attivita770.IdAnno);
                    return View(attivita770);
                }

                attivita770.CreatedAt = DateTime.UtcNow;
                attivita770.UpdatedAt = DateTime.UtcNow;

                _context.Add(attivita770);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Attività 770 creata con successo.";
                return RedirectToAction(nameof(Index), new { annoSelezionato = attivita770.AnnoFiscale?.Anno });
            }

            await PreparaDropdowns(attivita770.IdAnno);
            return View(attivita770);
        }

        // GET: Attivita770/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attivita770 = await _context.Attivita770
                .Include(a => a.AnnoFiscale)
                .FirstOrDefaultAsync(a => a.IdAttivita770 == id);

            if (attivita770 == null)
            {
                return NotFound();
            }

            await PreparaDropdowns(attivita770.IdAnno);
            return View(attivita770);
        }

        // POST: Attivita770/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAttivita770,IdAnno,IdCliente,IdProfessionista,Mod770LavAutonomo,Mod770Ordinario,InserimentoDatiDr,DrInvio,Ricevuta,PecInvioDr,ModCuFatte,CuUtiliPresenti,Note,CreatedAt")] Attivita770 attivita770)
        {
            if (id != attivita770.IdAttivita770)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    attivita770.UpdatedAt = DateTime.UtcNow;
                    _context.Update(attivita770);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Attività 770 aggiornata con successo.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Attivita770Exists(attivita770.IdAttivita770))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var anno = await _context.Attivita770
                    .Where(a => a.IdAttivita770 == id)
                    .Include(a => a.AnnoFiscale)
                    .Select(a => a.AnnoFiscale!.Anno)
                    .FirstOrDefaultAsync();

                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }

            await PreparaDropdowns(attivita770.IdAnno);
            return View(attivita770);
        }

        // GET: Attivita770/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attivita770 = await _context.Attivita770
                .Include(a => a.AnnoFiscale)
                .Include(a => a.Cliente)
                .Include(a => a.Professionista)
                .FirstOrDefaultAsync(m => m.IdAttivita770 == id);

            if (attivita770 == null)
            {
                return NotFound();
            }

            return View(attivita770);
        }

        // POST: Attivita770/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attivita770 = await _context.Attivita770
                .Include(a => a.AnnoFiscale)
                .FirstOrDefaultAsync(a => a.IdAttivita770 == id);

            if (attivita770 != null)
            {
                var anno = attivita770.AnnoFiscale?.Anno;
                _context.Attivita770.Remove(attivita770);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Attività 770 eliminata con successo.";
                return RedirectToAction(nameof(Index), new { annoSelezionato = anno });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool Attivita770Exists(int id)
        {
            return _context.Attivita770.Any(e => e.IdAttivita770 == id);
        }

        private async Task PreparaDropdowns(int? annoSelezionato = null)
        {
            // Anni fiscali
            ViewData["IdAnno"] = new SelectList(
                await _context.AnniFiscali.OrderByDescending(a => a.Anno).ToListAsync(),
                "IdAnno", "AnnoDescrizione", annoSelezionato);

            // Clienti - filtriamo per quelli con Mod770 = true
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
            // 1. Hanno Mod770 = true attualmente
            // 2. NON hanno ancora un'attività per quest'anno
            var clientiDaPopolare = await _context.Clienti
                .Where(c => c.Mod770 && !_context.Attivita770.Any(a => a.IdCliente == c.IdCliente && a.IdAnno == idAnno))
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
                var nuovaAttivita = new Attivita770
                {
                    IdAnno = idAnno,
                    IdCliente = cliente.IdCliente,
                    IdProfessionista = cliente.IdProfessionista,
                    Mod770LavAutonomo = false,
                    Mod770Ordinario = false,
                    InserimentoDatiDr = false,
                    DrInvio = false,
                    Ricevuta = false,
                    PecInvioDr = false,
                    ModCuFatte = false,
                    CuUtiliPresenti = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Attivita770.Add(nuovaAttivita);
            }

            if (clientiDaPopolare.Any())
            {
                Console.WriteLine($"DEBUG: Creando {clientiDaPopolare.Count} nuove attività 770");
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("DEBUG: Nessuna nuova attività 770 da creare");
            }
        }

        // POST: Attivita770/BulkUpdate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate([FromForm] List<Attivita770> attivita, int? annoSelezionato, IFormCollection form)
        {
            Console.WriteLine($"DEBUG BulkUpdate 770: attivita count = {attivita?.Count ?? 0}");
            Console.WriteLine($"DEBUG BulkUpdate 770: form keys = {string.Join(", ", form.Keys.Take(10))}...");
            
            // Usa IFormCollection per parsing manuale se il binding automatico fallisce
            if (attivita == null || !attivita.Any())
            {
                Console.WriteLine("DEBUG BulkUpdate 770: Binding automatico fallito, provo parsing manuale...");
                attivita = ParseAttivitaFromForm(form);
                Console.WriteLine($"DEBUG BulkUpdate 770: Dopo parsing manuale: {attivita?.Count ?? 0} attività");
            }
            
            if (attivita == null || !attivita.Any())
            {
                Console.WriteLine("DEBUG BulkUpdate 770: Nessuna attività da aggiornare anche dopo parsing manuale");
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

            foreach (var item in attivita.Where(a => a.IdAttivita770 > 0))
            {
                try
                {
                    Console.WriteLine($"DEBUG: Aggiornamento item ID={item.IdAttivita770}, Cliente={item.IdCliente}");
                    var esistente = await _context.Attivita770.FindAsync(item.IdAttivita770);
                    if (esistente != null)
                    {
                        Console.WriteLine($"DEBUG: Trovato esistente, aggiornamento campi...");
                        // Aggiorna solo i campi modificabili
                        esistente.IdProfessionista = item.IdProfessionista;
                        esistente.Mod770LavAutonomo = item.Mod770LavAutonomo;
                        esistente.Mod770Ordinario = item.Mod770Ordinario;
                        esistente.InserimentoDatiDr = item.InserimentoDatiDr;
                        esistente.DrInvio = item.DrInvio;
                        esistente.Ricevuta = item.Ricevuta;
                        esistente.PecInvioDr = item.PecInvioDr;
                        esistente.ModCuFatte = item.ModCuFatte;
                        esistente.CuUtiliPresenti = item.CuUtiliPresenti;
                        esistente.Note = item.Note;
                        esistente.UpdatedAt = DateTime.UtcNow;

                        aggiornamenti++;
                    }
                    else
                    {
                        Console.WriteLine($"DEBUG: Attività ID={item.IdAttivita770} non trovata nel database!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DEBUG: Errore aggiornamento item ID={item.IdAttivita770}: {ex.Message}");
                    errori++;
                }
            }

            try
            {
                Console.WriteLine($"DEBUG: Chiamata SaveChangesAsync con {aggiornamenti} aggiornamenti e {errori} errori");
                await _context.SaveChangesAsync();
                Console.WriteLine($"DEBUG: SaveChangesAsync completato con successo");

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



        // POST: Attivita770/DeleteAllForYear
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
                var attivitaDaEliminare = await _context.Attivita770
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .ToListAsync();

                var count = attivitaDaEliminare.Count;

                if (count == 0)
                {
                    return Json(new { success = false, message = $"Nessun inserimento trovato per l'anno {anno}." });
                }

                // Elimina tutti gli inserimenti
                _context.Attivita770.RemoveRange(attivitaDaEliminare);
                await _context.SaveChangesAsync();

                // Log dell'operazione
                Console.WriteLine($"Eliminati {count} inserimenti Mod. 770 per l'anno {anno}");

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

        // GET: Attivita770/ExportExcel
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

                // Ottieni tutti i dati delle attività 770 per l'anno
                var attivita = await _context.Attivita770
                    .Include(a => a.Cliente)
                    .Include(a => a.Professionista)
                    .Include(a => a.AnnoFiscale)
                    .Where(a => a.IdAnno == annoFiscale.IdAnno)
                    .OrderBy(a => a.Cliente!.NomeCliente)
                    .ToListAsync();

                // Crea file Excel usando EPPlus
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add($"Mod 770 - Anno {anno}");

                // Header styling
                using (var headerRange = worksheet.Cells["A1:L1"])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Headers
                var headers = new[] { "Cliente", "ATECO", "Professionista", "MOD 770 Lav.Aut.", "MOD 770 Ord.", 
                    "Inserim. Dati DR", "DR Invio", "Ricevuta", "PEC", "MOD CU Fatte", "CU Utili", "Note" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Dati
                for (int i = 0; i < attivita.Count; i++)
                {
                    var item = attivita[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = item.Cliente?.NomeCliente ?? "";
                    worksheet.Cells[row, 2].Value = item.Cliente?.CodiceAteco ?? "";
                    worksheet.Cells[row, 3].Value = item.Professionista?.NomeCompleto ?? "";
                    worksheet.Cells[row, 4].Value = item.Mod770LavAutonomo ? "✓" : "✗";
                    worksheet.Cells[row, 5].Value = item.Mod770Ordinario ? "✓" : "✗";
                    worksheet.Cells[row, 6].Value = item.InserimentoDatiDr ? "✓" : "✗";
                    worksheet.Cells[row, 7].Value = item.DrInvio ? "✓" : "✗";
                    worksheet.Cells[row, 8].Value = item.Ricevuta ? "✓" : "✗";
                    worksheet.Cells[row, 9].Value = item.PecInvioDr ? "✓" : "✗";
                    worksheet.Cells[row, 10].Value = item.ModCuFatte ? "✓" : "✗";
                    worksheet.Cells[row, 11].Value = item.CuUtiliPresenti ? "✓" : "✗";
                    worksheet.Cells[row, 12].Value = item.Note ?? "";

                    // Applica formattazione ai simboli
                    ApplySymbolFormatting(worksheet, row, 4, item.Mod770LavAutonomo);
                    ApplySymbolFormatting(worksheet, row, 5, item.Mod770Ordinario);
                    ApplySymbolFormatting(worksheet, row, 6, item.InserimentoDatiDr);
                    ApplySymbolFormatting(worksheet, row, 7, item.DrInvio);
                    ApplySymbolFormatting(worksheet, row, 8, item.Ricevuta);
                    ApplySymbolFormatting(worksheet, row, 9, item.PecInvioDr);
                    ApplySymbolFormatting(worksheet, row, 10, item.ModCuFatte);
                    ApplySymbolFormatting(worksheet, row, 11, item.CuUtiliPresenti);
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Aggiungi filtri automatici sulle intestazioni
                var dataRange = worksheet.Cells[1, 1, attivita.Count + 1, headers.Length];
                worksheet.Tables.Add(dataRange, "Mod770Data");
                var table = worksheet.Tables["Mod770Data"];
                table.ShowHeader = true;
                table.ShowFilter = true;
                table.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                // Crea il file
                var fileName = $"Mod770_Anno_{anno}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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

        // Metodo per parsing manuale dei dati dal form
        private List<Attivita770> ParseAttivitaFromForm(IFormCollection form)
        {
            var attivita = new List<Attivita770>();
            var indices = new HashSet<int>();

            // Trova tutti gli indici delle attività dal form (formato [@index])
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("[") && key.Contains("]."))
                {
                    var startIndex = key.IndexOf('[') + 1;
                    var endIndex = key.IndexOf(']');
                    if (int.TryParse(key.Substring(startIndex, endIndex - startIndex), out int index))
                    {
                        indices.Add(index);
                    }
                }
            }

            Console.WriteLine($"DEBUG ParseForm: Trovati {indices.Count} indici attività");

            // Per ogni indice, crea un oggetto Attivita770
            foreach (var index in indices.OrderBy(i => i))
            {
                try
                {
                    var attivita770 = new Attivita770();
                    
                    // Campi obbligatori
                    if (int.TryParse(form[$"[{index}].IdAttivita770"], out int idAttivita))
                        attivita770.IdAttivita770 = idAttivita;
                    
                    if (int.TryParse(form[$"[{index}].IdAnno"], out int idAnno))
                        attivita770.IdAnno = idAnno;
                    
                    if (int.TryParse(form[$"[{index}].IdCliente"], out int idCliente))
                        attivita770.IdCliente = idCliente;
                    
                    // Professionista (opzionale)
                    if (int.TryParse(form[$"[{index}].IdProfessionista"], out int idProf))
                        attivita770.IdProfessionista = idProf;
                    
                    // Checkbox (ASP.NET invia "true,false" per le checkbox)
                    attivita770.Mod770LavAutonomo = form[$"[{index}].Mod770LavAutonomo"].ToString().Contains("true");
                    attivita770.Mod770Ordinario = form[$"[{index}].Mod770Ordinario"].ToString().Contains("true");
                    attivita770.InserimentoDatiDr = form[$"[{index}].InserimentoDatiDr"].ToString().Contains("true");
                    attivita770.DrInvio = form[$"[{index}].DrInvio"].ToString().Contains("true");
                    attivita770.Ricevuta = form[$"[{index}].Ricevuta"].ToString().Contains("true");
                    attivita770.PecInvioDr = form[$"[{index}].PecInvioDr"].ToString().Contains("true");
                    attivita770.ModCuFatte = form[$"[{index}].ModCuFatte"].ToString().Contains("true");
                    attivita770.CuUtiliPresenti = form[$"[{index}].CuUtiliPresenti"].ToString().Contains("true");
                    
                    // Note
                    attivita770.Note = form[$"[{index}].Note"];
                    
                    // Date
                    if (DateTime.TryParse(form[$"[{index}].CreatedAt"], out DateTime createdAt))
                        attivita770.CreatedAt = createdAt;
                    
                    if (DateTime.TryParse(form[$"[{index}].UpdatedAt"], out DateTime updatedAt))
                        attivita770.UpdatedAt = updatedAt;
                    
                    Console.WriteLine($"DEBUG ParseForm: Parsed attività {index} - ID={attivita770.IdAttivita770}, Cliente={attivita770.IdCliente}");
                    
                    if (attivita770.IdAttivita770 > 0)
                    {
                        attivita.Add(attivita770);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DEBUG ParseForm: Errore parsing indice {index}: {ex.Message}");
                }
            }

            return attivita;
        }
    }
}
