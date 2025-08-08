using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ConsultingGroup.Models;
using ConsultingGroup.Services;

namespace ConsultingGroup.ViewComponents
{
    public class UserPermissionsViewComponent : ViewComponent
    {
        private readonly IUserPermissionsService _permissionsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserPermissionsViewComponent(
            IUserPermissionsService permissionsService,
            UserManager<ApplicationUser> userManager)
        {
            _permissionsService = permissionsService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Content("");
            }

            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (user == null)
            {
                return Content("");
            }

            var permissions = await _permissionsService.GetOrCreateUserPermissionsAsync(user.Id);
            return View(permissions);
        }
    }
}