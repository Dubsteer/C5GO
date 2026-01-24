using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface INotificationRepo
    {
        void Create(int userId, string message, string? link = null);
        List<Notification> GetForUser(int userId);
        int GetUnreadCount(int userId);
        void MarkAsRead(int notificationId);
    }
}
