using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

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
                "INSERT INTO post (authorid, title, content, posted_on) VALUES (@AUTHORID, @TITLE, @CONTENT, @POSTED_ON)",
                connection.GetInnerConn()
            );

            cmd.Parameters.AddWithValue("@AUTHORID", post.User.Id);
            cmd.Parameters.AddWithValue("@TITLE", post.Title);
            cmd.Parameters.AddWithValue("@CONTENT", post.Content);
            cmd.Parameters.AddWithValue("@POSTED_ON", post.Posted_on);
            cmd.ExecuteNonQuery();
        }

        public List<Post> GetAllPosts()
        {
            var cmd = new MySqlCommand("SELECT * FROM post", connection.GetInnerConn());
            var posts = new List<Post>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                posts.Add(new Post(
                    reader.GetInt32("id"),
                    new User(reader.GetInt32("authorid")),
                    reader.GetString("title"),
                    reader.GetString("content"),
                    reader.GetDateTime("posted_on")
                ));
            }

            return posts;
        }

        public void UpdatePost(Post post)
        {
            var cmd = new MySqlCommand(
                "UPDATE post SET authorid=@AUTHORID, title=@TITLE, content=@CONTENT, posted_on=@POSTED_ON WHERE id=@ID",
                connection.GetInnerConn()
            );

            cmd.Parameters.AddWithValue("@ID", post.Id);
            cmd.Parameters.AddWithValue("@AUTHORID", post.User.Id);
            cmd.Parameters.AddWithValue("@TITLE", post.Title);
            cmd.Parameters.AddWithValue("@CONTENT", post.Content);
            cmd.Parameters.AddWithValue("@POSTED_ON", post.Posted_on);
            cmd.ExecuteNonQuery();
        }

        public void DeletePost(Post post)
        {
            var cmd = new MySqlCommand("DELETE FROM post WHERE id=@ID", connection.GetInnerConn());
            cmd.Parameters.AddWithValue("@ID", post.Id);
            cmd.ExecuteNonQuery();
        }

        public bool CheckIfPostNameExists(string postTitle, int selfId)
        {
            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT * FROM post WHERE title = BINARY @TITLE AND id != @ID)",
                connection.GetInnerConn()
            );

            cmd.Parameters.AddWithValue("@TITLE", postTitle);
            cmd.Parameters.AddWithValue("@ID", selfId);

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }

        public Post GetPostById(int id)
        {
            var cmd = new MySqlCommand("SELECT * FROM post WHERE id=@id", connection.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Post(
                reader.GetInt32("id"),
                new User(reader.GetInt32("authorid")),
                reader.GetString("title"),
                reader.GetString("content"),
                reader.GetDateTime("posted_on")
            );
        }
    }
}
