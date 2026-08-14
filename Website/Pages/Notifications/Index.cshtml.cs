using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Notifications;

[Authorize]
public class IndexModel : PageModel
{
    private readonly NotificationManager notificationManager;

    public IndexModel(NotificationManager notificationManager)
    {
        this.notificationManager = notificationManager;
    }

    public IReadOnlyList<Notification> Notifications { get; private set; } = [];
    public int UnreadCount { get; private set; }
    public bool UnreadOnly { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public IActionResult OnGet(bool unreadOnly = false)
    {
        if (!TryGetUserId(out var userId))
            return Challenge();

        UnreadOnly = unreadOnly;
        Notifications = notificationManager.GetForUser(userId, 100, unreadOnly);
        UnreadCount = notificationManager.GetUnreadCount(userId);
        return Page();
    }

    public IActionResult OnPostMarkAllRead(bool unreadOnly = false)
    {
        if (!TryGetUserId(out var userId))
            return Challenge();

        var updated = notificationManager.MarkAllAsRead(userId);
        StatusMessage = updated == 0
            ? "You have no unread notifications."
            : updated == 1
                ? "1 notification marked as read."
                : $"{updated} notifications marked as read.";

        return RedirectToPage(new { unreadOnly });
    }

    private bool TryGetUserId(out int userId)
    {
        return int.TryParse(User.FindFirst("id")?.Value, out userId);
    }
}
