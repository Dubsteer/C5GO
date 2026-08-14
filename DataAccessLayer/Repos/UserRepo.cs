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
            if (conn.Connection.State != ConnectionState.Open)
                conn.Open();
        }

        private static string? SafeString(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetString(column);
        }

        private static int? SafeIntNullable(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetInt32(column);
        }

        private static DateTime? SafeDateTime(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? null : reader.GetDateTime(column);
        }

        private static bool SafeBool(MySqlDataReader reader, string column)
        {
            return !reader.IsDBNull(column) && reader.GetBoolean(column);
        }

        public void CreateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                INSERT INTO user
                (first_name, last_name, age, username, email, password, is_moderator, steam_id,
                 show_steam_profile,
                 email_confirmed, email_token, token_created_at)
                VALUES
                (@FIRST_NAME, @LAST_NAME, @AGE, @USERNAME, @EMAIL, @PASSWORD, @IS_MODERATOR, @STEAM_ID,
                 @SHOW_STEAM_PROFILE,
                 @EMAIL_CONFIRMED, @EMAIL_TOKEN, @TOKEN_CREATED_AT)
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@FIRST_NAME", user.Firstname ?? "");
            cmd.Parameters.AddWithValue("@LAST_NAME", user.Lastname ?? "");
            cmd.Parameters.AddWithValue("@AGE", user.Age > 0 ? user.Age : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@USERNAME", user.Username);
            cmd.Parameters.AddWithValue("@EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("@IS_MODERATOR", user.IsAdmin);
            cmd.Parameters.AddWithValue("@STEAM_ID",
                string.IsNullOrWhiteSpace(user.SteamId) ? (object)DBNull.Value : user.SteamId);
            cmd.Parameters.AddWithValue("@SHOW_STEAM_PROFILE", user.ShowSteamProfile);

            cmd.Parameters.AddWithValue("@EMAIL_CONFIRMED", user.EmailConfirmed);
            cmd.Parameters.AddWithValue("@EMAIL_TOKEN",
                string.IsNullOrEmpty(user.EmailToken) ? (object)DBNull.Value : user.EmailToken);
            cmd.Parameters.AddWithValue("@TOKEN_CREATED_AT",
                user.TokenCreatedAt.HasValue ? user.TokenCreatedAt.Value : (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public List<User> GetAllUsers()
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, show_steam_profile,
                       email_confirmed, email_token, token_created_at
                FROM user
            ", conn.Connection);

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
                       is_moderator, steam_id, show_steam_profile,
                       email_confirmed, email_token, token_created_at
                FROM user
                WHERE id = @ID
            ", conn.Connection);

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
                       is_moderator, steam_id, show_steam_profile,
                       email_confirmed, email_token, token_created_at
                FROM user
                WHERE email_token = @TOKEN
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@TOKEN", token);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return MapUser(reader);
        }

        public User? GetUserByEmail(string email)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, show_steam_profile,
                       email_confirmed, email_token, token_created_at
                FROM user
                WHERE LOWER(email) = LOWER(@EMAIL)
                LIMIT 1
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@EMAIL", email.Trim());

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapUser(reader) : null;
        }

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
                    steam_id = @STEAM_ID,
                    show_steam_profile = @SHOW_STEAM_PROFILE
                WHERE id = @ID
            ", conn.Connection);

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
            cmd.Parameters.AddWithValue("@SHOW_STEAM_PROFILE", user.ShowSteamProfile);

            cmd.ExecuteNonQuery();
        }

        public void UpdatePassword(int userId, string passwordHash)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                UPDATE user
                SET password = @PASSWORD
                WHERE id = @ID
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@ID", userId);
            cmd.Parameters.AddWithValue("@PASSWORD", passwordHash);
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
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@ID", userId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand("DELETE FROM user WHERE id=@ID", conn.Connection);
            cmd.Parameters.AddWithValue("@ID", user.Id);
            cmd.ExecuteNonQuery();
        }

        public bool UsernameExists(string username)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT 1 FROM user WHERE username=@USERNAME)",
                conn.Connection);

            cmd.Parameters.AddWithValue("@USERNAME", username);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public bool EmailExists(string email)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT 1 FROM user WHERE email=@EMAIL)",
                conn.Connection);

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
                conn.Connection);

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
                )", conn.Connection);

            cmd.Parameters.AddWithValue("@USERNAME", username);
            cmd.Parameters.AddWithValue("@ID", selfId);

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public List<User> SearchUser(string term)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(@"
                SELECT id, first_name, last_name, birthday, age, username, email, password,
                       is_moderator, steam_id, show_steam_profile,
                       email_confirmed, email_token, token_created_at
                FROM user
                WHERE username LIKE CONCAT('%', @TERM, '%')
                   OR email LIKE CONCAT('%', @TERM, '%')
                   OR first_name LIKE CONCAT('%', @TERM, '%')
                   OR last_name LIKE CONCAT('%', @TERM, '%')
            ", conn.Connection);

            cmd.Parameters.AddWithValue("@TERM", term);

            var users = new List<User>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(MapUser(reader));
            }

            return users;
        }

        private static User MapUser(MySqlDataReader reader)
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
                SafeString(reader, "steam_id")
            );

            user.Birthday = SafeDateTime(reader, "birthday");
            user.ShowSteamProfile = SafeBool(reader, "show_steam_profile");
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
               is_moderator, steam_id, show_steam_profile,
               email_confirmed, email_token, token_created_at
        FROM user
        WHERE username = @USERNAME
        LIMIT 1
    ", conn.Connection);

            cmd.Parameters.AddWithValue("@USERNAME", username);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return MapUser(reader);
        }

    }
}
