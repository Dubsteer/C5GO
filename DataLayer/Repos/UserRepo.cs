using LogicLayer;
using LogicLayer.Models;
using LogicLayer.IRepos;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace DataLayer.Repos
{
    public class UserRepo:IUserRepo
    {
        private readonly IConnection conn;

        public UserRepo(IConnection conn)
        {
            this.conn = conn;
        }
        public void CreateUser(User user)
        {
            var cmd = new MySqlCommand("INSERT INTO user(first_name, last_name, age, usernames, email, password, is_admin) VALUES (@FIRST_NAME, @LAST_NAME, @AGE, @USERNAMES, @EMAIL,@PASSWORD, @IS_ADMIN)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("FIRST_NAME", user.Firstname);
            cmd.Parameters.AddWithValue("LAST_NAME", user.Lastname);
            cmd.Parameters.AddWithValue("ADDRESS", user.Age);
            cmd.Parameters.AddWithValue("USERNAME", user.Username);
            cmd.Parameters.AddWithValue("EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("IS_ADMIN", user.IsAdmin);

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
            var cmd = new MySqlCommand(
              "select id, first_name, last_name, age, usernames, email, password, is_admin from user",
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
                                reader.GetString("usernames"),
                                reader.GetString("email"),
                                reader.GetString("password"),
                                reader.GetBoolean("is_admin")
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
            var cmd = new MySqlCommand("UPDATE user SET first_name = @FIRST_NAME, last_name = @LAST_NAME,age = @AGE, usernames = @USERNAMES, email = @EMAIL,password = @PASSWORD,  is_admin = @IS_ADMIN WHERE id = @ID ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", user.Id.Value);
            cmd.Parameters.AddWithValue("FIRST_NAME", user.Firstname);
            cmd.Parameters.AddWithValue("LAST_NAME", user.Lastname);
            cmd.Parameters.AddWithValue("ADDRESS", user.Age);
            cmd.Parameters.AddWithValue("USERNAME", user.Username);
            cmd.Parameters.AddWithValue("EMAIL", user.Gmail);
            cmd.Parameters.AddWithValue("PASSWORD", user.Password);
            cmd.Parameters.AddWithValue("IS_ADMIN", user.IsAdmin);

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
            var cmd = new MySqlCommand("delete from user where id = @ID",conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", user.Id.Value);

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
            var cmd = new MySqlCommand("select exists(select * from user where usernames = binary @USERNAMES and id !=@ID)", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("USERNAMES", username);
            cmd.Parameters.AddWithValue("ID", selfId);

            bool result;

            try
            {
                result = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return result;
        }
        public IList<User> SearchUser(string term)
        {
            var cmd = new MySqlCommand(
              "SELECT id, first_name, last_name, age, usernames, email, password, is_admin FROM users WHERE first_name LIKE '%term%' OR last_name LIKE '%term%'",
              conn.GetInnerConn());

            IList<User> users = new List<User>();

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
                                reader.GetString("usernames"),
                                reader.GetString("email"),
                                reader.GetString("password"),
                                reader.GetBoolean("is_admin")
                            ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                throw new Exception("No results found", ex);
            }

            return users;
        }
        public bool CheckIfEmailExists(string email, int selfId)
        {
            var cmd = new MySqlCommand(
                "select exists(select * from users where email = binary @EMAIL and id != @ID)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("EMAIL", email);
            cmd.Parameters.AddWithValue("ID", selfId);

            bool result;

            try
            {
                result = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return result;
        }
    }
}
