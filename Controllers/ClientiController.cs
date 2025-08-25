using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Data;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Attributes;
using ConsultingGroup.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace ConsultingGroup.Controllers
{
    [Authorize]
    [UserPermission("anagrafiche")]
    public class ClientiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProformaService _proformaService;

        public ClientiController(ApplicationDbContext context, ProformaService proformaService)
        {
            _context = context;
            _proformaService = proformaService;
        }

        // GET: Clienti/Statistiche
        public async Task<IActionResult> Statistiche()
        {
            var statistiche = new
            {
                // Statistiche per Studio (attraverso i programmi)
                ClientiPerStudio = await _context.Clienti
                    .Include(c => c.Programma)
                    .Where(c => c.Attivo)
                    .GroupBy(c => new { 
                        ProgrammaNome = c.Programma != null ? c.Programma.NomeProgramma : "Nessun Programma"
                    })
                    .Select(g => new {
                        StudioNome = g.Key.ProgrammaNome,
                        NumeroClienti = g.Count()
                    })
                    .OrderByDescending(x => x.NumeroClienti)
                    .ToListAsync(),

                // Statistiche per Professionista
                ClientiPerProfessionista = await _context.Clienti
                    .Include(c => c.Professionista)
                    .Where(c => c.Attivo)
                    .GroupBy(c => new {
                        ProfessionistaId = c.IdProfessionista,
                        ProfessionistaNome = c.Professionista != null ? 
                            c.Professionista.Nome + " " + c.Professionista.Cognome : 
                            "Nessun Professionista"
                    })
                    .Select(g => new {
                        ProfessionistaNome = g.Key.ProfessionistaNome,
                        NumeroClienti = g.Count(),
                        // Suddivisione per tipologie
                        ClientiRedditi = g.Count(c => c.Mod730 || c.Mod740 || c.Mod750 || c.Mod760 || c.Mod770 || c.ModCu || c.ModEnc || c.ModIrap),
                        ClientiIva = g.Count(c => c.Driva || c.Lipe || c.ModTrIva),
                        ClientiContabile = g.Count(c => c.Inail || c.CassettoFiscale || c.FatturazioneElettronicaTs || c.Conservazione || c.Imu || c.RegIva || c.RegCespiti || c.Inventari || c.LibroGiornale || c.LettereIntento || c.ModIntrastat || c.FirmaDigitale || c.TitolareEffettivo)
                    })
                    .OrderByDescending(x => x.NumeroClienti)
                    .ToListAsync(),

                // Totale dichiarazioni da fare
                TotaleDichiarazioni = new
                {
                    // Dichiarazioni Redditi
                    Mod730 = await _context.Clienti.CountAsync(c => c.Attivo && c.Mod730),
                    Mod740 = await _context.Clienti.CountAsync(c => c.Attivo && c.Mod740),
                    Mod750 = await _context.Clienti.CountAsync(c => c.Attivo && c.Mod750),
                    Mod760 = await _context.Clienti.CountAsync(c => c.Attivo && c.Mod760),
                    Mod770 = await _context.Clienti.CountAsync(c => c.Attivo && c.Mod770),
                    ModCu = await _context.Clienti.CountAsync(c => c.Attivo && c.ModCu),
                    ModEnc = await _context.Clienti.CountAsync(c => c.Attivo && c.ModEnc),
                    ModIrap = await _context.Clienti.CountAsync(c => c.Attivo && c.ModIrap),
                    
                    // Dichiarazioni IVA
                    Driva = await _context.Clienti.CountAsync(c => c.Attivo && c.Driva),
                    Lipe = await _context.Clienti.CountAsync(c => c.Attivo && c.Lipe),
                    ModTrIva = await _context.Clienti.CountAsync(c => c.Attivo && c.ModTrIva),
                    
                    // Attività Contabile
                    Inail = await _context.Clienti.CountAsync(c => c.Attivo && c.Inail),
                    CassettoFiscale = await _context.Clienti.CountAsync(c => c.Attivo && c.CassettoFiscale),
                    FatturazioneElettronicaTs = await _context.Clienti.CountAsync(c => c.Attivo && c.FatturazioneElettronicaTs),
                    Conservazione = await _context.Clienti.CountAsync(c => c.Attivo && c.Conservazione),
                    Imu = await _context.Clienti.CountAsync(c => c.Attivo && c.Imu),
                    RegIva = await _context.Clienti.CountAsync(c => c.Attivo && c.RegIva),
                    RegCespiti = await _context.Clienti.CountAsync(c => c.Attivo && c.RegCespiti),
                    Inventari = await _context.Clienti.CountAsync(c => c.Attivo && c.Inventari),
                    LibroGiornale = await _context.Clienti.CountAsync(c => c.Attivo && c.LibroGiornale),
                    LettereIntento = await _context.Clienti.CountAsync(c => c.Attivo && c.LettereIntento),
                    ModIntrastat = await _context.Clienti.CountAsync(c => c.Attivo && c.ModIntrastat),
                    FirmaDigitale = await _context.Clienti.CountAsync(c => c.Attivo && c.FirmaDigitale),
                    TitolareEffettivo = await _context.Clienti.CountAsync(c => c.Attivo && c.TitolareEffettivo)
                },

                // Statistiche generali
                StatisticheGenerali = new
                {
                    TotaleClientiAttivi = await _context.Clienti.CountAsync(c => c.Attivo),
                    TotaleClientiCessati = await _context.Clienti.CountAsync(c => !c.Attivo),
                    TotaleProgrammi = await _context.Programmi.CountAsync(p => p.Attivo),
                    TotaleProfessionisti = await _context.Professionisti.CountAsync(p => p.Attivo)
                }
            };

            return View(statistiche);
        }

        // GET: Clienti
        public async Task<IActionResult> Index(string searchString = "", string statusFilter = "", string[]? dichiarazioni = null, int page = 1, int pageSize = 10)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.DichiarazioniFilter = dichiarazioni ?? new string[0];

            var clientiQuery = _context.Clienti
                .Include(c => c.Programma)
                .Include(c => c.Professionista)
                .Include(c => c.RegimeContabile)
                .Include(c => c.TipologiaInps)
                .Include(c => c.AnnoFiscaleRiattivazione)
                .AsQueryable();

            // Filtro per ricerca testuale
            if (!string.IsNullOrEmpty(searchString))
            {
                clientiQuery = clientiQuery.Where(c => 
                    c.NomeCliente.Contains(searchString) ||
                    (c.MailCliente != null && c.MailCliente.Contains(searchString)) ||
                    (c.CodiceAteco != null && c.CodiceAteco.Contains(searchString)));
            }

            // Filtro per stato attivo/cessato
            if (!string.IsNullOrEmpty(statusFilter))
            {
                switch (statusFilter.ToLower())
                {
                    case "attivi":
                        clientiQuery = clientiQuery.Where(c => c.Attivo == true);
                        break;
                    case "cessati":
                        clientiQuery = clientiQuery.Where(c => c.Attivo == false);
                        break;
                    case "riattivati":
                        clientiQuery = clientiQuery.Where(c => c.RiattivatoPerAnno.HasValue);
                        break;
                    default:
                        // Mostra tutti se il filtro non è riconosciuto
                        break;
                }
            }

            // Filtro per dichiarazioni/attività (modalità OR: il cliente deve avere ALMENO UNA delle dichiarazioni selezionate)
            if (dichiarazioni != null && dichiarazioni.Length > 0)
            {
                clientiQuery = clientiQuery.Where(c => 
                    dichiarazioni.Any(d => 
                        (d.ToLower() == "mod730" && c.Mod730) ||
                        (d.ToLower() == "mod740" && c.Mod740) ||
                        (d.ToLower() == "mod750" && c.Mod750) ||
                        (d.ToLower() == "mod760" && c.Mod760) ||
                        (d.ToLower() == "mod770" && c.Mod770) ||
                        (d.ToLower() == "modcu" && c.ModCu) ||
                        (d.ToLower() == "modenc" && c.ModEnc) ||
                        (d.ToLower() == "modirap" && c.ModIrap) ||
                        (d.ToLower() == "driva" && c.Driva) ||
                        (d.ToLower() == "lipe" && c.Lipe) ||
                        (d.ToLower() == "modtriva" && c.ModTrIva) ||
                        (d.ToLower() == "inail" && c.Inail) ||
                        (d.ToLower() == "cassettofiscale" && c.CassettoFiscale) ||
                        (d.ToLower() == "fatturazioneelettronica" && c.FatturazioneElettronicaTs) ||
                        (d.ToLower() == "conservazione" && c.Conservazione) ||
                        (d.ToLower() == "imu" && c.Imu) ||
                        (d.ToLower() == "regiva" && c.RegIva) ||
                        (d.ToLower() == "regcespiti" && c.RegCespiti) ||
                        (d.ToLower() == "inventari" && c.Inventari) ||
                        (d.ToLower() == "librogiornale" && c.LibroGiornale) ||
                        (d.ToLower() == "lettereintento" && c.LettereIntento) ||
                        (d.ToLower() == "modintrastat" && c.ModIntrastat) ||
                        (d.ToLower() == "firmadigitale" && c.FirmaDigitale) ||
                        (d.ToLower() == "titolareeffettivo" && c.TitolareEffettivo)
                    )
                );
            }

            // Paginazione
            var totalItems = await clientiQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var clienti = await clientiQuery
                .OrderByDescending(c => c.DataAttivazione)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(clienti);
        }

        // GET: Clienti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clienti
                .Include(c => c.Programma)
                .Include(c => c.Professionista)
                .Include(c => c.RegimeContabile)
                .Include(c => c.TipologiaInps)
                .Include(c => c.AnnoFiscaleRiattivazione)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clienti/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ClientiViewModel();
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Clienti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientiViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Controllo per nomi duplicati
                var clienteEsistente = await _context.Clienti
                    .FirstOrDefaultAsync(c => c.NomeCliente.ToLower() == viewModel.NomeCliente.ToLower());
                
                if (clienteEsistente != null)
                {
                    ModelState.AddModelError("NomeCliente", $"Esiste già un cliente con il nome '{viewModel.NomeCliente}'. Scegli un nome diverso.");
                    await PopulateDropdowns(viewModel);
                    return View(viewModel);
                }

                var cliente = new Cliente
                {
                    NomeCliente = viewModel.NomeCliente,
                    IdProgramma = viewModel.IdProgramma,
                    IdProfessionista = viewModel.IdProfessionista,
                    MailCliente = viewModel.MailCliente,
                    IdRegimeContabile = viewModel.IdRegimeContabile,
                    IdTipologiaInps = viewModel.IdTipologiaInps,
                    ContabilitaInternaTrimestrale = viewModel.ContabilitaInternaTrimestrale,
                    ContabilitaInternaMensile = viewModel.ContabilitaInternaMensile,
                    TassoIvaTrimestrale = viewModel.TassoIvaTrimestrale,
                    
                    // Attività Redditi
                    Mod730 = viewModel.Mod730,
                    Mod740 = viewModel.Mod740,
                    Mod750 = viewModel.Mod750,
                    Mod760 = viewModel.Mod760,
                    Mod770 = viewModel.Mod770,
                    ModCu = viewModel.ModCu,
                    ModEnc = viewModel.ModEnc,
                    ModIrap = viewModel.ModIrap,
                    
                    // Attività IVA
                    Driva = viewModel.Driva,
                    Lipe = viewModel.Lipe,
                    ModTrIva = viewModel.ModTrIva,
                    
                    // Attività Contabile
                    Inail = viewModel.Inail,
                    CassettoFiscale = viewModel.CassettoFiscale,
                    FatturazioneElettronicaTs = viewModel.FatturazioneElettronicaTs,
                    Conservazione = viewModel.Conservazione,
                    Imu = viewModel.Imu,
                    RegIva = viewModel.RegIva,
                    RegCespiti = viewModel.RegCespiti,
                    Inventari = viewModel.Inventari,
                    LibroGiornale = viewModel.LibroGiornale,
                    LettereIntento = viewModel.LettereIntento,
                    ModIntrastat = viewModel.ModIntrastat,
                    FirmaDigitale = viewModel.FirmaDigitale,
                    TitolareEffettivo = viewModel.TitolareEffettivo,
                    
                    CodiceAteco = viewModel.CodiceAteco,
                    
                    // Nuovi campi dati cliente
                    CfCliente = viewModel.CfCliente,
                    PivaCliente = viewModel.PivaCliente,
                    Indirizzo = viewModel.Indirizzo,
                    Citta = viewModel.Citta,
                    Provincia = viewModel.Provincia,
                    Cap = viewModel.Cap,
                    LegaleRappresentante = viewModel.LegaleRappresentante,
                    CfLegaleRappresentante = viewModel.CfLegaleRappresentante,
                    
                    // Sezione mandati
                    DataMandato = viewModel.DataMandato,
                    ImportoMandatoAnnuo = viewModel.ImportoMandatoAnnuo,
                    ProformaTipo = viewModel.ProformaTipo,
                    
                    Attivo = true,
                    DataAttivazione = DateTime.UtcNow,
                    DataModifica = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    RiattivatoPerAnno = viewModel.RiattivatoPerAnno,
                    DataRiattivazione = viewModel.RiattivatoPerAnno.HasValue ? DateTime.UtcNow : null
                };

                _context.Clienti.Add(cliente);
                await _context.SaveChangesAsync();
                
                // Genera automaticamente le proforma se ci sono dati mandato
                await GeneraProformeAutomatiche(cliente.IdCliente, cliente.DataMandato, cliente.ImportoMandatoAnnuo, cliente.ProformaTipo);
                
                TempData["SuccessMessage"] = $"Cliente '{cliente.NomeCliente}' creato con successo.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: Clienti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clienti.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var viewModel = MapToViewModel(cliente);
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Clienti/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientiViewModel viewModel)
        {
            if (id != viewModel.IdCliente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cliente = await _context.Clienti.FindAsync(id);
                    if (cliente == null)
                    {
                        return NotFound();
                    }

                    // Controllo per nomi duplicati (solo se il nome è cambiato)
                    if (cliente.NomeCliente.ToLower() != viewModel.NomeCliente.ToLower())
                    {
                        var clienteEsistente = await _context.Clienti
                            .FirstOrDefaultAsync(c => c.NomeCliente.ToLower() == viewModel.NomeCliente.ToLower() && c.IdCliente != id);
                        
                        if (clienteEsistente != null)
                        {
                            ModelState.AddModelError("NomeCliente", $"Esiste già un cliente con il nome '{viewModel.NomeCliente}'. Scegli un nome diverso.");
                            await PopulateDropdowns(viewModel);
                            return View(viewModel);
                        }
                    }

                    // CONTROLLO RIMOZIONI CHECKBOX (per eliminazione dalle liste)
                    var checkboxRimosse = new List<(string Modulo, string Tabella, int Clienti)>();
                    var annoCorrente = await _context.AnniFiscali.OrderByDescending(a => a.Anno).FirstOrDefaultAsync();
                    
                    if (annoCorrente != null)
                    {
                        // Verifica per ogni checkbox che è stata deselezionata
                        if (cliente.Mod730 && !viewModel.Mod730)
                        {
                            var count = await _context.Attivita730.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD 730", "attivita_730", count));
                        }
                        if (cliente.Mod740 && !viewModel.Mod740)
                        {
                            var count = await _context.Attivita740.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD 740", "attivita_740", count));
                        }
                        if (cliente.Mod750 && !viewModel.Mod750)
                        {
                            var count = await _context.Attivita750.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD 750", "attivita_750", count));
                        }
                        if (cliente.Mod760 && !viewModel.Mod760)
                        {
                            var count = await _context.Attivita760.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD 760", "attivita_760", count));
                        }
                        if (cliente.Mod770 && !viewModel.Mod770)
                        {
                            var count = await _context.Attivita770.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD 770", "attivita_770", count));
                        }
                        if (cliente.ModCu && !viewModel.ModCu)
                        {
                            var count = await _context.AttivitaCu.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD CU", "attivita_cu", count));
                        }
                        if (cliente.ModEnc && !viewModel.ModEnc)
                        {
                            var count = await _context.AttivitaEnc.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD ENC", "attivita_enc", count));
                        }
                        if (cliente.ModIrap && !viewModel.ModIrap)
                        {
                            var count = await _context.AttivitaIrap.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD IRAP", "attivita_irap", count));
                        }
                        // Controlli per attività IVA
                        if (cliente.Driva && !viewModel.Driva)
                        {
                            var count = await _context.AttivitaDriva.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("DRIVA", "attivita_driva", count));
                        }
                        if (cliente.Lipe && !viewModel.Lipe)
                        {
                            var count = await _context.AttivitaLipe.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("LIPE", "attivita_lipe", count));
                        }
                        if (cliente.ModTrIva && !viewModel.ModTrIva)
                        {
                            var count = await _context.AttivitaTriva.CountAsync(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("MOD TR IVA", "attivita_mod_tr_iva", count));
                        }
                        
                        // CONTROLLI PER CONTABILITÀ INTERNA
                        if (cliente.ContabilitaInternaTrimestrale && !viewModel.ContabilitaInternaTrimestrale)
                        {
                            var count = await _context.ContabilitaInternaTrimestrale.CountAsync(ct => ct.IdCliente == cliente.IdCliente && ct.IdAnno == annoCorrente.IdAnno);
                            if (count > 0) checkboxRimosse.Add(("CONTABILITÀ TRIMESTRALE", "contabilita_interna_trimestrale", count));
                        }
                        // TODO: Quando sarà implementata la contabilità mensile, aggiungere:
                        // if (cliente.ContabilitaInternaMensile && !viewModel.ContabilitaInternaMensile)
                        // {
                        //     var count = await _context.ContabilitaInternaMensile.CountAsync(cm => cm.IdCliente == cliente.IdCliente && cm.IdAnno == annoCorrente.IdAnno);
                        //     if (count > 0) checkboxRimosse.Add(("CONTABILITÀ MENSILE", "contabilita_interna_mensile", count));
                        // }
                        
                        // Controlli per attività contabili
                        // TODO: Quando saranno create le tabelle per le attività contabili, aggiungere i controlli per:
                        // - INAIL (AttivitaInail)
                        // - Cassetto Fiscale (AttivitaCassettoFiscale)
                        // - Fatturazione Elettronica TS (AttivitaFatturazioneElettronicaTs)
                        // - Conservazione (AttivitaConservazione)
                        // - IMU (AttivitaImu)
                        // - Registro IVA (AttivitaRegIva)
                        // - Registro Cespiti (AttivitaRegCespiti)
                        // - Inventari (AttivitaInventari)
                        // - Libro Giornale (AttivitaLibroGiornale)
                        // - Lettere d'Intento (AttivitaLettereIntento)
                        // - Mod. INTRASTAT (AttivitaModIntrastat)
                        // - Firma Digitale (AttivitaFirmaDigitale)
                        // - Titolare Effettivo (AttivitaTitolareEffettivo)
                    }

                    // Se ci sono checkbox rimosse, richiedi conferma
                    if (checkboxRimosse.Count > 0)
                    {
                        // Verifica se l'utente ha confermato la rimozione
                        var confermaRimozione = Request.Form["ConfermaRimozione"].ToString();
                        if (confermaRimozione != "true")
                        {
                            // Prepara messaggio di conferma
                            var messaggiRimozione = checkboxRimosse.Select(cr => 
                                $"• {cr.Modulo}: {cr.Clienti} record nell'anno {annoCorrente?.Anno}").ToList();
                            
                            TempData["WarningMessage"] = $"ATTENZIONE! Hai deselezionato delle checkbox. Questo rimuoverà '{cliente.NomeCliente}' dalle seguenti liste attività:\n\n" +
                                string.Join("\n", messaggiRimozione) + 
                                "\n\nVuoi procedere con la rimozione? I dati verranno ELIMINATI definitivamente.";
                            
                            // Memorizza temporaneamente le checkbox rimosse
                            TempData["CheckboxRimosse"] = System.Text.Json.JsonSerializer.Serialize(checkboxRimosse);
                            TempData["RichiestaConfermaRimozione"] = "true";
                            
                            await PopulateDropdowns(viewModel);
                            return View(viewModel);
                        }
                        else
                        {
                            // Utente ha confermato, procedi con l'eliminazione
                            await RimuoviDalleListeAttivita(cliente.IdCliente, annoCorrente!.IdAnno, checkboxRimosse);
                            
                            var moduliRimossi = string.Join(", ", checkboxRimosse.Select(cr => cr.Modulo));
                            TempData["SuccessMessage"] = $"Cliente '{cliente.NomeCliente}' rimosso dalle liste: {moduliRimossi}";
                        }
                    }

                    // CONTROLLO CAMBIAMENTI ATTIVITÀ REDDITI
                    var cambiamentiAttivita = new List<string>();
                    
                    // Verifica cambiamenti per ogni attività redditi
                    if (cliente.Mod730 != viewModel.Mod730)
                        cambiamentiAttivita.Add(viewModel.Mod730 ? "Mod730 (AGGIUNTO)" : "Mod730 (RIMOSSO)");
                    if (cliente.Mod740 != viewModel.Mod740)
                        cambiamentiAttivita.Add(viewModel.Mod740 ? "Mod740 (AGGIUNTO)" : "Mod740 (RIMOSSO)");
                    if (cliente.Mod750 != viewModel.Mod750)
                        cambiamentiAttivita.Add(viewModel.Mod750 ? "Mod750 (AGGIUNTO)" : "Mod750 (RIMOSSO)");
                    if (cliente.Mod760 != viewModel.Mod760)
                        cambiamentiAttivita.Add(viewModel.Mod760 ? "Mod760 (AGGIUNTO)" : "Mod760 (RIMOSSO)");
                    if (cliente.Mod770 != viewModel.Mod770)
                        cambiamentiAttivita.Add(viewModel.Mod770 ? "Mod770 (AGGIUNTO)" : "Mod770 (RIMOSSO)");
                    if (cliente.ModCu != viewModel.ModCu)
                        cambiamentiAttivita.Add(viewModel.ModCu ? "ModCU (AGGIUNTO)" : "ModCU (RIMOSSO)");
                    if (cliente.ModEnc != viewModel.ModEnc)
                        cambiamentiAttivita.Add(viewModel.ModEnc ? "ModENC (AGGIUNTO)" : "ModENC (RIMOSSO)");
                    if (cliente.ModIrap != viewModel.ModIrap)
                        cambiamentiAttivita.Add(viewModel.ModIrap ? "ModIRAP (AGGIUNTO)" : "ModIRAP (RIMOSSO)");

                    // Se ci sono cambiamenti nelle attività redditi
                    if (cambiamentiAttivita.Count > 0)
                    {
                        // Gestione migrazione dati attività
                        await HandleActivityMigration(cliente, viewModel);
                        
                        TempData["ActivityChangesMessage"] = $"ATTENZIONE: Modifiche alle attività redditi per '{cliente.NomeCliente}': {string.Join(", ", cambiamentiAttivita)}. I dati sono stati spostati automaticamente tra le sezioni di Gestione Attività.";
                    }

                    // Aggiorna tutti i campi
                    cliente.NomeCliente = viewModel.NomeCliente;
                    cliente.IdProgramma = viewModel.IdProgramma;
                    cliente.IdProfessionista = viewModel.IdProfessionista;
                    cliente.MailCliente = viewModel.MailCliente;
                    cliente.IdRegimeContabile = viewModel.IdRegimeContabile;
                    cliente.IdTipologiaInps = viewModel.IdTipologiaInps;
                    cliente.ContabilitaInternaTrimestrale = viewModel.ContabilitaInternaTrimestrale;
                    cliente.ContabilitaInternaMensile = viewModel.ContabilitaInternaMensile;
                    cliente.TassoIvaTrimestrale = viewModel.TassoIvaTrimestrale;
                    
                    // Attività Redditi
                    cliente.Mod730 = viewModel.Mod730;
                    cliente.Mod740 = viewModel.Mod740;
                    cliente.Mod750 = viewModel.Mod750;
                    cliente.Mod760 = viewModel.Mod760;
                    cliente.Mod770 = viewModel.Mod770;
                    cliente.ModCu = viewModel.ModCu;
                    cliente.ModEnc = viewModel.ModEnc;
                    cliente.ModIrap = viewModel.ModIrap;
                    
                    // Attività IVA
                    cliente.Driva = viewModel.Driva;
                    cliente.Lipe = viewModel.Lipe;
                    cliente.ModTrIva = viewModel.ModTrIva;
                    
                    // Attività Contabile
                    cliente.Inail = viewModel.Inail;
                    cliente.CassettoFiscale = viewModel.CassettoFiscale;
                    cliente.FatturazioneElettronicaTs = viewModel.FatturazioneElettronicaTs;
                    cliente.Conservazione = viewModel.Conservazione;
                    cliente.Imu = viewModel.Imu;
                    cliente.RegIva = viewModel.RegIva;
                    cliente.RegCespiti = viewModel.RegCespiti;
                    cliente.Inventari = viewModel.Inventari;
                    cliente.LibroGiornale = viewModel.LibroGiornale;
                    cliente.LettereIntento = viewModel.LettereIntento;
                    cliente.ModIntrastat = viewModel.ModIntrastat;
                    cliente.FirmaDigitale = viewModel.FirmaDigitale;
                    cliente.TitolareEffettivo = viewModel.TitolareEffettivo;
                    
                    cliente.CodiceAteco = viewModel.CodiceAteco;
                    
                    // Nuovi campi dati cliente
                    cliente.CfCliente = viewModel.CfCliente;
                    cliente.PivaCliente = viewModel.PivaCliente;
                    cliente.Indirizzo = viewModel.Indirizzo;
                    cliente.Citta = viewModel.Citta;
                    cliente.Provincia = viewModel.Provincia;
                    cliente.Cap = viewModel.Cap;
                    cliente.LegaleRappresentante = viewModel.LegaleRappresentante;
                    cliente.CfLegaleRappresentante = viewModel.CfLegaleRappresentante;
                    
                    // Sezione mandati
                    cliente.DataMandato = viewModel.DataMandato;
                    cliente.ImportoMandatoAnnuo = viewModel.ImportoMandatoAnnuo;
                    cliente.ProformaTipo = viewModel.ProformaTipo;
                    
                    cliente.DataModifica = DateTime.UtcNow;
                    cliente.UpdatedAt = DateTime.UtcNow;
                    cliente.RiattivatoPerAnno = viewModel.RiattivatoPerAnno;

                    // Se è stato impostato un anno di riattivazione e non era già impostato
                    if (viewModel.RiattivatoPerAnno.HasValue && !cliente.DataRiattivazione.HasValue)
                    {
                        cliente.DataRiattivazione = DateTime.UtcNow;
                    }
                    // Se è stato rimosso l'anno di riattivazione
                    else if (!viewModel.RiattivatoPerAnno.HasValue)
                    {
                        cliente.DataRiattivazione = null;
                    }

                    await _context.SaveChangesAsync();
                    
                    // Rigenera automaticamente le proforma se ci sono dati mandato
                    await GeneraProformeAutomatiche(cliente.IdCliente, cliente.DataMandato, cliente.ImportoMandatoAnnuo, cliente.ProformaTipo);
                    
                    TempData["SuccessMessage"] = $"Cliente '{cliente.NomeCliente}' modificato con successo.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(viewModel.IdCliente))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Clienti/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clienti.FindAsync(id);
            if (cliente != null)
            {
                var clienteNome = cliente.NomeCliente;
                _context.Clienti.Remove(cliente);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Cliente '{clienteNome}' eliminato con successo.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clienti/MigrateExistingActivities
        [HttpGet]
        public async Task<IActionResult> MigrateExistingActivities()
        {
            var annoCorrente = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.AnnoCorrente);
            if (annoCorrente == null)
            {
                TempData["ErrorMessage"] = "Nessun anno fiscale corrente trovato.";
                return RedirectToAction(nameof(Index));
            }

            // Trova clienti che sono passati da 730 a 740 ma hanno ancora dati 730
            var clientiDaMigrare = await _context.Attivita730
                .Include(a => a.Cliente)
                .Where(a => a.IdAnno == annoCorrente.IdAnno && a.Cliente != null && a.Cliente.Mod740 && !a.Cliente.Mod730)
                .Select(a => new { 
                    AttivitaId = a.IdAttivita730,
                    ClienteId = a.IdCliente, 
                    ClienteNome = a.Cliente!.NomeCliente,
                    CodiceDr = a.CodiceDr,
                    RaccoltaDocumenti = a.RaccoltaDocumenti
                })
                .ToListAsync();

            ViewBag.ClientiDaMigrare = clientiDaMigrare;
            ViewBag.AnnoCorrente = annoCorrente.Anno;
            return View();
        }

        // POST: Clienti/ExecuteMigration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExecuteMigration()
        {
            var annoCorrente = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.AnnoCorrente);
            if (annoCorrente == null)
            {
                TempData["ErrorMessage"] = "Nessun anno fiscale corrente trovato.";
                return RedirectToAction(nameof(Index));
            }

            var migrateCount = 0;
            
            // Trova e migra tutte le attività 730 di clienti che ora hanno 740
            var attivitaDaMigrare = await _context.Attivita730
                .Include(a => a.Cliente)
                .Where(a => a.IdAnno == annoCorrente.IdAnno && a.Cliente != null && a.Cliente.Mod740 && !a.Cliente.Mod730)
                .ToListAsync();

            foreach (var old730 in attivitaDaMigrare)
            {
                // Verifica se esiste già un'attività 740 per questo cliente
                var esisteGia740 = await _context.Attivita740
                    .AnyAsync(a => a.IdCliente == old730.IdCliente && a.IdAnno == old730.IdAnno);
                
                if (!esisteGia740)
                {
                    // Crea nuova attività 740
                    var nuova740 = new Attivita740
                    {
                        IdAnno = old730.IdAnno,
                        IdCliente = old730.IdCliente,
                        IdProfessionista = old730.IdProfessionista,
                        CodiceDr = old730.CodiceDr,
                        RaccoltaDocumenti = old730.RaccoltaDocumenti,
                        
                        // Migra stati DR esistenti
                        DrInserita = old730.DrInserita,
                        DrInseritaData = old730.DrInseritaData,
                        DrControllata = old730.DrControllata,
                        DrControllataData = old730.DrControllataData,
                        DrSpedita = old730.DrSpedita,
                        DrSpeditaData = old730.DrSpeditaData,
                        
                        // Migra altri stati
                        RicevutaDr = old730.RicevutaDr730,
                        DrFirmata = old730.DrFirmata,
                        PecInvioDr = old730.PecInvioDr,
                        
                        // Note
                        Note = old730.Note,
                        
                        // Campi specifici 740 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita740.Add(nuova740);
                    migrateCount++;
                }
                
                // Rimuovi vecchia attività 730
                _context.Attivita730.Remove(old730);
            }
            
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"Migrazione completata! {migrateCount} attività migrate da Mod.730 a Mod.740 per l'anno {annoCorrente.Anno}.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Clienti/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var cliente = await _context.Clienti.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            if (cliente.Attivo)
            {
                // Cessazione
                cliente.Attivo = false;
                cliente.DataCessazione = DateTime.UtcNow;
                cliente.DataModifica = DateTime.UtcNow;
                cliente.UpdatedAt = DateTime.UtcNow;
                
                TempData["SuccessMessage"] = $"Cliente '{cliente.NomeCliente}' cessato con successo.";
            }
            else
            {
                // Riattivazione
                cliente.Attivo = true;
                cliente.DataCessazione = null;
                cliente.DataModifica = DateTime.UtcNow;
                cliente.UpdatedAt = DateTime.UtcNow;
                
                TempData["SuccessMessage"] = $"Cliente '{cliente.NomeCliente}' riattivato con successo.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clienti.Any(e => e.IdCliente == id);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CambiaPeriodicita(int idCliente, string nuovaPeriodicita)
        {
            try
            {
                Console.WriteLine($"[CambiaPeriodicita] Inizio: idCliente={idCliente}, nuovaPeriodicita={nuovaPeriodicita}");
                
                var cliente = await _context.Clienti.FindAsync(idCliente);
                if (cliente == null)
                {
                    Console.WriteLine($"[CambiaPeriodicita] Cliente non trovato: {idCliente}");
                    return Json(new { success = false, message = "Cliente non trovato" });
                }

                Console.WriteLine($"[CambiaPeriodicita] Cliente trovato: {cliente.NomeCliente}, ImportoMandato={cliente.ImportoMandatoAnnuo}, DataMandato={cliente.DataMandato}");

                // Elimina tutte le proforma esistenti per questo cliente
                var proformeEsistenti = await _context.ProformeGenerate
                    .Where(p => p.IdCliente == idCliente)
                    .ToListAsync();
                
                Console.WriteLine($"[CambiaPeriodicita] Proforma esistenti da eliminare: {proformeEsistenti.Count}");
                
                if (proformeEsistenti.Any())
                {
                    _context.ProformeGenerate.RemoveRange(proformeEsistenti);
                }

                // Aggiorna il tipo di proforma del cliente
                cliente.ProformaTipo = nuovaPeriodicita;
                cliente.UpdatedAt = DateTime.Now;

                // Crea nuove proforma del nuovo tipo
                if (!string.IsNullOrEmpty(nuovaPeriodicita) && cliente.ImportoMandatoAnnuo.HasValue && cliente.ImportoMandatoAnnuo > 0)
                {
                    Console.WriteLine($"[CambiaPeriodicita] Creazione nuove proforma...");
                    await _proformaService.GeneraProformeAsync(
                        idCliente, 
                        cliente.DataMandato, 
                        cliente.ImportoMandatoAnnuo.Value, 
                        nuovaPeriodicita
                    );
                    Console.WriteLine($"[CambiaPeriodicita] Nuove proforma create");
                }
                else
                {
                    Console.WriteLine($"[CambiaPeriodicita] Condizioni non soddisfatte per creare proforma");
                    Console.WriteLine($"[CambiaPeriodicita] - nuovaPeriodicita vuota: {string.IsNullOrEmpty(nuovaPeriodicita)}");
                    Console.WriteLine($"[CambiaPeriodicita] - ImportoMandatoAnnuo null: {!cliente.ImportoMandatoAnnuo.HasValue}");
                    Console.WriteLine($"[CambiaPeriodicita] - ImportoMandatoAnnuo <= 0: {cliente.ImportoMandatoAnnuo <= 0}");
                }

                await _context.SaveChangesAsync();

                // Ricarica le nuove proforma per restituirle
                var nuoveProformeCreate = await _context.ProformeGenerate
                    .Where(p => p.IdCliente == idCliente)
                    .OrderBy(p => p.NumeroRata)
                    .Select(p => new {
                        p.IdProforma,
                        p.NumeroRata,
                        p.DataScadenza,
                        p.ImportoRata,
                        p.TipoProforma
                    })
                    .ToListAsync();

                Console.WriteLine($"[CambiaPeriodicita] Proforma finali trovate: {nuoveProformeCreate.Count}");
                foreach (var proforma in nuoveProformeCreate)
                {
                    Console.WriteLine($"[CambiaPeriodicita] - ID: {proforma.IdProforma}, Rata: {proforma.NumeroRata}, Data: {proforma.DataScadenza:yyyy-MM-dd}, Importo: {proforma.ImportoRata}");
                }

                return Json(new { 
                    success = true, 
                    message = $"Periodicità cambiata in {nuovaPeriodicita}",
                    proforma = nuoveProformeCreate
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore: {ex.Message}" });
            }
        }

        private async Task PopulateDropdowns(ClientiViewModel viewModel)
        {
            // Programmi
            var programmi = await _context.Programmi
                .Where(p => p.Attivo)
                .OrderBy(p => p.NomeProgramma)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProgramma.ToString(),
                    Text = p.NomeProgramma,
                    Selected = false
                })
                .ToListAsync();
            viewModel.ProgrammiDisponibili = programmi;

            // Professionisti
            var professionisti = await _context.Professionisti
                .Where(p => p.Attivo)
                .OrderBy(p => p.Cognome).ThenBy(p => p.Nome)
                .Select(p => new SelectListItem
                {
                    Value = p.IdProfessionista.ToString(),
                    Text = $"{p.Cognome} {p.Nome}",
                    Selected = false
                })
                .ToListAsync();
            viewModel.ProfessionistiDisponibili = professionisti;

            // Regimi Contabili
            var regimi = await _context.RegimiContabili
                .Where(r => r.Attivo)
                .OrderBy(r => r.NomeRegime)
                .Select(r => new SelectListItem
                {
                    Value = r.IdRegimeContabile.ToString(),
                    Text = r.NomeRegime,
                    Selected = false
                })
                .ToListAsync();
            viewModel.RegimiContabiliDisponibili = regimi;

            // Tipologie INPS
            var tipologie = await _context.TipologieInps
                .Where(t => t.Attivo)
                .OrderBy(t => t.Tipologia)
                .Select(t => new SelectListItem
                {
                    Value = t.IdTipologiaInps.ToString(),
                    Text = t.Tipologia,
                    Selected = false
                })
                .ToListAsync();
            viewModel.TipologieInpsDisponibili = tipologie;

            // Anni Fiscali
            var anniFiscali = await _context.AnniFiscali
                .Where(a => a.Attivo)
                .OrderByDescending(a => a.Anno)
                .Select(a => new SelectListItem
                {
                    Value = a.IdAnno.ToString(),
                    Text = $"{a.Anno} - {a.Descrizione}",
                    Selected = false
                })
                .ToListAsync();
            viewModel.AnniFiscaliDisponibili = anniFiscali;

            // Periodicità Proforma
            var periodicita = new List<SelectListItem>
            {
                new SelectListItem { Value = "trimestrale", Text = "Trimestrale", Selected = viewModel.ProformaTipo == "trimestrale" || (string.IsNullOrEmpty(viewModel.ProformaTipo) && viewModel.IdCliente == 0) },
                new SelectListItem { Value = "bimestrale", Text = "Bimestrale", Selected = viewModel.ProformaTipo == "bimestrale" },
                new SelectListItem { Value = "semestrale", Text = "Semestrale", Selected = viewModel.ProformaTipo == "semestrale" },
                new SelectListItem { Value = "mensile", Text = "Mensile", Selected = viewModel.ProformaTipo == "mensile" }
            };
            viewModel.PeriodicittaDisponibili = periodicita;
        }

        private ClientiViewModel MapToViewModel(Cliente cliente)
        {
            return new ClientiViewModel
            {
                IdCliente = cliente.IdCliente,
                NomeCliente = cliente.NomeCliente,
                IdProgramma = cliente.IdProgramma,
                IdProfessionista = cliente.IdProfessionista,
                MailCliente = cliente.MailCliente,
                IdRegimeContabile = cliente.IdRegimeContabile,
                IdTipologiaInps = cliente.IdTipologiaInps,
                ContabilitaInternaTrimestrale = cliente.ContabilitaInternaTrimestrale,
                ContabilitaInternaMensile = cliente.ContabilitaInternaMensile,
                TassoIvaTrimestrale = cliente.TassoIvaTrimestrale,
                
                // Attività Redditi
                Mod730 = cliente.Mod730,
                Mod740 = cliente.Mod740,
                Mod750 = cliente.Mod750,
                Mod760 = cliente.Mod760,
                Mod770 = cliente.Mod770,
                ModCu = cliente.ModCu,
                ModEnc = cliente.ModEnc,
                ModIrap = cliente.ModIrap,
                
                // Attività IVA
                Driva = cliente.Driva,
                Lipe = cliente.Lipe,
                ModTrIva = cliente.ModTrIva,
                
                // Attività Contabile
                Inail = cliente.Inail,
                CassettoFiscale = cliente.CassettoFiscale,
                FatturazioneElettronicaTs = cliente.FatturazioneElettronicaTs,
                Conservazione = cliente.Conservazione,
                Imu = cliente.Imu,
                RegIva = cliente.RegIva,
                RegCespiti = cliente.RegCespiti,
                Inventari = cliente.Inventari,
                LibroGiornale = cliente.LibroGiornale,
                LettereIntento = cliente.LettereIntento,
                ModIntrastat = cliente.ModIntrastat,
                FirmaDigitale = cliente.FirmaDigitale,
                TitolareEffettivo = cliente.TitolareEffettivo,
                
                CodiceAteco = cliente.CodiceAteco,
                
                // Nuovi campi dati cliente
                CfCliente = cliente.CfCliente,
                PivaCliente = cliente.PivaCliente,
                Indirizzo = cliente.Indirizzo,
                Citta = cliente.Citta,
                Provincia = cliente.Provincia,
                Cap = cliente.Cap,
                LegaleRappresentante = cliente.LegaleRappresentante,
                CfLegaleRappresentante = cliente.CfLegaleRappresentante,
                
                // Sezione mandati
                DataMandato = cliente.DataMandato,
                ImportoMandatoAnnuo = cliente.ImportoMandatoAnnuo,
                ProformaTipo = cliente.ProformaTipo,
                
                Attivo = cliente.Attivo,
                DataAttivazione = cliente.DataAttivazione,
                DataModifica = cliente.DataModifica,
                DataCessazione = cliente.DataCessazione,
                RiattivatoPerAnno = cliente.RiattivatoPerAnno,
                DataRiattivazione = cliente.DataRiattivazione
            };
        }

        // GET: Clienti/ListaClientiFiltro
        public async Task<IActionResult> ListaClientiFiltro(string statusFilter = "tutti", string[]? servizi = null, bool schemaCompleto = false)
        {
            // Query base con include per le relazioni
            var clientiQuery = _context.Clienti
                .Include(c => c.Programma)
                .Include(c => c.Professionista)
                .Include(c => c.RegimeContabile)
                .Include(c => c.TipologiaInps)
                .AsQueryable();

            // Applica filtro stato
            clientiQuery = statusFilter.ToLower() switch
            {
                "attivi" => clientiQuery.Where(c => c.Attivo == true),
                "cessati" => clientiQuery.Where(c => c.Attivo == false || c.DataCessazione.HasValue),
                _ => clientiQuery // "tutti" - nessun filtro
            };

            // Se non è schema completo, applica filtro servizi
            if (!schemaCompleto && servizi != null && servizi.Length > 0)
            {
                clientiQuery = clientiQuery.Where(c => 
                    servizi.Any(s => 
                        (s.ToLower() == "mod730" && c.Mod730) ||
                        (s.ToLower() == "mod740" && c.Mod740) ||
                        (s.ToLower() == "mod750" && c.Mod750) ||
                        (s.ToLower() == "mod760" && c.Mod760) ||
                        (s.ToLower() == "mod770" && c.Mod770) ||
                        (s.ToLower() == "modcu" && c.ModCu) ||
                        (s.ToLower() == "modenc" && c.ModEnc) ||
                        (s.ToLower() == "modirap" && c.ModIrap) ||
                        (s.ToLower() == "driva" && c.Driva) ||
                        (s.ToLower() == "lipe" && c.Lipe) ||
                        (s.ToLower() == "modtriva" && c.ModTrIva) ||
                        (s.ToLower() == "inail" && c.Inail) ||
                        (s.ToLower() == "cassettofiscale" && c.CassettoFiscale) ||
                        (s.ToLower() == "fatturazioneelettronica" && c.FatturazioneElettronicaTs) ||
                        (s.ToLower() == "conservazione" && c.Conservazione) ||
                        (s.ToLower() == "imu" && c.Imu) ||
                        (s.ToLower() == "regiva" && c.RegIva) ||
                        (s.ToLower() == "regcespiti" && c.RegCespiti) ||
                        (s.ToLower() == "inventari" && c.Inventari) ||
                        (s.ToLower() == "librogiornale" && c.LibroGiornale) ||
                        (s.ToLower() == "lettereintento" && c.LettereIntento) ||
                        (s.ToLower() == "modintrastat" && c.ModIntrastat) ||
                        (s.ToLower() == "firmadigitale" && c.FirmaDigitale) ||
                        (s.ToLower() == "titolareeffettivo" && c.TitolareEffettivo)
                    )
                );
            }

            var clienti = await clientiQuery.OrderBy(c => c.NomeCliente).ToListAsync();

            // Prepara ViewBag per i filtri
            ViewBag.StatusFilter = statusFilter;
            ViewBag.ServiziFilter = servizi ?? new string[0];
            ViewBag.SchemaCompleto = schemaCompleto;

            return View(clienti);
        }

        // GET: Clienti/ExportClientiFiltro
        public async Task<IActionResult> ExportClientiFiltro(string statusFilter = "tutti", string[]? servizi = null, bool schemaCompleto = false)
        {
            // Riutilizza la stessa logica di filtro
            var clientiQuery = _context.Clienti
                .Include(c => c.Programma)
                .Include(c => c.Professionista)
                .Include(c => c.RegimeContabile)
                .Include(c => c.TipologiaInps)
                .AsQueryable();

            // Applica filtro stato
            clientiQuery = statusFilter.ToLower() switch
            {
                "attivi" => clientiQuery.Where(c => c.Attivo == true),
                "cessati" => clientiQuery.Where(c => c.Attivo == false || c.DataCessazione.HasValue),
                _ => clientiQuery // "tutti" - nessun filtro
            };

            // Se non è schema completo, applica filtro servizi
            if (!schemaCompleto && servizi != null && servizi.Length > 0)
            {
                clientiQuery = clientiQuery.Where(c => 
                    servizi.Any(s => 
                        (s.ToLower() == "mod730" && c.Mod730) ||
                        (s.ToLower() == "mod740" && c.Mod740) ||
                        (s.ToLower() == "mod750" && c.Mod750) ||
                        (s.ToLower() == "mod760" && c.Mod760) ||
                        (s.ToLower() == "mod770" && c.Mod770) ||
                        (s.ToLower() == "modcu" && c.ModCu) ||
                        (s.ToLower() == "modenc" && c.ModEnc) ||
                        (s.ToLower() == "modirap" && c.ModIrap) ||
                        (s.ToLower() == "driva" && c.Driva) ||
                        (s.ToLower() == "lipe" && c.Lipe) ||
                        (s.ToLower() == "modtriva" && c.ModTrIva) ||
                        (s.ToLower() == "inail" && c.Inail) ||
                        (s.ToLower() == "cassettofiscale" && c.CassettoFiscale) ||
                        (s.ToLower() == "fatturazioneelettronica" && c.FatturazioneElettronicaTs) ||
                        (s.ToLower() == "conservazione" && c.Conservazione) ||
                        (s.ToLower() == "imu" && c.Imu) ||
                        (s.ToLower() == "regiva" && c.RegIva) ||
                        (s.ToLower() == "regcespiti" && c.RegCespiti) ||
                        (s.ToLower() == "inventari" && c.Inventari) ||
                        (s.ToLower() == "librogiornale" && c.LibroGiornale) ||
                        (s.ToLower() == "lettereintento" && c.LettereIntento) ||
                        (s.ToLower() == "modintrastat" && c.ModIntrastat) ||
                        (s.ToLower() == "firmadigitale" && c.FirmaDigitale) ||
                        (s.ToLower() == "titolareeffettivo" && c.TitolareEffettivo)
                    )
                );
            }

            var clienti = await clientiQuery.OrderBy(c => c.NomeCliente).ToListAsync();

            // Crea file Excel usando EPPlus
            using var package = new OfficeOpenXml.ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(schemaCompleto ? "Schema Completo" : "Lista Filtrata");
            
            // Header styling
            using (var headerRange = worksheet.Cells["A1:Z1"])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }
            
            if (schemaCompleto)
            {
                // Header per schema completo
                var headers = new[] { "Cliente", "Stato", "Programma", "Professionista",
                    "Mod730", "Mod740", "Mod750", "Mod760", "Mod770", "ModCU", "ModENC", "ModIRAP",
                    "DRIVA", "LIPE", "ModTrIVA", "INAIL", "CassettoFiscale", "FattElettronicaTS",
                    "Conservazione", "IMU", "RegIVA", "RegCespiti", "Inventari", "LibroGiornale",
                    "LettereIntento", "ModINTRASTAT", "FirmaDigitale", "TitolareEffettivo" };
                
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Aggiungi stile header per categorie
                using (var redditiRange = worksheet.Cells["E1:L1"])
                {
                    redditiRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    redditiRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                    redditiRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var ivaRange = worksheet.Cells["M1:O1"])
                {
                    ivaRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ivaRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                    ivaRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                using (var contabileRange = worksheet.Cells["P1:AB1"])
                {
                    contabileRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    contabileRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Blue);
                    contabileRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                
                // Dati per schema completo
                for (int i = 0; i < clienti.Count; i++)
                {
                    var cliente = clienti[i];
                    var row = i + 2;
                    
                    worksheet.Cells[row, 1].Value = cliente.NomeCliente;
                    worksheet.Cells[row, 2].Value = cliente.Attivo ? "Attivo" : "Cessato";
                    worksheet.Cells[row, 3].Value = cliente.Programma?.NomeProgramma ?? "";
                    worksheet.Cells[row, 4].Value = cliente.Professionista?.NomeCompleto ?? "";
                    
                    // Servizi con checkmark
                    worksheet.Cells[row, 5].Value = cliente.Mod730 ? "✓" : "";
                    worksheet.Cells[row, 6].Value = cliente.Mod740 ? "✓" : "";
                    worksheet.Cells[row, 7].Value = cliente.Mod750 ? "✓" : "";
                    worksheet.Cells[row, 8].Value = cliente.Mod760 ? "✓" : "";
                    worksheet.Cells[row, 9].Value = cliente.Mod770 ? "✓" : "";
                    worksheet.Cells[row, 10].Value = cliente.ModCu ? "✓" : "";
                    worksheet.Cells[row, 11].Value = cliente.ModEnc ? "✓" : "";
                    worksheet.Cells[row, 12].Value = cliente.ModIrap ? "✓" : "";
                    worksheet.Cells[row, 13].Value = cliente.Driva ? "✓" : "";
                    worksheet.Cells[row, 14].Value = cliente.Lipe ? "✓" : "";
                    worksheet.Cells[row, 15].Value = cliente.ModTrIva ? "✓" : "";
                    worksheet.Cells[row, 16].Value = cliente.Inail ? "✓" : "";
                    worksheet.Cells[row, 17].Value = cliente.CassettoFiscale ? "✓" : "";
                    worksheet.Cells[row, 18].Value = cliente.FatturazioneElettronicaTs ? "✓" : "";
                    worksheet.Cells[row, 19].Value = cliente.Conservazione ? "✓" : "";
                    worksheet.Cells[row, 20].Value = cliente.Imu ? "✓" : "";
                    worksheet.Cells[row, 21].Value = cliente.RegIva ? "✓" : "";
                    worksheet.Cells[row, 22].Value = cliente.RegCespiti ? "✓" : "";
                    worksheet.Cells[row, 23].Value = cliente.Inventari ? "✓" : "";
                    worksheet.Cells[row, 24].Value = cliente.LibroGiornale ? "✓" : "";
                    worksheet.Cells[row, 25].Value = cliente.LettereIntento ? "✓" : "";
                    worksheet.Cells[row, 26].Value = cliente.ModIntrastat ? "✓" : "";
                    worksheet.Cells[row, 27].Value = cliente.FirmaDigitale ? "✓" : "";
                    worksheet.Cells[row, 28].Value = cliente.TitolareEffettivo ? "✓" : "";
                }
            }
            else
            {
                // Header per lista normale
                worksheet.Cells[1, 1].Value = "Cliente";
                worksheet.Cells[1, 2].Value = "Stato";
                worksheet.Cells[1, 3].Value = "Servizi";
                
                // Dati per lista normale
                for (int i = 0; i < clienti.Count; i++)
                {
                    var cliente = clienti[i];
                    var row = i + 2;
                    
                    var serviziAttivi = new List<string>();
                    if (cliente.Mod730) serviziAttivi.Add("Mod730");
                    if (cliente.Mod740) serviziAttivi.Add("Mod740");
                    if (cliente.Mod750) serviziAttivi.Add("Mod750");
                    if (cliente.Mod760) serviziAttivi.Add("Mod760");
                    if (cliente.Mod770) serviziAttivi.Add("Mod770");
                    if (cliente.ModCu) serviziAttivi.Add("ModCU");
                    if (cliente.ModEnc) serviziAttivi.Add("ModENC");
                    if (cliente.ModIrap) serviziAttivi.Add("ModIRAP");
                    if (cliente.Driva) serviziAttivi.Add("DRIVA");
                    if (cliente.Lipe) serviziAttivi.Add("LIPE");
                    if (cliente.ModTrIva) serviziAttivi.Add("ModTrIVA");
                    if (cliente.Inail) serviziAttivi.Add("INAIL");
                    if (cliente.CassettoFiscale) serviziAttivi.Add("CassettoFiscale");
                    if (cliente.FatturazioneElettronicaTs) serviziAttivi.Add("FattElettronicaTS");
                    if (cliente.Conservazione) serviziAttivi.Add("Conservazione");
                    if (cliente.Imu) serviziAttivi.Add("IMU");
                    if (cliente.RegIva) serviziAttivi.Add("RegIVA");
                    if (cliente.RegCespiti) serviziAttivi.Add("RegCespiti");
                    if (cliente.Inventari) serviziAttivi.Add("Inventari");
                    if (cliente.LibroGiornale) serviziAttivi.Add("LibroGiornale");
                    if (cliente.LettereIntento) serviziAttivi.Add("LettereIntento");
                    if (cliente.ModIntrastat) serviziAttivi.Add("ModINTRASTAT");
                    if (cliente.FirmaDigitale) serviziAttivi.Add("FirmaDigitale");
                    if (cliente.TitolareEffettivo) serviziAttivi.Add("TitolareEffettivo");
                    
                    worksheet.Cells[row, 1].Value = cliente.NomeCliente;
                    worksheet.Cells[row, 2].Value = cliente.Attivo ? "Attivo" : "Cessato";
                    worksheet.Cells[row, 3].Value = string.Join(", ", serviziAttivi);
                }
            }
            
            // Autofit colonne
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            
            var fileContents = package.GetAsByteArray();
            var fileName = $"ClientiFiltro_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // Metodo per gestire la migrazione automatica dei dati tra attività
        private async Task HandleActivityMigration(Cliente cliente, ClientiViewModel viewModel)
        {
            var annoCorrente = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.AnnoCorrente);
            if (annoCorrente == null) return;

            // GESTIONE MOD 730 -> MOD 740
            if (cliente.Mod730 && !viewModel.Mod730 && !cliente.Mod740 && viewModel.Mod740)
            {
                // Migra da 730 a 740
                var attivita730 = await _context.Attivita730
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old730 in attivita730)
                {
                    // Crea nuova attività 740 con VALORI DI DEFAULT (non copia dati da 730)
                    var nuova740 = new Attivita740
                    {
                        IdAnno = old730.IdAnno,
                        IdCliente = old730.IdCliente,
                        IdProfessionista = old730.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 730
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        // Campi specifici 740 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita740.Add(nuova740);
                }
                
                // Rimuovi vecchie attività 730
                _context.Attivita730.RemoveRange(attivita730);
            }
            // GESTIONE MOD 740 -> MOD 730
            else if (cliente.Mod740 && !viewModel.Mod740 && !cliente.Mod730 && viewModel.Mod730)
            {
                // Migra da 740 a 730
                var attivita740 = await _context.Attivita740
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old740 in attivita740)
                {
                    // Crea nuova attività 730 con VALORI DI DEFAULT (non copia dati da 740)
                    var nuova730 = new Attivita730
                    {
                        IdAnno = old740.IdAnno,
                        IdCliente = old740.IdCliente,
                        IdProfessionista = old740.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 740
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr730 = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita730.Add(nuova730);
                }
                
                // Rimuovi vecchie attività 740
                _context.Attivita740.RemoveRange(attivita740);
            }
            // GESTIONE MOD 750 -> MOD 730
            else if (cliente.Mod750 && !viewModel.Mod750 && !cliente.Mod730 && viewModel.Mod730)
            {
                // Migra da 750 a 730
                var attivita750 = await _context.Attivita750
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old750 in attivita750)
                {
                    // Crea nuova attività 730 con VALORI DI DEFAULT (non copia dati da 750)
                    var nuova730 = new Attivita730
                    {
                        IdAnno = old750.IdAnno,
                        IdCliente = old750.IdCliente,
                        IdProfessionista = old750.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 750
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr730 = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita730.Add(nuova730);
                }
                
                // Rimuovi vecchie attività 750
                _context.Attivita750.RemoveRange(attivita750);
            }
            // GESTIONE MOD 750 -> MOD 740
            else if (cliente.Mod750 && !viewModel.Mod750 && !cliente.Mod740 && viewModel.Mod740)
            {
                // Migra da 750 a 740
                var attivita750 = await _context.Attivita750
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old750 in attivita750)
                {
                    // Crea nuova attività 740 con VALORI DI DEFAULT (non copia dati da 750)
                    var nuova740 = new Attivita740
                    {
                        IdAnno = old750.IdAnno,
                        IdCliente = old750.IdCliente,
                        IdProfessionista = old750.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 750
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        // Campi specifici 740 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita740.Add(nuova740);
                }
                
                // Rimuovi vecchie attività 750
                _context.Attivita750.RemoveRange(attivita750);
            }
            // GESTIONE MOD 730 -> MOD 750
            else if (cliente.Mod730 && !viewModel.Mod730 && !cliente.Mod750 && viewModel.Mod750)
            {
                // Migra da 730 a 750
                var attivita730 = await _context.Attivita730
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old730 in attivita730)
                {
                    // Crea nuova attività 750 con VALORI DI DEFAULT (non copia dati da 730)
                    var nuova750 = new Attivita750
                    {
                        IdAnno = old730.IdAnno,
                        IdCliente = old730.IdCliente,
                        IdProfessionista = old730.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 730
                        AppuntamentoDataOra = null, // Campo specifico di 750
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        // Campi specifici 750 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita750.Add(nuova750);
                }
                
                // Rimuovi vecchie attività 730
                _context.Attivita730.RemoveRange(attivita730);
            }
            // GESTIONE MOD 740 -> MOD 750
            else if (cliente.Mod740 && !viewModel.Mod740 && !cliente.Mod750 && viewModel.Mod750)
            {
                // Migra da 740 a 750
                var attivita740 = await _context.Attivita740
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old740 in attivita740)
                {
                    // Crea nuova attività 750 con VALORI DI DEFAULT (non copia dati da 740)
                    var nuova750 = new Attivita750
                    {
                        IdAnno = old740.IdAnno,
                        IdCliente = old740.IdCliente,
                        IdProfessionista = old740.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 740
                        AppuntamentoDataOra = null, // Campo specifico di 750
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        // Campi specifici 750 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita750.Add(nuova750);
                }
                
                // Rimuovi vecchie attività 740
                _context.Attivita740.RemoveRange(attivita740);
            }
            // GESTIONE MOD 760 -> MOD 730
            else if (cliente.Mod760 && !viewModel.Mod760 && !cliente.Mod730 && viewModel.Mod730)
            {
                // Migra da 760 a 730
                var attivita760 = await _context.Attivita760
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old760 in attivita760)
                {
                    // Crea nuova attività 730 con VALORI DI DEFAULT (non copia dati da 760)
                    var nuova730 = new Attivita730
                    {
                        IdAnno = old760.IdAnno,
                        IdCliente = old760.IdCliente,
                        IdProfessionista = old760.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 760
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr730 = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita730.Add(nuova730);
                }
                
                // Rimuovi vecchie attività 760
                _context.Attivita760.RemoveRange(attivita760);
            }
            // GESTIONE MOD 760 -> MOD 740
            else if (cliente.Mod760 && !viewModel.Mod760 && !cliente.Mod740 && viewModel.Mod740)
            {
                // Migra da 760 a 740
                var attivita760 = await _context.Attivita760
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old760 in attivita760)
                {
                    // Crea nuova attività 740 con VALORI DI DEFAULT (non copia dati da 760)
                    var nuova740 = new Attivita740
                    {
                        IdAnno = old760.IdAnno,
                        IdCliente = old760.IdCliente,
                        IdProfessionista = old760.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 760
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        // Campi specifici 740 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita740.Add(nuova740);
                }
                
                // Rimuovi vecchie attività 760
                _context.Attivita760.RemoveRange(attivita760);
            }
            // GESTIONE MOD 760 -> MOD 750
            else if (cliente.Mod760 && !viewModel.Mod760 && !cliente.Mod750 && viewModel.Mod750)
            {
                // Migra da 760 a 750
                var attivita760 = await _context.Attivita760
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old760 in attivita760)
                {
                    // Crea nuova attività 750 con VALORI DI DEFAULT (non copia dati da 760)
                    var nuova750 = new Attivita750
                    {
                        IdAnno = old760.IdAnno,
                        IdCliente = old760.IdCliente,
                        IdProfessionista = old760.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 760
                        AppuntamentoDataOra = null, // Campo specifico di 750
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        RicevutaDr = false,
                        DrFirmata = false,
                        PecInvioDr = false,
                        
                        // Note vuote
                        Note = null,
                        
                        // Campi specifici 750 - valori default
                        FileIsaDisponibile = false,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita750.Add(nuova750);
                }
                
                // Rimuovi vecchie attività 760
                _context.Attivita760.RemoveRange(attivita760);
            }
            // GESTIONE MOD 730 -> MOD 760
            else if (cliente.Mod730 && !viewModel.Mod730 && !cliente.Mod760 && viewModel.Mod760)
            {
                // Migra da 730 a 760
                var attivita730 = await _context.Attivita730
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old730 in attivita730)
                {
                    // Crea nuova attività 760 con VALORI DI DEFAULT (non copia dati da 730)
                    var nuova760 = new Attivita760
                    {
                        IdAnno = old730.IdAnno,
                        IdCliente = old730.IdCliente,
                        IdProfessionista = old730.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 730
                        AppuntamentoDataOra = null, // Campo specifico di 760
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        FileIsa = false,
                        Ricevuta = false,
                        PecInvioDr = false,
                        DrFirmata = false,
                        Cciaa = false,
                        
                        // Campi specifici 760 - valori default
                        BilancioStato = "da_chiudere",
                        BilancioInserimento = "da_inserire",
                        BilancioDeposito = "da_depositare",
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita760.Add(nuova760);
                }
                
                // Rimuovi vecchie attività 730
                _context.Attivita730.RemoveRange(attivita730);
            }
            // GESTIONE MOD 740 -> MOD 760
            else if (cliente.Mod740 && !viewModel.Mod740 && !cliente.Mod760 && viewModel.Mod760)
            {
                // Migra da 740 a 760
                var attivita740 = await _context.Attivita740
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old740 in attivita740)
                {
                    // Crea nuova attività 760 con VALORI DI DEFAULT (non copia dati da 740)
                    var nuova760 = new Attivita760
                    {
                        IdAnno = old740.IdAnno,
                        IdCliente = old740.IdCliente,
                        IdProfessionista = old740.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 740
                        AppuntamentoDataOra = null, // Campo specifico di 760
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        FileIsa = false,
                        Ricevuta = false,
                        PecInvioDr = false,
                        DrFirmata = false,
                        Cciaa = false,
                        
                        // Campi specifici 760 - valori default
                        BilancioStato = "da_chiudere",
                        BilancioInserimento = "da_inserire",
                        BilancioDeposito = "da_depositare",
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita760.Add(nuova760);
                }
                
                // Rimuovi vecchie attività 740
                _context.Attivita740.RemoveRange(attivita740);
            }
            // GESTIONE MOD 750 -> MOD 760
            else if (cliente.Mod750 && !viewModel.Mod750 && !cliente.Mod760 && viewModel.Mod760)
            {
                // Migra da 750 a 760
                var attivita750 = await _context.Attivita750
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old750 in attivita750)
                {
                    // Crea nuova attività 760 con VALORI DI DEFAULT (non copia dati da 750)
                    var nuova760 = new Attivita760
                    {
                        IdAnno = old750.IdAnno,
                        IdCliente = old750.IdCliente,
                        IdProfessionista = old750.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 750
                        AppuntamentoDataOra = null, // Campo comune
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        FileIsa = false,
                        Ricevuta = false,
                        PecInvioDr = false,
                        DrFirmata = false,
                        Cciaa = false,
                        
                        // Campi specifici 760 - valori default
                        BilancioStato = "da_chiudere",
                        BilancioInserimento = "da_inserire",
                        BilancioDeposito = "da_depositare",
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    
                    _context.Attivita760.Add(nuova760);
                }
                
                // Rimuovi vecchie attività 750
                _context.Attivita750.RemoveRange(attivita750);
            }
            
            // GESTIONE MOD ENC
            // MOD 750 -> MOD ENC
            if (cliente.Mod750 && !viewModel.Mod750 && !cliente.ModEnc && viewModel.ModEnc)
            {
                // Migra da 750 a ENC
                var attivita750 = await _context.Attivita750
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var old750 in attivita750)
                {
                    // Crea nuova attività ENC con VALORI DI DEFAULT (non copia dati da 750)
                    var nuovaEnc = new AttivitaEnc
                    {
                        IdAnno = old750.IdAnno,
                        IdCliente = old750.IdCliente,
                        IdProfessionista = old750.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da 750
                        AppuntamentoDataOra = null,
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default
                        FileIsa = false,
                        Ricevuta = false,
                        PecInvioDr = false,
                        DrFirmata = false,
                        Cciaa = false,
                        
                        // F24 - valori di default
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.AttivitaEnc.Add(nuovaEnc);
                }
                
                // Rimuovi vecchie attività 750
                _context.Attivita750.RemoveRange(attivita750);
            }
            // MOD ENC -> MOD 750
            else if (cliente.ModEnc && !viewModel.ModEnc && !cliente.Mod750 && viewModel.Mod750)
            {
                // Migra da ENC a 750
                var attivitaEnc = await _context.AttivitaEnc
                    .Where(a => a.IdCliente == cliente.IdCliente && a.IdAnno == annoCorrente.IdAnno)
                    .ToListAsync();

                foreach (var oldEnc in attivitaEnc)
                {
                    // Crea nuova attività 750 con VALORI DI DEFAULT (non copia dati da ENC)
                    var nuova750 = new Attivita750
                    {
                        IdAnno = oldEnc.IdAnno,
                        IdCliente = oldEnc.IdCliente,
                        IdProfessionista = oldEnc.IdProfessionista, // Mantiene solo il professionista
                        
                        // TUTTI valori di default - NON copiare da ENC
                        AppuntamentoDataOra = null,
                        CodiceDr = null,
                        RaccoltaDocumenti = "da_richiedere",
                        
                        // Stati DR - valori di default
                        DrInserita = false,
                        DrInseritaData = null,
                        IsaDrInseriti = false,
                        IsaDrInseritiData = null,
                        DrControllata = false,
                        DrControllataData = null,
                        DrSpedita = false,
                        DrSpeditaData = null,
                        
                        // Altri stati - valori di default specifici 750
                        FileIsaDisponibile = false,
                        RicevutaDr = false,
                        PecInvioDr = false,
                        DrFirmata = false,
                        Bilancio = "da_chiudere",
                        Cciaa = false,
                        
                        // F24 - valori di default
                        NumeroRateF24PrimoAccontoSaldo = 0,
                        F24PrimoAccontoSaldoConsegnato = false,
                        F24SecondoAcconto = 0,
                        F24SecondoAccontoConsegnato = false,
                        
                        // Note vuote
                        Note = null,
                        
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Attivita750.Add(nuova750);
                }
                
                // Rimuovi vecchie attività ENC
                _context.AttivitaEnc.RemoveRange(attivitaEnc);
            }
            
            // Salva i cambiamenti della migrazione
            await _context.SaveChangesAsync();
        }

        // Metodo per rimuovere cliente dalle liste attività quando checkbox deselezionate
        private async Task RimuoviDalleListeAttivita(int idCliente, int idAnno, List<(string Modulo, string Tabella, int Clienti)> checkboxRimosse)
        {
            foreach (var checkbox in checkboxRimosse)
            {
                switch (checkbox.Tabella)
                {
                    case "attivita_730":
                        var attivita730 = await _context.Attivita730
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.Attivita730.RemoveRange(attivita730);
                        break;

                    case "attivita_740":
                        var attivita740 = await _context.Attivita740
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.Attivita740.RemoveRange(attivita740);
                        break;

                    case "attivita_750":
                        var attivita750 = await _context.Attivita750
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.Attivita750.RemoveRange(attivita750);
                        break;

                    case "attivita_760":
                        var attivita760 = await _context.Attivita760
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.Attivita760.RemoveRange(attivita760);
                        break;

                    case "attivita_770":
                        var attivita770 = await _context.Attivita770
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.Attivita770.RemoveRange(attivita770);
                        break;

                    case "attivita_enc":
                        var attivitaEnc = await _context.AttivitaEnc
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.AttivitaEnc.RemoveRange(attivitaEnc);
                        break;

                    case "attivita_irap":
                        var attivitaIrap = await _context.AttivitaIrap
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.AttivitaIrap.RemoveRange(attivitaIrap);
                        break;

                    case "attivita_cu":
                        var attivitaCu = await _context.AttivitaCu
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.AttivitaCu.RemoveRange(attivitaCu);
                        break;

                    case "attivita_driva":
                        var attivitaDriva = await _context.AttivitaDriva
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.AttivitaDriva.RemoveRange(attivitaDriva);
                        break;

                    case "attivita_lipe":
                        var attivitaLipe = await _context.AttivitaLipe
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.AttivitaLipe.RemoveRange(attivitaLipe);
                        break;
                    case "attivita_mod_tr_iva":
                        var attivitaTriva = await _context.AttivitaTriva
                            .Where(a => a.IdCliente == idCliente && a.IdAnno == idAnno)
                            .ToListAsync();
                        _context.AttivitaTriva.RemoveRange(attivitaTriva);
                        break;

                    case "contabilita_interna_trimestrale":
                        var contabilitaTrimestrale = await _context.ContabilitaInternaTrimestrale
                            .Where(ct => ct.IdCliente == idCliente && ct.IdAnno == idAnno)
                            .ToListAsync();
                        _context.ContabilitaInternaTrimestrale.RemoveRange(contabilitaTrimestrale);
                        Console.WriteLine($"🗑️ Rimosse {contabilitaTrimestrale.Count} contabilità trimestrali per cliente {idCliente}");
                        break;

                    case "contabilita_interna_mensile":
                        // TODO: Implementare quando sarà creata la tabella ContabilitaInternaMensile
                        Console.WriteLine($"NOTA: Rimozione contabilità mensile per cliente {idCliente} - non ancora implementata");
                        break;

                    // TODO: Aggiungere case per attività contabili quando saranno create le tabelle:
                    // case "attivita_inail":
                    // case "attivita_cassetto_fiscale":
                    // case "attivita_fatturazione_elettronica_ts":
                    // case "attivita_conservazione":
                    // case "attivita_imu":
                    // case "attivita_reg_iva":
                    // case "attivita_reg_cespiti":
                    // case "attivita_inventari":
                    // case "attivita_libro_giornale":
                    // case "attivita_lettere_intento":
                    // case "attivita_mod_intrastat":
                    // case "attivita_firma_digitale":
                    // case "attivita_titolare_effettivo":
                    //     // Implementare quando saranno create le tabelle corrispondenti
                    //     break;
                }
            }

            // Salva tutte le rimozioni
            if (checkboxRimosse.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// API per rigenerare le proforma di un cliente (chiamata AJAX)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> RigeneraProforma(int idCliente, DateTime? dataMandato, decimal? importoMandatoAnnuo, string proformaTipo)
        {
            try
            {
                if (dataMandato.HasValue && importoMandatoAnnuo.HasValue && importoMandatoAnnuo.Value > 0 && !string.IsNullOrEmpty(proformaTipo))
                {
                    var proformeGenerate = await _proformaService.RigeneraProformeAsync(idCliente, dataMandato, importoMandatoAnnuo, proformaTipo);
                    if (proformeGenerate.Any())
                    {
                        var cliente = await _context.Clienti.FindAsync(idCliente);
                        var numeroProforms = proformeGenerate.Count;
                        var tipoDesc = proformaTipo.ToLower() == "trimestrale" ? "trimestrali" : "mensili";
                        var message = $"Rigenerate {numeroProforms} proforma {tipoDesc} per {cliente?.NomeCliente}";
                        TempData["ProformaMessage"] = message;
                        return Json(new { success = true, message });
                    }
                }
                return Json(new { success = false, message = "Dati mandato non completi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore nella rigenerazione delle proforma: {ex.Message}" });
            }
        }

        /// <summary>
        /// API per verificare se un cliente ha proforma esistenti (chiamata AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> HasProformeEsistenti(int idCliente)
        {
            var hasProforma = await _proformaService.HasProformeEsistentiAsync(idCliente);
            return Json(new { hasProforma });
        }

        /// <summary>
        /// API per ottenere l'anno fatturazione corrente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAnnoFatturazioneCorrente()
        {
            var annoCorrente = await _context.AnniFatturazione
                .Where(a => a.AnnoCorrente)
                .Select(a => a.Anno)
                .FirstOrDefaultAsync();
            
            return Json(new { anno = annoCorrente });
        }

        /// <summary>
        /// API per ottenere le proforma esistenti di un cliente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProformeCliente(int idCliente)
        {
            try
            {
                var proformeEsistenti = await _proformaService.GetProformeClienteAsync(idCliente);
                var proformeData = proformeEsistenti.Select(p => new
                {
                    idProforma = p.IdProforma,
                    numeroRata = p.NumeroRata,
                    dataScadenza = p.DataScadenza.ToString("yyyy-MM-dd"),
                    importoRata = p.ImportoRata,
                    tipoProforma = p.TipoProforma
                }).ToList();

                return Json(new { success = true, proforma = proformeData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore nel caricamento proforma: {ex.Message}" });
            }
        }

        /// <summary>
        /// API per aggiornare una singola proforma (data e/o importo)
        /// </summary>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateProformaCustom(int idProforma, DateTime dataScadenza, decimal importoRata)
        {
            try
            {
                Console.WriteLine($"[UpdateProformaCustom] Ricevuta richiesta: idProforma={idProforma}, dataScadenza={dataScadenza:yyyy-MM-dd}, importoRata={importoRata}");
                
                // Validazioni essenziali
                if (importoRata <= 0)
                {
                    return Json(new { success = false, message = "L'importo deve essere maggiore di zero." });
                }
                
                // Validazione data: solo date valide, non importa se passate
                if (dataScadenza == DateTime.MinValue)
                {
                    return Json(new { success = false, message = "Inserire una data di scadenza valida." });
                }

                var proforma = await _context.ProformeGenerate.FindAsync(idProforma);
                if (proforma == null)
                {
                    Console.WriteLine($"[UpdateProformaCustom] Proforma non trovata con ID: {idProforma}");
                    return Json(new { success = false, message = "Proforma non trovata." });
                }

                Console.WriteLine($"[UpdateProformaCustom] Proforma trovata: ID={proforma.IdProforma}, Cliente={proforma.IdCliente}, Rata={proforma.NumeroRata}");
                Console.WriteLine($"[UpdateProformaCustom] Valori precedenti: Data={proforma.DataScadenza:yyyy-MM-dd}, Importo={proforma.ImportoRata}");

                // Aggiorna i valori
                proforma.DataScadenza = dataScadenza;
                proforma.ImportoRata = importoRata;
                proforma.UpdatedAt = DateTime.UtcNow;

                Console.WriteLine($"[UpdateProformaCustom] Valori aggiornati: Data={proforma.DataScadenza:yyyy-MM-dd}, Importo={proforma.ImportoRata}");

                var changesCount = await _context.SaveChangesAsync();
                Console.WriteLine($"[UpdateProformaCustom] Salvataggio completato. Righe modificate: {changesCount}");

                return Json(new { 
                    success = true, 
                    message = "Proforma aggiornata con successo.",
                    dataScadenza = dataScadenza.ToString("dd/MM/yyyy"),
                    importoRata = importoRata.ToString("F2")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore nell'aggiornamento: {ex.Message}" });
            }
        }

        /// <summary>
        /// API per aggiornare multiple proforma in batch
        /// </summary>
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateProformeBatch([FromBody] List<ProformaUpdateDto> updates)
        {
            try
            {
                if (updates == null || !updates.Any())
                {
                    return Json(new { success = false, message = "Nessuna modifica da applicare." });
                }

                var proformaIds = updates.Select(u => u.IdProforma).ToList();
                var proformeEsistenti = await _context.ProformeGenerate
                    .Where(p => proformaIds.Contains(p.IdProforma))
                    .ToListAsync();

                foreach (var update in updates)
                {
                    var proforma = proformeEsistenti.FirstOrDefault(p => p.IdProforma == update.IdProforma);
                    if (proforma != null)
                    {
                        // Validazioni essenziali
                        if (update.ImportoRata <= 0)
                        {
                            return Json(new { success = false, message = $"L'importo per la rata {proforma.NumeroRata} deve essere maggiore di zero." });
                        }
                        
                        if (update.DataScadenza == DateTime.MinValue)
                        {
                            return Json(new { success = false, message = $"Data di scadenza non valida per la rata {proforma.NumeroRata}." });
                        }

                        proforma.DataScadenza = update.DataScadenza;
                        proforma.ImportoRata = update.ImportoRata;
                        proforma.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Aggiornate {updates.Count} proforma con successo."
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Errore nell'aggiornamento batch: {ex.Message}" });
            }
        }



        /// <summary>
        /// Genera automaticamente le proforma per un cliente se ha dati mandato completi
        /// </summary>
        private async Task GeneraProformeAutomatiche(int idCliente, DateTime? dataMandato, decimal? importoMandatoAnnuo, string? proformaTipo)
        {
            try
            {
                // Verifica che ci siano tutti i dati necessari
                if (dataMandato.HasValue && 
                    importoMandatoAnnuo.HasValue && 
                    importoMandatoAnnuo.Value > 0 && 
                    !string.IsNullOrEmpty(proformaTipo))
                {
                    var proformeGenerate = await _proformaService.GeneraProformeAsync(
                        idCliente, 
                        dataMandato, 
                        importoMandatoAnnuo, 
                        proformaTipo);

                    if (proformeGenerate.Any())
                    {
                        var cliente = await _context.Clienti.FindAsync(idCliente);
                        var numeroProforms = proformeGenerate.Count;
                        var tipoDesc = proformaTipo.ToLower() == "trimestrale" ? "trimestrali" : "mensili";
                        
                        TempData["ProformaMessage"] = $"Generate automaticamente {numeroProforms} proforma {tipoDesc} per {cliente?.NomeCliente}";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log dell'errore ma non bloccare il salvataggio del cliente
                TempData["ProformaError"] = $"Errore nella generazione automatica delle proforma: {ex.Message}";
            }
        }
    }

    /// <summary>
    /// DTO per l'aggiornamento batch delle proforma
    /// </summary>
    public class ProformaUpdateDto
    {
        public int IdProforma { get; set; }
        public DateTime DataScadenza { get; set; }
        public decimal ImportoRata { get; set; }
    }
}
