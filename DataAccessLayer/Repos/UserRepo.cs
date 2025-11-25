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
            if (conn.GetInnerConn().State != System.Data.ConnectionState.Open)
                conn.Open();
        }

        public void CreateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                @"INSERT INTO user 
                  (first_name, last_name, age, username, email, password, is_moderator, steam_id)
                  VALUES (@FIRST_NAME, @LAST_NAME, @AGE, @USERNAME, @EMAIL, @PASSWORD, @IS_MODERATOR, @STEAM_ID)",
                conn.GetInnerConn());

            cmd.Parameters.Add("@FIRST_NAME", MySqlDbType.VarChar).Value = user.Firstname;
            cmd.Parameters.Add("@LAST_NAME", MySqlDbType.VarChar).Value = user.Lastname;
            cmd.Parameters.Add("@AGE", MySqlDbType.Int32).Value = user.Age;
            cmd.Parameters.Add("@USERNAME", MySqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@EMAIL", MySqlDbType.VarChar).Value = user.Gmail;
            cmd.Parameters.Add("@PASSWORD", MySqlDbType.VarChar).Value = user.Password;
            cmd.Parameters.Add("@IS_MODERATOR", MySqlDbType.Bit).Value = user.IsAdmin;
            cmd.Parameters.Add("@STEAM_ID", MySqlDbType.VarChar).Value = user.SteamId ?? "0";

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
                        reader.GetString("first_name"),
                        reader.GetString("last_name"),
                        reader.GetInt32("age"),
                        reader.GetString("username"),
                        reader.GetString("email"),
                        reader.GetString("password"),
                        reader.GetBoolean("is_moderator"),
                        reader.IsDBNull("steam_id") ? "0" : reader.GetString("steam_id")
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

            cmd.Parameters.Add("@ID", MySqlDbType.Int32).Value = id;

            try
            {
                using var reader = cmd.ExecuteReader();

                if (!reader.Read()) return null;

                return new User(
                    reader.GetInt32("id"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    reader.GetInt32("age"),
                    reader.GetString("username"),
                    reader.GetString("email"),
                    reader.GetString("password"),
                    reader.GetBoolean("is_moderator"),
                    reader.IsDBNull("steam_id") ? "0" : reader.GetString("steam_id")
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

            cmd.Parameters.Add("@ID", MySqlDbType.Int32).Value = user.Id;
            cmd.Parameters.Add("@FIRST_NAME", MySqlDbType.VarChar).Value = user.Firstname;
            cmd.Parameters.Add("@LAST_NAME", MySqlDbType.VarChar).Value = user.Lastname;
            cmd.Parameters.Add("@AGE", MySqlDbType.Int32).Value = user.Age;
            cmd.Parameters.Add("@USERNAME", MySqlDbType.VarChar).Value = user.Username;
            cmd.Parameters.Add("@EMAIL", MySqlDbType.VarChar).Value = user.Gmail;
            cmd.Parameters.Add("@PASSWORD", MySqlDbType.VarChar).Value = user.Password;
            cmd.Parameters.Add("@IS_MODERATOR", MySqlDbType.Bit).Value = user.IsAdmin;
            cmd.Parameters.Add("@STEAM_ID", MySqlDbType.VarChar).Value = user.SteamId ?? "0";

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

            // delete comments
            var cmd1 = new MySqlCommand("DELETE FROM comment WHERE authorid=@ID", conn.GetInnerConn());
            cmd1.Parameters.Add("@ID", MySqlDbType.Int32).Value = user.Id;
            cmd1.ExecuteNonQuery();

            // delete applications
            var cmd2 = new MySqlCommand("DELETE FROM applications WHERE playerId=@ID", conn.GetInnerConn());
            cmd2.Parameters.Add("@ID", MySqlDbType.Int32).Value = user.Id;
            cmd2.ExecuteNonQuery();

            // delete user
            var cmd3 = new MySqlCommand("DELETE FROM user WHERE id=@ID", conn.GetInnerConn());
            cmd3.Parameters.Add("@ID", MySqlDbType.Int32).Value = user.Id;

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

            cmd.Parameters.Add("@USERNAME", MySqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@ID", MySqlDbType.Int32).Value = selfId;

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

            cmd.Parameters.Add("@term", MySqlDbType.VarChar).Value = term;

            var users = new List<User>();

            try
            {
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User(
                        reader.GetInt32("id"),
                        reader.GetString("first_name"),
                        reader.GetString("last_name"),
                        reader.GetInt32("age"),
                        reader.GetString("username"),
                        reader.GetString("email"),
                        reader.GetString("password"),
                        reader.GetBoolean("is_moderator"),
                        reader.IsDBNull("steam_id") ? "0" : reader.GetString("steam_id")
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
