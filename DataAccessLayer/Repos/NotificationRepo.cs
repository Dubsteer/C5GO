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
            if (conn.Connection.State != ConnectionState.Open)
                conn.Open();
        }

        public void Create(int userId, string message, string? link = null)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"INSERT INTO notification (user_id, message, link)
                  VALUES (@u, @m, @l)",
                conn.Connection);

            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@m", message);
            cmd.Parameters.AddWithValue("@l", link);

            cmd.ExecuteNonQuery();
        }

        public List<Notification> GetForUser(int userId, int limit, bool unreadOnly = false)
        {
            EnsureOpen();

            var list = new List<Notification>();
            limit = Math.Clamp(limit, 1, 100);

            var cmd = new MySqlCommand(
                @"SELECT id, message, link, is_read, created_at
                  FROM notification
                  WHERE user_id=@u
                    AND (@unreadOnly=0 OR is_read=0)
                  ORDER BY created_at DESC, id DESC
                  LIMIT @limit",
                conn.Connection);

            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@unreadOnly", unreadOnly);
            cmd.Parameters.AddWithValue("@limit", limit);

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
                conn.Connection);

            cmd.Parameters.AddWithValue("@u", userId);

            return System.Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Notification? MarkAsRead(int notificationId, int userId)
        {
            EnsureOpen();

            using var selectCommand = new MySqlCommand(
                @"SELECT id, user_id, message, link, is_read, created_at
                  FROM notification
                  WHERE id=@id AND user_id=@userId",
                conn.Connection);

            selectCommand.Parameters.AddWithValue("@id", notificationId);
            selectCommand.Parameters.AddWithValue("@userId", userId);

            Notification? notification;
            using (var reader = selectCommand.ExecuteReader())
            {
                if (!reader.Read())
                    return null;

                notification = new Notification
                {
                    Id = reader.GetInt32("id"),
                    UserId = reader.GetInt32("user_id"),
                    Message = reader.GetString("message"),
                    Link = reader.IsDBNull("link") ? null : reader.GetString("link"),
                    IsRead = true,
                    CreatedAt = reader.GetDateTime("created_at")
                };
            }

            using var updateCommand = new MySqlCommand(
                @"UPDATE notification
                  SET is_read=1
                  WHERE id=@id AND user_id=@userId",
                conn.Connection);

            updateCommand.Parameters.AddWithValue("@id", notificationId);
            updateCommand.Parameters.AddWithValue("@userId", userId);
            updateCommand.ExecuteNonQuery();

            return notification;
        }

        public int MarkAllAsRead(int userId)
        {
            EnsureOpen();

            using var command = new MySqlCommand(
                @"UPDATE notification
                  SET is_read=1
                  WHERE user_id=@userId AND is_read=0",
                conn.Connection);

            command.Parameters.AddWithValue("@userId", userId);
            return command.ExecuteNonQuery();
        }
    }
}
