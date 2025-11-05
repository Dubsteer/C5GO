using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataLayer.Repos
{
    public class PostRepo : IPostRepo
    {
        private readonly IConnection connection;

        public PostRepo(IConnection connection)
        {
            this.connection = connection;
        }

        public void CreatePost(Post post)
        {
            var cmd = new MySqlCommand(
                "insert into post(authorid, content, posted_on) values (@AUTHORID, @CONTENT, @POSTED_ON)", connection.GetInnerConn());

            cmd.Parameters.AddWithValue("@authorid", post.User.Id);
            cmd.Parameters.AddWithValue("@content", post.Content);
            cmd.Parameters.AddWithValue("@posted_on", post.Posted_on);
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

        public List<Post> GetAllPosts()
        {
            var cmd = new MySqlCommand("select * from post ", connection.GetInnerConn());

            var posts = new List<Post>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add
                            (new Post(
                            reader.GetInt32("id"),
                            new User(reader.GetInt32("authorid")),
                            reader.GetString("content"),
                            reader.GetDateTime("posted_on")
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return posts;
        }

        public void UpdatePost(Post post)
        {
            var cmd = new MySqlCommand("update post set  authorid=@AUTHORID, content=@CONTENT, posted_on=@POSTED_ON where id=@ID", connection.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", post.Id);
            cmd.Parameters.AddWithValue("@AUTHORID", post.User.Id);
            cmd.Parameters.AddWithValue("@CONTENT", post.Content);
            cmd.Parameters.AddWithValue("@POSTED_ON", post.Posted_on);

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

        public void DeletePost(Post post)
        {
            var cmd = new MySqlCommand(
                "delete from post where id = @ID",
                 connection.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", post.Id);

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

        public bool CheckIfPostNameExists(string postContent, int selfId)
        {
            var cmd = new MySqlCommand(
               "select exists(select * from post where content = binary @CONTENT and id != @ID)",
               connection.GetInnerConn());

            cmd.Parameters.AddWithValue("CONTENT", postContent);
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
