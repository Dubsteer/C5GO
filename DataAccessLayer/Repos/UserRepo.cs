using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

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

        private string SafeString(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? "" : reader.GetString(column);
        }

        private int SafeInt(MySqlDataReader reader, string column)
        {
            return reader.IsDBNull(column) ? 0 : reader.GetInt32(column);
        }

        private bool SafeBool(MySqlDataReader reader, string column)
        {
            return !reader.IsDBNull(column) && reader.GetBoolean(column);
        }

        public void CreateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"INSERT INTO user 
                (first_name, last_name, age, username, email, password, is_moderator, steam_id)
                VALUES (@FIRST_NAME, @LAST_NAME, @AGE, @USERNAME, @EMAIL, @PASSWORD, @IS_MODERATOR, @STEAM_ID)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@FIRST_NAME", user.Firstname ?? "");
            cmd.Parameters.AddWithValue("@LAST_NAME", user.Lastname ?? "");
            cmd.Parameters.AddWithValue("@AGE", user.Age);
            cmd.Parameters.AddWithValue("@USERNAME", user.Username);
            cmd.Parameters.AddWithValue("@EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("@IS_MODERATOR", user.IsAdmin);
            cmd.Parameters.AddWithValue("@STEAM_ID", user.SteamId ?? "00000000");

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error creating user", ex);
            }
        }

        public List<User> GetAllUsers()
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, first_name, last_name, age, username, email, password, is_moderator, steam_id 
                  FROM user",
                conn.GetInnerConn());

            var users = new List<User>();

            try
            {
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User(
                        reader.GetInt32("id"),
                        SafeString(reader, "first_name"),
                        SafeString(reader, "last_name"),
                        SafeInt(reader, "age"),
                        SafeString(reader, "username"),
                        SafeString(reader, "email"),
                        SafeString(reader, "password"),
                        SafeBool(reader, "is_moderator"),
                        SafeString(reader, "steam_id")
                    ));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading users", ex);
            }

            return users;
        }

        public User? GetUserById(int id)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, first_name, last_name, age, username, email, password, is_moderator, steam_id
                  FROM user
                  WHERE id = @ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", id);

            try
            {
                using var reader = cmd.ExecuteReader();

                if (!reader.Read()) return null;

                return new User(
                    reader.GetInt32("id"),
                    SafeString(reader, "first_name"),
                    SafeString(reader, "last_name"),
                    SafeInt(reader, "age"),
                    SafeString(reader, "username"),
                    SafeString(reader, "email"),
                    SafeString(reader, "password"),
                    SafeBool(reader, "is_moderator"),
                    SafeString(reader, "steam_id")
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading user by ID", ex);
            }
        }

        public void UpdateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"UPDATE user SET
                    first_name=@FIRST_NAME,
                    last_name=@LAST_NAME,
                    age=@AGE,
                    username=@USERNAME,
                    email=@EMAIL,
                    password=@PASSWORD,
                    is_moderator=@IS_MODERATOR,
                    steam_id=@STEAM_ID
                WHERE id=@ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", user.Id);
            cmd.Parameters.AddWithValue("@FIRST_NAME", user.Firstname ?? "");
            cmd.Parameters.AddWithValue("@LAST_NAME", user.Lastname ?? "");
            cmd.Parameters.AddWithValue("@AGE", user.Age);
            cmd.Parameters.AddWithValue("@USERNAME", user.Username);
            cmd.Parameters.AddWithValue("@EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("@IS_MODERATOR", user.IsAdmin);
            cmd.Parameters.AddWithValue("@STEAM_ID", user.SteamId ?? "00000000");

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error updating user", ex);
            }
        }

        public void DeleteUser(User user)
        {
            EnsureConnection();

            var cmd1 = new MySqlCommand("DELETE FROM comment WHERE authorid=@ID", conn.GetInnerConn());
            cmd1.Parameters.AddWithValue("@ID", user.Id);
            cmd1.ExecuteNonQuery();

            var cmd2 = new MySqlCommand("DELETE FROM applications WHERE playerId=@ID", conn.GetInnerConn());
            cmd2.Parameters.AddWithValue("@ID", user.Id);
            cmd2.ExecuteNonQuery();

            var cmd3 = new MySqlCommand("DELETE FROM user WHERE id=@ID", conn.GetInnerConn());
            cmd3.Parameters.AddWithValue("@ID", user.Id);

            try
            {
                cmd3.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error deleting user", ex);
            }
        }

        public bool CheckIfUsernameExists(string username, int selfId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT EXISTS(
                    SELECT 1 FROM user
                    WHERE username = BINARY @USERNAME AND id != @ID
                )",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@USERNAME", username);
            cmd.Parameters.AddWithValue("@ID", selfId);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error checking username", ex);
            }
        }

        public List<User> SearchUser(string term)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"SELECT id, first_name, last_name, age, username, email, password, is_moderator, steam_id
                  FROM user
                  WHERE username LIKE CONCAT('%', @term, '%')",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@term", term);

            var users = new List<User>();

            try
            {
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User(
                        reader.GetInt32("id"),
                        SafeString(reader, "first_name"),
                        SafeString(reader, "last_name"),
                        SafeInt(reader, "age"),
                        SafeString(reader, "username"),
                        SafeString(reader, "email"),
                        SafeString(reader, "password"),
                        SafeBool(reader, "is_moderator"),
                        SafeString(reader, "steam_id")
                    ));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Search failed", ex);
            }

            return users;
        }
    }
}
