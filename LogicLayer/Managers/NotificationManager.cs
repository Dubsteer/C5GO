using LogicLayer.IRepos;
using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.Managers
{
    public class NotificationManager
    {
        private readonly INotificationRepo repo;

        public NotificationManager(INotificationRepo r)
        {
            repo = r;
        }

        public List<Notification> GetForUser(
            int userId,
            int limit = 50,
            bool unreadOnly = false)
        {
            if (userId <= 0)
                return [];

            return repo.GetForUser(userId, Math.Clamp(limit, 1, 100), unreadOnly);
        }

        public int GetUnreadCount(int userId)
        {
            return repo.GetUnreadCount(userId);
        }

        public Notification? MarkAsRead(int notificationId, int userId)
        {
            if (notificationId <= 0 || userId <= 0)
                return null;

            return repo.MarkAsRead(notificationId, userId);
        }

        public int MarkAllAsRead(int userId)
        {
            return userId > 0 ? repo.MarkAllAsRead(userId) : 0;
        }
    }
}
