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
            if (conn.Connection.State != ConnectionState.Open)
                conn.Open();
        }

        public void CreatePost(Post post)
        {
            EnsureOpen();

            var cmd = new MySqlCommand(
                @"INSERT INTO post (authorid, title, content, posted_on, image_path)
          VALUES (@AUTHORID, @TITLE, @CONTENT, @POSTED_ON, @IMAGE_PATH)",
                conn.Connection);

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
            var cmd = new MySqlCommand(
                @"SELECT p.id, p.authorid, p.title, p.content, p.posted_on, p.image_path,
                         u.username AS author_username
                  FROM post p
                  LEFT JOIN user u ON u.id = p.authorid
                  ORDER BY p.posted_on DESC, p.id DESC",
                conn.Connection);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var author = new User(reader.GetInt32("authorid"))
                    {
                        Username = reader.IsDBNull("author_username")
                            ? "Unknown user"
                            : reader.GetString("author_username")
                    };

                    var post = new Post(
                        reader.GetInt32("id"),
                        author,
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
                @"SELECT p.id, p.authorid, p.title, p.content, p.posted_on, p.image_path,
                         u.username AS author_username
                  FROM post p
                  LEFT JOIN user u ON u.id = p.authorid
                  WHERE p.id = @id",
                conn.Connection);

            cmd.Parameters.AddWithValue("@id", id);

            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    return null;

                var author = new User(reader.GetInt32("authorid"))
                {
                    Username = reader.IsDBNull("author_username")
                        ? "Unknown user"
                        : reader.GetString("author_username")
                };

                var post = new Post(
                    reader.GetInt32("id"),
                    author,
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
                conn.Connection);

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
                conn.Connection);

            cmd.Parameters.AddWithValue("@ID", post.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
