using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Models;
using ConsultingGroup.Data;

namespace ConsultingGroup.Services
{
    public class DatabaseSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly ApplicationDbContext _context;

        public DatabaseSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DatabaseSeeder> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Create roles
                await CreateRolesAsync();
                
                // Create admin user
                await CreateAdminUserAsync();
                
                // Create test users for different roles
                await CreateTestUsersAsync();
                
                // Seed Anagrafiche data
                await SeedAnniFiscali();
                await SeedProfessionisti();
                await SeedRegimiContabili();
                await SeedTipologieInps();
                await SeedClienti();
                
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task CreateRolesAsync()
        {
            string[] roles = { "Administrator", "UserSenior", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Role '{Role}' created successfully.", role);
                    }
                    else
                    {
                        _logger.LogError("Failed to create role '{Role}': {Errors}", 
                            role, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private async Task CreateAdminUserAsync()
        {
            const string adminUsername = "admin";
            const string adminEmail = "admin@consultinggroup.com";
            const string adminPassword = "123456";

            var existingUser = await _userManager.FindByNameAsync(adminUsername);
            
            if (existingUser == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Amministratore",
                    LastName = "Sistema",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                
                if (result.Succeeded)
                {
                    // Add to Administrator role
                    await _userManager.AddToRoleAsync(adminUser, "Administrator");
                    _logger.LogInformation("Admin user created successfully with username: {Username}", adminUsername);
                }
                else
                {
                    _logger.LogError("Failed to create admin user: {Errors}", 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                _logger.LogInformation("Admin user already exists.");
                
                // Ensure admin user is in Administrator role
                if (!await _userManager.IsInRoleAsync(existingUser, "Administrator"))
                {
                    await _userManager.AddToRoleAsync(existingUser, "Administrator");
                    _logger.LogInformation("Added Administrator role to existing admin user.");
                }
            }
        }

        private async Task CreateTestUsersAsync()
        {
            // Create Senior User
            const string seniorUsername = "senior";
            const string seniorEmail = "senior@consultinggroup.com";
            const string seniorPassword = "123456";

            var existingSenior = await _userManager.FindByNameAsync(seniorUsername);
            if (existingSenior == null)
            {
                var seniorUser = new ApplicationUser
                {
                    UserName = seniorUsername,
                    Email = seniorEmail,
                    EmailConfirmed = true,
                    FirstName = "Utente",
                    LastName = "Senior",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(seniorUser, seniorPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(seniorUser, "UserSenior");
                    _logger.LogInformation("Senior user created successfully with username: {Username}", seniorUsername);
                }
            }
            else if (!await _userManager.IsInRoleAsync(existingSenior, "UserSenior"))
            {
                await _userManager.AddToRoleAsync(existingSenior, "UserSenior");
            }

            // Create Basic User
            const string userUsername = "user";
            const string userEmail = "user@consultinggroup.com";
            const string userPassword = "123456";

            var existingUser = await _userManager.FindByNameAsync(userUsername);
            if (existingUser == null)
            {
                var basicUser = new ApplicationUser
                {
                    UserName = userUsername,
                    Email = userEmail,
                    EmailConfirmed = true,
                    FirstName = "Utente",
                    LastName = "Base",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(basicUser, userPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(basicUser, "User");
                    _logger.LogInformation("Basic user created successfully with username: {Username}", userUsername);
                }
            }
            else if (!await _userManager.IsInRoleAsync(existingUser, "User"))
            {
                await _userManager.AddToRoleAsync(existingUser, "User");
            }

            // Aggiorna permessi esistenti al nuovo sistema semplificato
            await UpdateExistingUserPermissionsAsync();
        }

        private async Task UpdateExistingUserPermissionsAsync()
        {
            _logger.LogInformation("Aggiornamento permessi esistenti al sistema semplificato...");

            // Ottieni tutti gli utenti con i loro ruoli
            var allUsers = _userManager.Users.ToList();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault();

                if (string.IsNullOrEmpty(primaryRole))
                    continue;

                // Controlla se l'utente ha già i permessi
                var existingPermissions = await _context.UserPermissions
                    .FirstOrDefaultAsync(p => p.UserId == user.Id);

                if (existingPermissions != null)
                {
                    // AGGIORNA i permessi esistenti al nuovo sistema
                    bool needsUpdate = false;

                    if (primaryRole == "UserSenior")
                    {
                        if (!existingPermissions.CanAccessDatiUtenzaRiservata || !existingPermissions.CanAccessDatiUtenzaGenerale)
                        {
                            existingPermissions.CanAccessGestioneClienti = true;
                            existingPermissions.CanAccessDatiUtenzaRiservata = true; // Senior può accedere a dati riservati
                            existingPermissions.CanAccessDatiUtenzaGenerale = true;
                            needsUpdate = true;
                        }
                    }
                    else if (primaryRole == "User")
                    {
                        if (existingPermissions.CanAccessDatiUtenzaRiservata || !existingPermissions.CanAccessDatiUtenzaGenerale)
                        {
                            existingPermissions.CanAccessGestioneClienti = true;
                            existingPermissions.CanAccessDatiUtenzaRiservata = false; // User NON può accedere a dati riservati
                            existingPermissions.CanAccessDatiUtenzaGenerale = true;
                            needsUpdate = true;
                        }
                    }
                    else if (primaryRole == "Administrator")
                    {
                        if (!existingPermissions.CanAccessDatiUtenzaRiservata || !existingPermissions.CanAccessDatiUtenzaGenerale)
                        {
                            existingPermissions.CanAccessGestioneClienti = true;
                            existingPermissions.CanAccessDatiUtenzaRiservata = true; // Admin accesso completo
                            existingPermissions.CanAccessDatiUtenzaGenerale = true;
                            needsUpdate = true;
                        }
                    }

                    if (needsUpdate)
                    {
                        existingPermissions.UpdatedAt = DateTime.UtcNow;
                        existingPermissions.ModifiedBy = "System-Update";
                        _context.Update(existingPermissions);
                        _logger.LogInformation("Permessi aggiornati per utente: {Username} ({Role})", user.UserName, primaryRole);
                    }
                }
                else
                {
                    // CREA nuovi permessi se non esistono
                    var newPermissions = new UserPermissions
                    {
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        ModifiedBy = "System-Update"
                    };

                    if (primaryRole == "UserSenior")
                    {
                        newPermissions.CanAccessGestioneClienti = true;
                        newPermissions.CanAccessDatiUtenzaRiservata = true; // Senior può accedere a dati riservati
                        newPermissions.CanAccessDatiUtenzaGenerale = true;
                        newPermissions.CanAccessAreaAmministrativa = true; // Senior può accedere all'area amministrativa
                        newPermissions.CanAccessAnagrafiche = true; // Senior può accedere alle anagrafiche
                    }
                    else if (primaryRole == "User")
                    {
                        newPermissions.CanAccessGestioneClienti = true;
                        newPermissions.CanAccessDatiUtenzaRiservata = false; // User NON può accedere a dati riservati
                        newPermissions.CanAccessDatiUtenzaGenerale = true;
                        newPermissions.CanAccessAreaAmministrativa = false; // User NON può accedere all'area amministrativa
                        newPermissions.CanAccessAnagrafiche = true; // User può accedere alle anagrafiche
                    }
                    else // Administrator
                    {
                        newPermissions.CanAccessGestioneClienti = true;
                        newPermissions.CanAccessDatiUtenzaRiservata = true; // Admin accesso completo
                        newPermissions.CanAccessDatiUtenzaGenerale = true;
                        newPermissions.CanAccessAreaAmministrativa = true; // Admin può accedere all'area amministrativa
                        newPermissions.CanAccessAnagrafiche = true; // Admin può accedere alle anagrafiche
                    }

                    _context.UserPermissions.Add(newPermissions);
                    _logger.LogInformation("Permessi creati per utente: {Username} ({Role})", user.UserName, primaryRole);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Aggiornamento permessi completato.");
            
            // Seed Anni Fiscali
            await SeedAnniFiscali();
            
            // Seed Professionisti
            await SeedProfessionisti();
            
            // Seed Regimi Contabili
            await SeedRegimiContabili();
            
            // Seed Tipologie INPS
            await SeedTipologieInps();
        }
        
        private async Task SeedAnniFiscali()
        {
            _logger.LogInformation("Verifica e creazione anni fiscali...");
            
            // Controlla se esistono già anni fiscali
            var esisteAnnoFiscale = await _context.AnniFiscali.AnyAsync();
            if (!esisteAnnoFiscale)
            {
                var currentYear = DateTime.Now.Year;
                var currentDateTime = DateTime.UtcNow;
                
                // Anno corrente con scadenze tipiche
                var annoCorrente = new AnnoFiscale
                {
                    Anno = currentYear,
                    Descrizione = $"Anno Fiscale {currentYear}",
                    Attivo = true,
                    AnnoCorrente = true,
                    
                    // Scadenze tipiche italiane per l'anno corrente
                    Scadenza730 = new DateTime(currentYear, 7, 31),      // 31 luglio
                    Scadenza740 = new DateTime(currentYear, 9, 30),      // 30 settembre
                    Scadenza750 = new DateTime(currentYear + 1, 2, 28),  // 28 febbraio (anno successivo)
                    Scadenza760 = new DateTime(currentYear, 10, 31),     // 31 ottobre
                    Scadenza770 = new DateTime(currentYear, 11, 30),     // 30 novembre
                    
                    ScadenzaENC = new DateTime(currentYear, 7, 31),      // 31 luglio
                    ScadenzaIRAP = new DateTime(currentYear, 6, 30),     // 30 giugno
                    ScadenzaCU = new DateTime(currentYear + 1, 2, 28),   // 28 febbraio (anno successivo)
                    ScadenzaDIVA = new DateTime(currentYear + 1, 2, 28), // 28 febbraio (anno successivo) - DRIVA
                    
                    // Lipe trimestrali
                    ScadenzaLipe1t = new DateTime(currentYear, 5, 16),   // 16 maggio
                    ScadenzaLipe2t = new DateTime(currentYear, 8, 20),   // 20 agosto
                    ScadenzaLipe3t = new DateTime(currentYear, 11, 18),  // 18 novembre
                    ScadenzaLipe4t = new DateTime(currentYear + 1, 2, 17), // 17 febbraio (anno successivo)
                    
                    CreatedAt = currentDateTime,
                    UpdatedAt = currentDateTime
                };
                
                // Anno precedente come esempio
                var annoPrecedente = new AnnoFiscale
                {
                    Anno = currentYear - 1,
                    Descrizione = $"Anno Fiscale {currentYear - 1}",
                    Attivo = true,
                    AnnoCorrente = false,
                    
                    // Scadenze già passate per l'anno precedente
                    Scadenza730 = new DateTime(currentYear - 1, 7, 31),
                    Scadenza740 = new DateTime(currentYear - 1, 9, 30),
                    Scadenza750 = new DateTime(currentYear, 2, 28),
                    Scadenza760 = new DateTime(currentYear - 1, 10, 31),
                    Scadenza770 = new DateTime(currentYear - 1, 11, 30),
                    
                    ScadenzaENC = new DateTime(currentYear - 1, 7, 31),
                    ScadenzaIRAP = new DateTime(currentYear - 1, 6, 30),
                    ScadenzaCU = new DateTime(currentYear, 2, 28),
                    ScadenzaDIVA = new DateTime(currentYear, 2, 28),     // DRIVA
                    
                    ScadenzaLipe1t = new DateTime(currentYear - 1, 5, 16),
                    ScadenzaLipe2t = new DateTime(currentYear - 1, 8, 20),
                    ScadenzaLipe3t = new DateTime(currentYear - 1, 11, 18),
                    ScadenzaLipe4t = new DateTime(currentYear, 2, 17),
                    
                    CreatedAt = currentDateTime,
                    UpdatedAt = currentDateTime
                };
                
                // Anno successivo per pianificazione
                var annoSuccessivo = new AnnoFiscale
                {
                    Anno = currentYear + 1,
                    Descrizione = $"Anno Fiscale {currentYear + 1}",
                    Attivo = true,
                    AnnoCorrente = false,
                    
                    // Scadenze complete per l'anno futuro
                    Scadenza730 = new DateTime(currentYear + 1, 7, 31),      // 31 luglio
                    Scadenza740 = new DateTime(currentYear + 1, 9, 30),      // 30 settembre
                    Scadenza750 = new DateTime(currentYear + 2, 2, 28),      // 28 febbraio (anno successivo)
                    Scadenza760 = new DateTime(currentYear + 1, 10, 31),     // 31 ottobre
                    Scadenza770 = new DateTime(currentYear + 1, 11, 30),     // 30 novembre
                    
                    ScadenzaENC = new DateTime(currentYear + 1, 7, 31),      // 31 luglio
                    ScadenzaIRAP = new DateTime(currentYear + 1, 6, 30),     // 30 giugno
                    ScadenzaCU = new DateTime(currentYear + 2, 2, 28),       // 28 febbraio (anno successivo)
                    ScadenzaDIVA = new DateTime(currentYear + 2, 2, 28),     // 28 febbraio (anno successivo) - DRIVA
                    
                    // Lipe trimestrali
                    ScadenzaLipe1t = new DateTime(currentYear + 1, 5, 16),   // 16 maggio
                    ScadenzaLipe2t = new DateTime(currentYear + 1, 8, 20),   // 20 agosto
                    ScadenzaLipe3t = new DateTime(currentYear + 1, 11, 18),  // 18 novembre
                    ScadenzaLipe4t = new DateTime(currentYear + 2, 2, 17),   // 17 febbraio (anno successivo)
                    
                    CreatedAt = currentDateTime,
                    UpdatedAt = currentDateTime
                };
                
                _context.AnniFiscali.AddRange(annoPrecedente, annoCorrente, annoSuccessivo);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Creati {Count} anni fiscali iniziali: {Years}", 
                    3, $"{currentYear - 1}, {currentYear} (corrente), {currentYear + 1}");
            }
            else
            {
                _logger.LogInformation("Anni fiscali già presenti nel database.");
            }
        }
        
        private async Task SeedProfessionisti()
        {
            _logger.LogInformation("Verifica e creazione professionisti...");
            
            // Controlla se esistono già professionisti
            var esisteProfessionista = await _context.Professionisti.AnyAsync();
            if (!esisteProfessionista)
            {
                _logger.LogInformation("Creazione professionisti di esempio...");

                var professionisti = new List<Professionista>
                {
                    new Professionista
                    {
                        Nome = "Mario",
                        Cognome = "Rossi",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-12),
                        DataModifica = DateTime.UtcNow.AddMonths(-12),
                        CreatedAt = DateTime.UtcNow.AddMonths(-12),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-12)
                    },
                    new Professionista
                    {
                        Nome = "Giulia",
                        Cognome = "Bianchi",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-8),
                        DataModifica = DateTime.UtcNow.AddMonths(-8),
                        CreatedAt = DateTime.UtcNow.AddMonths(-8),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-8)
                    },
                    new Professionista
                    {
                        Nome = "Luca",
                        Cognome = "Verdi",
                        Attivo = false,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-18),
                        DataModifica = DateTime.UtcNow.AddMonths(-6),
                        DataCessazione = DateTime.UtcNow.AddMonths(-6),
                        CreatedAt = DateTime.UtcNow.AddMonths(-18),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-6)
                    },
                    new Professionista
                    {
                        Nome = "Anna",
                        Cognome = "Neri",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-3),
                        DataModifica = DateTime.UtcNow.AddMonths(-3),
                        CreatedAt = DateTime.UtcNow.AddMonths(-3),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-3)
                    },
                    new Professionista
                    {
                        Nome = "Franco",
                        Cognome = "Gialli",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-24),
                        DataModifica = DateTime.UtcNow.AddMonths(-1),
                        DataRiattivazione = DateTime.UtcNow.AddMonths(-1),
                        CreatedAt = DateTime.UtcNow.AddMonths(-24),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-1)
                    }
                };

                // Assegna alcuni professionisti all'anno corrente se esiste
                var annoCorrente = await _context.AnniFiscali
                    .Where(a => a.AnnoCorrente)
                    .FirstOrDefaultAsync();

                if (annoCorrente != null)
                {
                    // Assegna gli ultimi due professionisti all'anno corrente
                    professionisti[3].RiattivatoPerAnno = annoCorrente.IdAnno;
                    professionisti[4].RiattivatoPerAnno = annoCorrente.IdAnno;
                }

                _context.Professionisti.AddRange(professionisti);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Creati {Count} professionisti di esempio: {Names}",
                    professionisti.Count,
                    string.Join(", ", professionisti.Select(p => $"{p.Nome} {p.Cognome}")));
            }
            else
            {
                _logger.LogInformation("Professionisti già presenti nel database.");
            }
        }
        
        private async Task SeedRegimiContabili()
        {
            _logger.LogInformation("Verifica e creazione regimi contabili...");
            
            // Controlla se esistono già regimi contabili
            var esisteRegimeContabile = await _context.RegimiContabili.AnyAsync();
            if (!esisteRegimeContabile)
            {
                _logger.LogInformation("Creazione regimi contabili di esempio...");

                var regimiContabili = new List<RegimeContabile>
                {
                    new RegimeContabile
                    {
                        NomeRegime = "Regime Ordinario",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-18),
                        DataModifica = DateTime.UtcNow.AddMonths(-18),
                        CreatedAt = DateTime.UtcNow.AddMonths(-18),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-18)
                    },
                    new RegimeContabile
                    {
                        NomeRegime = "Regime Forfettario",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-12),
                        DataModifica = DateTime.UtcNow.AddMonths(-12),
                        CreatedAt = DateTime.UtcNow.AddMonths(-12),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-12)
                    },
                    new RegimeContabile
                    {
                        NomeRegime = "Regime dei Minimi",
                        Attivo = false,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-24),
                        DataModifica = DateTime.UtcNow.AddMonths(-6),
                        DataCessazione = DateTime.UtcNow.AddMonths(-6),
                        CreatedAt = DateTime.UtcNow.AddMonths(-24),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-6)
                    },
                    new RegimeContabile
                    {
                        NomeRegime = "Regime Startup",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-6),
                        DataModifica = DateTime.UtcNow.AddMonths(-6),
                        CreatedAt = DateTime.UtcNow.AddMonths(-6),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-6)
                    },
                    new RegimeContabile
                    {
                        NomeRegime = "Regime Semplificato",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-3),
                        DataModifica = DateTime.UtcNow.AddMonths(-1),
                        DataRiattivazione = DateTime.UtcNow.AddMonths(-1),
                        CreatedAt = DateTime.UtcNow.AddMonths(-3),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-1)
                    }
                };

                // Assegna alcuni regimi all'anno corrente se esiste
                var annoCorrente = await _context.AnniFiscali
                    .Where(a => a.AnnoCorrente)
                    .FirstOrDefaultAsync();

                if (annoCorrente != null)
                {
                    // Assegna gli ultimi due regimi all'anno corrente
                    regimiContabili[3].RiattivatoPerAnno = annoCorrente.IdAnno;
                    regimiContabili[4].RiattivatoPerAnno = annoCorrente.IdAnno;
                }

                _context.RegimiContabili.AddRange(regimiContabili);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Creati {Count} regimi contabili di esempio: {Names}",
                    regimiContabili.Count,
                    string.Join(", ", regimiContabili.Select(r => r.NomeRegime)));
            }
            else
            {
                _logger.LogInformation("Regimi contabili già presenti nel database.");
            }
        }

        private async Task SeedTipologieInps()
        {
            _logger.LogInformation("Verifica e creazione tipologie INPS...");

            var hasTipologieInps = await _context.TipologieInps.AnyAsync();
            
            if (!hasTipologieInps)
            {
                var tipologieInps = new List<TipologiaInps>
                {
                    new TipologiaInps
                    {
                        Tipologia = "Artigiano",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Commerciante",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Lavoratore Autonomo",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Professionista",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Società di Persone",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Società di Capitali",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Cooperativa",
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow,
                        DataModifica = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new TipologiaInps
                    {
                        Tipologia = "Associazione",
                        Attivo = false, // Esempio di tipologia cessata
                        DataAttivazione = DateTime.UtcNow.AddDays(-100),
                        DataModifica = DateTime.UtcNow.AddDays(-30),
                        DataCessazione = DateTime.UtcNow.AddDays(-30),
                        CreatedAt = DateTime.UtcNow.AddDays(-100),
                        UpdatedAt = DateTime.UtcNow.AddDays(-30)
                    }
                };

                _context.TipologieInps.AddRange(tipologieInps);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Creati {Count} tipologie INPS di esempio: {Names}",
                    tipologieInps.Count,
                    string.Join(", ", tipologieInps.Select(t => t.Tipologia)));
            }
            else
            {
                _logger.LogInformation("Tipologie INPS già presenti nel database.");
            }
        }

        private async Task SeedClienti()
        {
            _logger.LogInformation("Verifica e creazione clienti di esempio...");

            var hasClienti = await _context.Clienti.AnyAsync();

            if (!hasClienti)
            {
                // Recupero dati per le FK
                var programma = await _context.Programmi.FirstOrDefaultAsync(p => p.Attivo);
                var professionista = await _context.Professionisti.FirstOrDefaultAsync(p => p.Attivo);
                var regimeContabile = await _context.RegimiContabili.FirstOrDefaultAsync(r => r.Attivo);
                var tipologiaInps = await _context.TipologieInps.FirstOrDefaultAsync(t => t.Attivo);
                var annoCorrente = await _context.AnniFiscali.FirstOrDefaultAsync(a => a.AnnoCorrente);

                var clienti = new List<Cliente>
                {
                    new Cliente
                    {
                        NomeCliente = "Rossi Mario SRL",
                        MailCliente = "info@rossimario.it",
                        CodiceAteco = "62.01.00",
                        IdProgramma = programma?.IdProgramma,
                        IdProfessionista = professionista?.IdProfessionista,
                        IdRegimeContabile = regimeContabile?.IdRegimeContabile,
                        IdTipologiaInps = tipologiaInps?.IdTipologiaInps,
                        ContabilitaInternaTrimestrale = true, // Interno Trimestrale
                        ContabilitaInternaMensile = true, // Interno Mensile
                        
                        // Attività Redditi
                        Mod730 = true,
                        Mod740 = true,
                        ModCu = true,
                        ModIrap = true,
                        
                        // Attività IVA
                        Driva = true,
                        Lipe = true,
                        
                        // Attività Contabile
                        RegIva = true,
                        LibroGiornale = true,
                        FatturazioneElettronicaTs = true,
                        CassettoFiscale = true,
                        
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-12),
                        DataModifica = DateTime.UtcNow.AddMonths(-1),
                        CreatedAt = DateTime.UtcNow.AddMonths(-12),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-1)
                    },
                    new Cliente
                    {
                        NomeCliente = "Bianchi Consulting",
                        MailCliente = "admin@bianchiconsulting.com",
                        CodiceAteco = "70.22.09",
                        IdProfessionista = professionista?.IdProfessionista,
                        IdRegimeContabile = regimeContabile?.IdRegimeContabile,
                        ContabilitaInternaTrimestrale = false, // Non Trimestrale (Esterno)
                        ContabilitaInternaMensile = false, // Non Mensile
                        
                        // Solo alcune attività
                        Mod740 = true,
                        ModEnc = true,
                        Driva = true,
                        RegIva = true,
                        Conservazione = true,
                        
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-6),
                        DataModifica = DateTime.UtcNow.AddDays(-15),
                        CreatedAt = DateTime.UtcNow.AddMonths(-6),
                        UpdatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new Cliente
                    {
                        NomeCliente = "Verde Commercio",
                        MailCliente = "commercio@verde.it",
                        CodiceAteco = "47.11.10",
                        IdTipologiaInps = tipologiaInps?.IdTipologiaInps,
                        ContabilitaInternaTrimestrale = true, // Interno Trimestrale
                        ContabilitaInternaMensile = false, // Non Mensile
                        
                        // Molte attività
                        Mod730 = true,
                        Mod740 = true,
                        Mod750 = true,
                        ModCu = true,
                        Driva = true,
                        Lipe = true,
                        ModTrIva = true,
                        Inail = true,
                        RegIva = true,
                        RegCespiti = true,
                        LibroGiornale = true,
                        Imu = true,
                        
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-3),
                        DataModifica = DateTime.UtcNow.AddDays(-7),
                        CreatedAt = DateTime.UtcNow.AddMonths(-3),
                        UpdatedAt = DateTime.UtcNow.AddDays(-7)
                    },
                    new Cliente
                    {
                        NomeCliente = "Studio Legale Neri",
                        MailCliente = "info@studioneri.legal",
                        CodiceAteco = "69.10.10",
                        RiattivatoPerAnno = annoCorrente?.IdAnno, // Riattivato per anno corrente
                        DataRiattivazione = DateTime.UtcNow.AddDays(-30),
                        
                        // Attività per studi legali
                        Mod770 = true,
                        ModCu = true,
                        ModEnc = true,
                        CassettoFiscale = true,
                        FatturazioneElettronicaTs = true,
                        FirmaDigitale = true,
                        TitolareEffettivo = true,
                        
                        Attivo = true,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-18),
                        DataModifica = DateTime.UtcNow.AddDays(-30),
                        CreatedAt = DateTime.UtcNow.AddMonths(-18),
                        UpdatedAt = DateTime.UtcNow.AddDays(-30)
                    },
                    new Cliente
                    {
                        NomeCliente = "Gialli Transport SPA",
                        MailCliente = "trasporti@gialli.it",
                        CodiceAteco = "49.41.00",
                        ContabilitaInternaTrimestrale = true, // Interno Trimestrale
                        ContabilitaInternaMensile = true, // Interno Mensile
                        
                        // Cliente cessato
                        Attivo = false,
                        DataAttivazione = DateTime.UtcNow.AddMonths(-24),
                        DataModifica = DateTime.UtcNow.AddMonths(-3),
                        DataCessazione = DateTime.UtcNow.AddMonths(-3),
                        CreatedAt = DateTime.UtcNow.AddMonths(-24),
                        UpdatedAt = DateTime.UtcNow.AddMonths(-3),
                        
                        // Attività che aveva quando era attivo
                        Mod740 = true,
                        ModIrap = true,
                        Driva = true,
                        Lipe = true,
                        Inail = true,
                        RegIva = true,
                        LibroGiornale = true
                    }
                };

                _context.Clienti.AddRange(clienti);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Creati {Count} clienti di esempio: {Names}",
                    clienti.Count,
                    string.Join(", ", clienti.Select(c => c.NomeCliente)));
            }
            else
            {
                _logger.LogInformation("Clienti già presenti nel database.");
            }
        }
    }
}