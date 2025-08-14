using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultingGroup.Models;
using ConsultingGroup.ViewModels;
using ConsultingGroup.Data;

namespace ConsultingGroup.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<UserManagementController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
        }

        // GET: UserManagement
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = roles.FirstOrDefault() ?? "User",
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                });
            }

            return View(userViewModels);
        }

        // GET: UserManagement/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await GetRolesSelectListAsync();
            return View();
        }

        // POST: UserManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("UserName", "Nome utente già esistente.");
                    ViewBag.Roles = await GetRolesSelectListAsync();
                    return View(model);
                }

                // Check if email already exists
                existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email già esistente.");
                    ViewBag.Roles = await GetRolesSelectListAsync();
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add role
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    _logger.LogInformation("User '{UserName}' created successfully.", model.UserName);
                    TempData["SuccessMessage"] = $"Utente '{model.UserName}' creato con successo.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Roles = await GetRolesSelectListAsync();
            return View(model);
        }

        // GET: UserManagement/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "User",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            ViewBag.Roles = await GetRolesSelectListAsync();
            return View(model);
        }

        // POST: UserManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Check if username already exists (excluding current user)
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("UserName", "Nome utente già esistente.");
                    ViewBag.Roles = await GetRolesSelectListAsync();
                    return View(model);
                }

                // Check if email already exists (excluding current user)
                existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "Email già esistente.");
                    ViewBag.Roles = await GetRolesSelectListAsync();
                    return View(model);
                }

                // Update user properties
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.IsActive = model.IsActive;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update password if provided
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                        
                        if (!passwordResult.Succeeded)
                        {
                            foreach (var error in passwordResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            ViewBag.Roles = await GetRolesSelectListAsync();
                            return View(model);
                        }
                    }

                    // Update role
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    _logger.LogInformation("User '{UserName}' updated successfully.", model.UserName);
                    TempData["SuccessMessage"] = $"Utente '{model.UserName}' aggiornato con successo.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            ViewBag.Roles = await GetRolesSelectListAsync();
            return View(model);
        }

        // GET: UserManagement/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? "User",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return View(model);
        }

        // POST: UserManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting the current user
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["ErrorMessage"] = "Non puoi eliminare il tuo stesso account.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Use a transaction to ensure atomicity
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    // Check if user has created any data using the injected context
                    bool hasData = await _context.Banche.AnyAsync(b => b.UserId == user.Id) ||
                                  await _context.CarteCredito.AnyAsync(c => c.UserId == user.Id) ||
                                  await _context.Utenze.AnyAsync(u => u.UserId == user.Id) ||
                                  await _context.Cancelleria.AnyAsync(c => c.UserId == user.Id) ||
                                  await _context.Mail.AnyAsync(m => m.UserId == user.Id) ||
                                  await _context.UtentiPC.AnyAsync(u => u.UserId == user.Id) ||
                                  await _context.AltriDati.AnyAsync(a => a.UserId == user.Id) ||
                                  await _context.Entratel.AnyAsync(e => e.UserId == user.Id) ||
                                  await _context.UtentiTS.AnyAsync(u => u.UserId == user.Id) ||
                                  await _context.UserPermissions.AnyAsync(p => p.UserId == user.Id);

                if (hasData)
                {
                    // If user has data, deactivate instead of delete
                    user.IsActive = false;
                    user.UserName = $"DISABLED_{user.UserName}_{DateTime.Now:yyyyMMdd}";
                    
                    // Remove all roles
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                    }
                    
                    // Remove user permissions
                    var userPermissions = await _context.UserPermissions.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (userPermissions != null)
                    {
                        _context.UserPermissions.Remove(userPermissions);
                        await _context.SaveChangesAsync();
                    }
                    
                    var updateResult = await _userManager.UpdateAsync(user);
                    
                    if (updateResult.Succeeded)
                    {
                        await transaction.CommitAsync();
                        _logger.LogInformation("User '{UserName}' deactivated (had associated data).", user.UserName);
                        TempData["SuccessMessage"] = $"Utente disabilitato con successo. L'utente aveva dati associati e non può essere eliminato completamente.";
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                        TempData["ErrorMessage"] = $"Errore durante la disabilitazione dell'utente: {errors}";
                    }
                }
                else
                {
                    // Remove user permissions first
                    var userPermissions = await _context.UserPermissions.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (userPermissions != null)
                    {
                        _context.UserPermissions.Remove(userPermissions);
                        await _context.SaveChangesAsync();
                    }
                    
                    // Remove user roles
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                    }

                    // Delete the user (no associated data)
                    var result = await _userManager.DeleteAsync(user);
                    
                    if (result.Succeeded)
                    {
                        await transaction.CommitAsync();
                        _logger.LogInformation("User '{UserName}' deleted successfully.", user.UserName);
                        TempData["SuccessMessage"] = $"Utente '{user.UserName}' eliminato con successo.";
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to delete user '{UserName}': {Errors}", user.UserName, errors);
                        TempData["ErrorMessage"] = $"Errore durante l'eliminazione dell'utente: {errors}";
                    }
                }
                
                // Close inner try block for transaction
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while deleting user '{UserName}'", user.UserName);
                TempData["ErrorMessage"] = "Errore del database durante l'eliminazione dell'utente. Contattare l'amministratore.";
            }
            catch (InvalidOperationException ioEx)
            {
                _logger.LogError(ioEx, "Invalid operation error occurred while deleting user '{UserName}'", user.UserName);
                TempData["ErrorMessage"] = "Operazione non valida durante l'eliminazione dell'utente. Riprova.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting user '{UserName}'", user.UserName);
                TempData["ErrorMessage"] = $"Errore imprevisto durante l'eliminazione dell'utente. ID: {HttpContext.TraceIdentifier}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: UserManagement/ResetPassword/5
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id,
                UserName = user.UserName!
            };

            return View(model);
        }

        // POST: UserManagement/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password reset for user '{UserName}'.", user.UserName);
                    TempData["SuccessMessage"] = $"Password resetata con successo per l'utente '{user.UserName}'.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        private async Task<Microsoft.AspNetCore.Mvc.Rendering.SelectList> GetRolesSelectListAsync()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roles);
        }
    }
}