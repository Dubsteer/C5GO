using System;
using System.Collections.Generic;
using System.Diagnostics;
using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;

namespace DataLayer.Repos
{
    public class CommentRepo : ICommentRepo
    {
        private readonly IConnection conn;

        public CommentRepo(IConnection conn)
        {
            this.conn = conn;
        }

        // 🧩 Add Comment
        public void AddComment(Comment comment)
        {
            var cmd = new MySqlCommand(
                "INSERT INTO comment (authorid, content, posted_on, post_id) " +
                "VALUES (@authorid, @content, @posted_on, @post_id)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@authorid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.Content);
            cmd.Parameters.AddWithValue("@posted_on", comment.Posted_on);
            cmd.Parameters.AddWithValue("@post_id", comment.PostId);

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

        // 🧩 Get all comments for one post
        public List<Comment> GetAllCommentsByPostId(int id)
        {
            var cmd = new MySqlCommand(@"
                SELECT comment.*, user.*
                FROM comment
                JOIN user ON comment.authorid = user.id
                WHERE post_id = @post_id
                ORDER BY posted_on DESC;", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@post_id", id);

            var comments = new List<Comment>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int commentId = reader.GetInt32("id");
                        int authorId = reader.GetInt32("authorid");
                        string content = reader.GetString("content");
                        DateTime postedOn = reader.GetDateTime("posted_on");
                        int postId = reader.GetInt32("post_id");

                        string firstName = reader.GetString("first_name");
                        string lastName = reader.GetString("last_name");
                        int age = reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32("age");
                        string username = reader.GetString("username");
                        string email = reader.GetString("email");
                        string password = reader.GetString("password");
                        bool isModerator = reader.GetBoolean("is_moderator");
                        string steamId = reader.IsDBNull(reader.GetOrdinal("steam_id")) ? "0" : reader.GetString("steam_id");

                        User author = new User(authorId, firstName, lastName, age, username, email, password, isModerator, steamId);
                        Comment comment = new Comment(commentId, author, content, postedOn, postId);
                        comments.Add(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading comments. Please try again.", ex);
            }

            return comments;
        }

        // 🧩 Get one comment by user id
        public Comment GetCommentByUserId(int id)
        {
            var cmd = new MySqlCommand(@"
                SELECT comment.*, user.*
                FROM comment
                JOIN user ON comment.authorid = user.id
                WHERE comment.id = @comment_id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment_id", id);

            Comment comment = null;

            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string content = reader.GetString("content");
                        DateTime postedOn = reader.GetDateTime("posted_on");
                        int post_id = reader.GetInt32("post_id");
                        int authorId = reader.GetInt32("authorid");
                        string firstName = reader.GetString("first_name");
                        string lastName = reader.GetString("last_name");
                        int age = reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32("age");
                        string username = reader.GetString("username");
                        string email = reader.GetString("email");
                        string password = reader.GetString("password");
                        bool isModerator = reader.GetBoolean("is_moderator");
                        string steamId = reader.IsDBNull(reader.GetOrdinal("steam_id")) ? "0" : reader.GetString("steam_id");

                        User author = new User(authorId, firstName, lastName, age, username, email, password, isModerator, steamId);
                        comment = new Comment(id, author, content, postedOn, post_id);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading comment by user.", ex);
            }

            return comment;
        }

        // 🧩 Update comment
        public void UpdateComment(Comment comment)
        {
            var cmd = new MySqlCommand(
                "UPDATE comment SET authorid = @authorid, content = @content, " +
                "posted_on = @posted_on, post_id = @post_id WHERE id = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", comment.Id);
            cmd.Parameters.AddWithValue("@authorid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.Content);
            cmd.Parameters.AddWithValue("@posted_on", comment.Posted_on);
            cmd.Parameters.AddWithValue("@post_id", comment.PostId);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error updating comment.", ex);
            }
        }

        // 🧩 Delete comment
        public void DeleteComment(Comment comment)
        {
            var cmd = new MySqlCommand(
                "DELETE FROM comment WHERE id = @id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", comment.Id);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error deleting comment.", ex);
            }
        }

        // 🧩 Get comment by ID
        public Comment GetCommentById(int id)
        {
            var cmd = new MySqlCommand(
                "SELECT * FROM comment WHERE id = @id",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            Comment comment = null;

            try
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int commentId = reader.GetInt32("id");
                        int authorId = reader.GetInt32("authorid");
                        string content = reader.GetString("content");
                        DateTime postedOn = reader.GetDateTime("posted_on");
                        int postId = reader.GetInt32("post_id");

                        comment = new Comment(commentId, new User(authorId), content, postedOn, postId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading comment by ID.", ex);
            }

            return comment;
        }

        // 🧩 Get all comments
        public List<Comment> GetAllComments()
        {
            var cmd = new MySqlCommand(
                "SELECT id, authorid, content, posted_on, post_id FROM comment",
                conn.GetInnerConn());

            var comments = new List<Comment>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int commentId = reader.GetInt32("id");
                        int authorId = reader.GetInt32("authorid");
                        string content = reader.GetString("content");
                        DateTime postedOn = reader.GetDateTime("posted_on");
                        int postId = reader.GetInt32("post_id");

                        Comment comment = new Comment(commentId, new User(authorId), content, postedOn, postId);
                        comments.Add(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading all comments.", ex);
            }

            return comments;
        }

        // 🧩 Add reply to a comment
        public void AddReply(CommentReply reply)
        {
            var cmd = new MySqlCommand(
                "INSERT INTO commentreply (content, posted_on, comment_id) VALUES (@content, @posted_on, @comment_id)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@content", reply.Content);
            cmd.Parameters.AddWithValue("@posted_on", DateTime.Now);
            cmd.Parameters.AddWithValue("@comment_id", reply.CommentId);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error adding reply.", ex);
            }
        }

        // 🧩 Get all replies for a comment
        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            var cmd = new MySqlCommand(
                "SELECT * FROM commentreply WHERE comment_id = @comment_id ORDER BY posted_on DESC",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment_id", commentId);

            var replies = new List<CommentReply>();

            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int replyId = reader.GetInt32("id");
                        string content = reader.GetString("content");
                        DateTime postedOn = reader.GetDateTime("posted_on");
                        int _commentId = reader.GetInt32("comment_id");

                        replies.Add(new CommentReply(replyId, content, postedOn, _commentId));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error loading replies.", ex);
            }

            return replies;
        }

        // 🧩 Check if comment text already exists
        public bool CheckIfCommentExists(string comment)
        {
            var cmd = new MySqlCommand(
                "SELECT EXISTS (SELECT * FROM comment WHERE content = @comment)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment", comment);

            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Error checking comment existence.", ex);
            }
        }
    }
}
