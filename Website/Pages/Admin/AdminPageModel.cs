using LogicLayer.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    public abstract class AdminPageModel : PageModel
    {
        protected readonly UserManager userManager;

        protected AdminPageModel(UserManager userManager)
        {
            this.userManager = userManager;
        }

        protected IActionResult? RequireAdmin()
        {
            if (!User.Identity!.IsAuthenticated)
                return RedirectToPage("/Login");

            if (!userManager.IsAdmin(User.Identity.Name!))
                return StatusCode(403);

            return null;
        }
    }
}
