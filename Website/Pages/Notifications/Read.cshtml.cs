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

        public IActionResult OnPost(int id)
        {
            if (!int.TryParse(User.FindFirst("id")?.Value, out var userId))
                return Challenge();

            var notification = manager.MarkAsRead(id, userId);
            if (notification == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(notification.Link) &&
                Url.IsLocalUrl(notification.Link))
            {
                return LocalRedirect(notification.Link);
            }

            return RedirectToPage("/Notifications/Index");
        }
    }
}
