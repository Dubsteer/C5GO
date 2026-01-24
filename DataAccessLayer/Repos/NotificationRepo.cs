using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class NotificationRepo : INotificationRepo
    {
        private readonly IConnection conn;

        public NotificationRepo(IConnection c)
        {
            conn = c;
        }

        private void EnsureOpen()
        {
            if (conn.GetInnerConn().State != ConnectionState.Open)
                conn.Open();
        }

        public void Create(int userId, string message, string? link = null)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"INSERT INTO notification (user_id, message, link)
                  VALUES (@u, @m, @l)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@m", message);
            cmd.Parameters.AddWithValue("@l", link);

            cmd.ExecuteNonQuery();
        }

        public List<Notification> GetForUser(int userId)
        {
            EnsureOpen();

            var list = new List<Notification>();

            var cmd = new MySqlCommand(
                @"SELECT id, message, link, is_read, created_at
                  FROM notification
                  WHERE user_id=@u
                  ORDER BY created_at DESC",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@u", userId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Notification
                {
                    Id = r.GetInt32("id"),
                    UserId = userId,
                    Message = r.GetString("message"),
                    Link = r.IsDBNull("link") ? null : r.GetString("link"),
                    IsRead = r.GetBoolean("is_read"),
                    CreatedAt = r.GetDateTime("created_at")
                });
            }

            return list;
        }

        public int GetUnreadCount(int userId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"SELECT COUNT(*) FROM notification
                  WHERE user_id=@u AND is_read=0",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@u", userId);

            return System.Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void MarkAsRead(int notificationId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"UPDATE notification SET is_read=1 WHERE id=@id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", notificationId);
            cmd.ExecuteNonQuery();
        }
    }
}
