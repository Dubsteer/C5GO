using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface INotificationRepo
    {
        void Create(int userId, string message, string? link = null);
        List<Notification> GetForUser(int userId, int limit, bool unreadOnly = false);
        int GetUnreadCount(int userId);
        Notification? MarkAsRead(int notificationId, int userId);
        int MarkAllAsRead(int userId);
    }
}
