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
                    }
                    else if (primaryRole == "User")
                    {
                        newPermissions.CanAccessGestioneClienti = true;
                        newPermissions.CanAccessDatiUtenzaRiservata = false; // User NON può accedere a dati riservati
                        newPermissions.CanAccessDatiUtenzaGenerale = true;
                    }
                    else // Administrator
                    {
                        newPermissions.CanAccessGestioneClienti = true;
                        newPermissions.CanAccessDatiUtenzaRiservata = true; // Admin accesso completo
                        newPermissions.CanAccessDatiUtenzaGenerale = true;
                    }

                    _context.UserPermissions.Add(newPermissions);
                    _logger.LogInformation("Permessi creati per utente: {Username} ({Role})", user.UserName, primaryRole);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Aggiornamento permessi completato.");
        }
    }
}