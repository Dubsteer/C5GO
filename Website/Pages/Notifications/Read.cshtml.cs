using LogicLayer.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Notifications
{
    [Authorize]
    public class ReadModel : PageModel
    {
        private readonly NotificationManager manager;

        public ReadModel(NotificationManager m)
        {
            manager = m;
        }

        public IActionResult OnGet(int id, string link)
        {
            manager.MarkAsRead(id);

            if (!string.IsNullOrEmpty(link))
                return Redirect(link);

            return RedirectToPage("/Index");
        }
    }
}
