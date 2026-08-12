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

    public List<Notification> GetForUser(int userId) =>
        Notifications.Where(item => item.UserId == userId).ToList();

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
}
