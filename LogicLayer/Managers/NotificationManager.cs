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

        public List<Notification> GetForUser(int userId)
        {
            return repo.GetForUser(userId);
        }

        public int GetUnreadCount(int userId)
        {
            return repo.GetUnreadCount(userId);
        }

        public Notification? MarkAsRead(int notificationId, int userId)
        {
            return repo.MarkAsRead(notificationId, userId);
        }
    }
}
