using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using ConsultingGroup.Models;
using ConsultingGroup.Services;

namespace ConsultingGroup.Attributes
{
    public class UserPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _permission;

        public UserPermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            
            // Amministratori hanno sempre accesso
            if (user.IsInRole("Administrator"))
            {
                await next();
                return;
            }

            // Controlla se l'utente è autenticato
            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var permissionsService = context.HttpContext.RequestServices.GetRequiredService<IUserPermissionsService>();

            var currentUser = await userManager.GetUserAsync(user);
            if (currentUser == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var hasPermission = await permissionsService.UserCanAccessAsync(currentUser.Id, _permission);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}