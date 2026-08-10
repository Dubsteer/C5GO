using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly IConnection conn;

        public UserRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureConnection()
        {
            if (conn.GetInnerConn().State != ConnectionState.Open)
                conn.Open();
        }

        // =========================
        // SAFE READERS (NULL SAFE)
        // =========================
        private string? SafeString(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetString(column);
        }

        private int? SafeIntNullable(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetInt32(column);
        }

        private DateTime? SafeDateTime(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetDateTime(column);
        }

        private bool SafeBool(MySqlDataReader reader, string column)
        {
            return !reader.IsDBNull(column) && reader.GetBoolean(column);
        }

        // =========================
        // CREATE
        // =========================
        public void CreateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                INSERT INTO user
                (first_name, last_name, age, username, email, password, is_moderator, steam_id,
                 email_confirmed, email_token, token_created_at)
                VALUES
                (@FIRST_NAME, @LAST_NAME, @AGE, @USERNAME, @EMAIL, @PASSWORD, @IS_MODERATOR, @STEAM_ID,
                 @EMAIL_CONFIRMED, @EMAIL_TOKEN, @TOKEN_CREATED_AT)
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@FIRST_NAME", user.Firstname ?? "");
            cmd.Parameters.AddWithValue("@LAST_NAME", user.Lastname ?? "");
            cmd.Parameters.AddWithValue("@AGE", user.Age > 0 ? user.Age : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@USERNAME", user.Username);
            cmd.Parameters.AddWithValue("@EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("@IS_MODERATOR", user.IsAdmin);
            cmd.Parameters.AddWithValue("@STEAM_ID",
                string.IsNullOrWhiteSpace(user.SteamId) ? (object)DBNull.Value : user.SteamId);

            cmd.Parameters.AddWithValue("@EMAIL_CONFIRMED", user.EmailConfirmed);
            cmd.Parameters.AddWithValue("@EMAIL_TOKEN",
                string.IsNullOrEmpty(user.EmailToken) ? (object)DBNull.Value : user.EmailToken);
            cmd.Parameters.AddWithValue("@TOKEN_CREATED_AT",
                user.TokenCreatedAt.HasValue ? user.TokenCreatedAt.Value : (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        // =========================
        // READ
        // =========================
        public List<User> GetAllUsers()
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, email_confirmed, email_token, token_created_at
                FROM user
            ", conn.GetInnerConn());

            var users = new List<User>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        public User? GetUserById(int id)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, email_confirmed, email_token, token_created_at
                FROM user
                WHERE id = @ID
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return MapUser(reader);
        }

        public User? GetUserByEmailToken(string token)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, email_confirmed, email_token, token_created_at
                FROM user
                WHERE email_token = @TOKEN
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@TOKEN", token);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return MapUser(reader);
        }

        // =========================
        // UPDATE
        // =========================
        public void UpdateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                UPDATE user SET
                    first_name = @FIRST_NAME,
                    last_name = @LAST_NAME,
                    age = @AGE,
                    username = @USERNAME,
                    email = @EMAIL,
                    password = @PASSWORD,
                    is_moderator = @IS_MODERATOR,
                    steam_id = @STEAM_ID
                WHERE id = @ID
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", user.Id);
            cmd.Parameters.AddWithValue("@FIRST_NAME", user.Firstname ?? "");
            cmd.Parameters.AddWithValue("@LAST_NAME", user.Lastname ?? "");
            cmd.Parameters.AddWithValue("@AGE", user.Age > 0 ? user.Age : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@USERNAME", user.Username);
            cmd.Parameters.AddWithValue("@EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("@IS_MODERATOR", user.IsAdmin);
            cmd.Parameters.AddWithValue("@STEAM_ID",
                string.IsNullOrWhiteSpace(user.SteamId) ? (object)DBNull.Value : user.SteamId);

            cmd.ExecuteNonQuery();
        }

        public void ConfirmEmail(int userId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                UPDATE user
                SET email_confirmed = 1,
                    email_token = NULL,
                    token_created_at = NULL
                WHERE id = @ID
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", userId);
            cmd.ExecuteNonQuery();
        }

        // =========================
        // DELETE
        // =========================
        public void DeleteUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand("DELETE FROM user WHERE id=@ID", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@ID", user.Id);
            cmd.ExecuteNonQuery();
        }

        // =========================
        // CHECKS
        // =========================
        public bool UsernameExists(string username)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT 1 FROM user WHERE username=@USERNAME)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@USERNAME", username);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public bool EmailExists(string email)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT 1 FROM user WHERE email=@EMAIL)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@EMAIL", email);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public bool SteamIdExists(string steamId)
        {
            if (string.IsNullOrWhiteSpace(steamId))
                return false;

            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT 1 FROM user WHERE steam_id=@STEAM_ID)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@STEAM_ID", steamId);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public bool CheckIfUsernameExists(string username, int selfId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT EXISTS(
                    SELECT 1 FROM user
                    WHERE username=@USERNAME AND id != @ID
                )", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@USERNAME", username);
            cmd.Parameters.AddWithValue("@ID", selfId);

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public List<User> SearchUser(string term)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, email_confirmed, email_token, token_created_at
                FROM user
                WHERE username LIKE CONCAT('%', @TERM, '%')
                   OR email LIKE CONCAT('%', @TERM, '%')
                   OR first_name LIKE CONCAT('%', @TERM, '%')
                   OR last_name LIKE CONCAT('%', @TERM, '%')
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@TERM", term);

            var users = new List<User>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        // =========================
        // MAPPER (NULL SAFE)
        // =========================
        private User MapUser(MySqlDataReader reader)
        {
            var user = new User(
                reader.GetInt32("id"),
                SafeString(reader, "first_name") ?? "",
                SafeString(reader, "last_name") ?? "",
                SafeIntNullable(reader, "age") ?? 0,
                SafeString(reader, "username") ?? "",
                SafeString(reader, "email") ?? "",
                SafeString(reader, "password") ?? "",
                SafeBool(reader, "is_moderator"),
                SafeString(reader, "steam_id") ?? "0"
            );

            user.Birthday = SafeDateTime(reader, "birthday");
            user.EmailConfirmed = SafeBool(reader, "email_confirmed");
            user.EmailToken = SafeString(reader, "email_token");
            user.TokenCreatedAt = SafeDateTime(reader, "token_created_at");

            return user;
        }

        public User? GetUserByUsername(string username)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
        SELECT id, first_name, last_name, birthday, age, username, email, password,
               is_moderator, steam_id, email_confirmed, email_token, token_created_at
        FROM user
        WHERE username = @USERNAME
        LIMIT 1
    ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@USERNAME", username);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return MapUser(reader);
        }

    }
}
