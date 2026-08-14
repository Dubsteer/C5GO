using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos;

public class MockNotificationRepo : INotificationRepo
{
    public List<Notification> Notifications { get; } = [];

    public void Create(int userId, string message, string? link = null)
    {
        Notifications.Add(new Notification
        {
            Id = Notifications.Count + 1,
            UserId = userId,
            Message = message,
            Link = link,
            CreatedAt = DateTime.UtcNow
        });
    }

    public List<Notification> GetForUser(int userId, int limit, bool unreadOnly = false) =>
        Notifications
            .Where(item => item.UserId == userId && (!unreadOnly || !item.IsRead))
            .OrderByDescending(item => item.CreatedAt)
            .ThenByDescending(item => item.Id)
            .Take(limit)
            .ToList();

    public int GetUnreadCount(int userId) =>
        Notifications.Count(item => item.UserId == userId && !item.IsRead);

    public Notification? MarkAsRead(int notificationId, int userId)
    {
        var notification = Notifications.FirstOrDefault(item =>
            item.Id == notificationId && item.UserId == userId);
        if (notification != null)
            notification.IsRead = true;
        return notification;
    }

    public int MarkAllAsRead(int userId)
    {
        var unread = Notifications.Where(item => item.UserId == userId && !item.IsRead).ToList();
        foreach (var notification in unread)
            notification.IsRead = true;

        return unread.Count;
    }
}
