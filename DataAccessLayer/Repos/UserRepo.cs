using LogicLayer;
using LogicLayer.Models;
using LogicLayer.IRepos;
using MySql.Data.MySqlClient;
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
                "INSERT INTO user (first_name, last_name, age, username, email, password, is_moderator, steam_id) " +
                "VALUES (@FIRST_NAME, @LAST_NAME, @AGE, @USERNAME, @EMAIL, @PASSWORD, @IS_MODERATOR, @STEAM_ID)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@FIRST_NAME", user.Firstname);
            cmd.Parameters.AddWithValue("@LAST_NAME", user.Lastname);
            cmd.Parameters.AddWithValue("@AGE", user.Age);
            cmd.Parameters.AddWithValue("@USERNAME", user.Username);
            cmd.Parameters.AddWithValue("@EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("@PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("@IS_MODERATOR", user.IsAdmin);
            cmd.Parameters.AddWithValue("@STEAM_ID", user.SteamId ?? "0");

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public List<User> GetAllUsers()
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
              "SELECT id, first_name, last_name, age, username, email, password, is_moderator FROM user",
              conn.GetInnerConn());

            var users = new List<User>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(
                            new User(
                                reader.GetInt32("id"),
                                reader.GetString("first_name"),
                                reader.GetString("last_name"),
                                reader.GetInt32("age"),
                                reader.GetString("username"),
                                reader.GetString("email"),
                                reader.GetString("password"),
                                reader.GetBoolean("is_moderator")
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return users;
        }

        public void UpdateUser(User user)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "UPDATE user SET first_name = @FIRST_NAME, last_name = @LAST_NAME, age = @AGE, " +
                "username = @USERNAME, email = @EMAIL, password = @PASSWORD, is_moderator = @IS_MODERATOR " +
                "WHERE id = @ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", user.Id.Value);
            cmd.Parameters.AddWithValue("FIRST_NAME", user.Firstname);
            cmd.Parameters.AddWithValue("LAST_NAME", user.Lastname);
            cmd.Parameters.AddWithValue("AGE", user.Age);
            cmd.Parameters.AddWithValue("USERNAME", user.Username);
            cmd.Parameters.AddWithValue("EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("IS_MODERATOR", user.IsAdmin);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public void DeleteUser(User user)
        {
            EnsureConnection();

            // 1) Delete comments
            var deleteComments = new MySqlCommand(
                "DELETE FROM comment WHERE authorid=@ID",
                conn.GetInnerConn());

            deleteComments.Parameters.AddWithValue("@ID", user.Id.Value);
            deleteComments.ExecuteNonQuery();

            // 2) Delete tournament applications
            var deleteFromApplications = new MySqlCommand(
                "DELETE FROM applications WHERE playerId=@ID",
                conn.GetInnerConn());

            deleteFromApplications.Parameters.AddWithValue("@ID", user.Id.Value);
            deleteFromApplications.ExecuteNonQuery();

            // 3) Delete user
            var cmd = new MySqlCommand(
                "DELETE FROM user WHERE id = @ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", user.Id.Value);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public bool CheckIfUsernameExists(string username, int selfId)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT * FROM user WHERE username = BINARY @USERNAME AND id != @ID)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("USERNAME", username);
            cmd.Parameters.AddWithValue("ID", selfId);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public List<User> SearchUser(string term)
        {
            EnsureConnection();

            var cmd = new MySqlCommand(
                "SELECT id, first_name, last_name, age, username, email, password, is_moderator " +
                "FROM user WHERE username LIKE CONCAT('%', @term, '%')",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@term", term);

            var users = new List<User>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(
                            new User(
                                reader.GetInt32("id"),
                                reader.GetString("first_name"),
                                reader.GetString("last_name"),
                                reader.GetInt32("age"),
                                reader.GetString("username"),
                                reader.GetString("email"),
                                reader.GetString("password"),
                                reader.GetBoolean("is_moderator")
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("No results found?", ex);
            }

            return users;
        }
    }
}
