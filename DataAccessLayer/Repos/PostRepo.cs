using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataLayer.Repos
{
    public class PostRepo : IPostRepo
    {
        private readonly IConnection conn;

        public PostRepo(IConnection conn)
        {
            this.conn = conn;
        }

        private void EnsureOpen()
        {
            if (conn.GetInnerConn().State != ConnectionState.Open)
                conn.Open();
        }

        public void CreatePost(Post post)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"INSERT INTO post (authorid, title, content, posted_on, image_path)
          VALUES (@AUTHORID, @TITLE, @CONTENT, @POSTED_ON, @IMAGE_PATH)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@AUTHORID", post.User.Id);
            cmd.Parameters.AddWithValue("@TITLE", post.Title);
            cmd.Parameters.AddWithValue("@CONTENT", post.Content);
            cmd.Parameters.AddWithValue("@POSTED_ON", post.Posted_on);
            cmd.Parameters.AddWithValue("@IMAGE_PATH",
                string.IsNullOrWhiteSpace(post.ImagePath)
                    ? (object)DBNull.Value
                    : post.ImagePath);

            cmd.ExecuteNonQuery();
        }


        public List<Post> GetAllPosts()
        {
            EnsureOpen();

            var posts = new List<Post>();
            var cmd = new MySqlCommand("SELECT * FROM post", conn.GetInnerConn());

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var post = new Post(
                        reader.GetInt32("id"),
                        new User(reader.GetInt32("authorid")),
                        reader.GetString("title"),
                        reader.GetString("content"),
                        reader.GetDateTime("posted_on")
                    );

                    post.ImagePath = reader.IsDBNull("image_path")
                        ? null
                        : reader.GetString("image_path");

                    posts.Add(post);
                }
            }

            return posts;
        }


        public Post? GetPostById(int id)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                "SELECT * FROM post WHERE id = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    return null;

                var post = new Post(
                    reader.GetInt32("id"),
                    new User(reader.GetInt32("authorid")),
                    reader.GetString("title"),
                    reader.GetString("content"),
                    reader.GetDateTime("posted_on")
                );

                post.ImagePath = reader.IsDBNull("image_path")
                    ? null
                    : reader.GetString("image_path");

                return post;
            }
        }


        public void UpdatePost(Post post)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"UPDATE post SET
            title = @TITLE,
            content = @CONTENT,
            image_path = @IMAGE_PATH
          WHERE id = @ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", post.Id);
            cmd.Parameters.AddWithValue("@TITLE", post.Title);
            cmd.Parameters.AddWithValue("@CONTENT", post.Content);
            cmd.Parameters.AddWithValue(
                "@IMAGE_PATH",
                post.ImagePath ?? (object)DBNull.Value
            );

            cmd.ExecuteNonQuery();
        }


        public void DeletePost(Post post)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                "DELETE FROM post WHERE id=@ID",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@ID", post.Id);
            cmd.ExecuteNonQuery();
        }

        public bool CheckIfPostNameExists(string postTitle, int selfId)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"SELECT EXISTS(
                    SELECT 1 FROM post
                    WHERE title = BINARY @TITLE AND id != @ID
                  )",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@TITLE", postTitle);
            cmd.Parameters.AddWithValue("@ID", selfId);

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }
    }
}
