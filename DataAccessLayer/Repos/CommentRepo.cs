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

        public void AddComment(Comment comment)
        {
            var cmd = new MySqlCommand("insert into comment (authorid, content, posted_on, post_id) values (@authorid, @content, @posted_on, @post_id)", conn.GetInnerConn());

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

        public List<Comment> GetAllCommentsByPostId(int id)
        {
            var cmd = new MySqlCommand("select comment.*, user.* " +
                                         "from comment " +
                                         "join user ON comment.authorid = user.id " +
                                         "where post_id = @post_id " +
                                         "order by posted_on DESC", conn.GetInnerConn());
            var comments = new List<Comment>();
            try
            {
                cmd.Parameters.AddWithValue("@post_id", id);

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
                        int age = reader.GetInt32("age");
                        string username = reader.GetString("username");
                        string email = reader.GetString("email");
                        string password = reader.GetString("password");
                        bool isAdmin = reader.GetBoolean("is_admin");

                        User author = new User(authorId, firstName, lastName, age, username, email, password, isAdmin);
                        Comment comment = new Comment(commentId, author, content, postedOn, postId);
                        comments.Add(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
            return comments;
        }

        public Comment GetCommentByUserId(int id)
        {
            var cmd = new MySqlCommand("select comment.*, user.* " +
                                         "from comment " +
                                         "join user on comment.authorid = user.id " +
                                         "where comment.id = @comment_id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment_id", id);

            Comment comment = null;

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
                    int age = reader.GetInt32("age");
                    string username = reader.GetString("username");
                    string email = reader.GetString("email");
                    string password = reader.GetString("password");
                    bool isAdmin = reader.GetBoolean("is_admin");
                    User author = new User(authorId, firstName, lastName, age, username, email, password, isAdmin);
                    comment = new Comment(id, author, content, postedOn, post_id);
                }
            }

            return comment;
        }

        public void UpdateComment(Comment comment)
        {
            var cmd = new MySqlCommand("update comment set authorid = @authorid, content = @content, posted_on = @posted_on, post_id = @post_id  where id = @id ", conn.GetInnerConn());

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
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public void DeleteComment(Comment comment)
        {
            var cmd = new MySqlCommand("delete FROM comment where id = @id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", comment.Id);

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

        public Comment GetCommentById(int id)
        {
            var cmd = new MySqlCommand("select * from comment where id = @id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            Comment comment = null;

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

            return comment;
        }

        public List<Comment> GetAllComments()
        {
            var cmd = new MySqlCommand("select id, authorid, content, posted_on, post_id from comment", conn.GetInnerConn());

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
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return comments;
        }

        public void AddReply(CommentReply reply)
        {
            var cmd = new MySqlCommand("INSERT INTO commentreply (content, posted_on, comment_id) VALUES (@content, @posted_on, @comment_id)", conn.GetInnerConn());

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
                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
        }

        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            var cmd = new MySqlCommand("SELECT * FROM commentreply WHERE comment_id = @comment_id ORDER BY posted_on DESC", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment_id", commentId);

            var replies = new List<CommentReply>();

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

            return replies;
        }

        public bool CheckIfCommentExists(string comment)
        {
            var cmd = new MySqlCommand("select exists (select * from comment where comment = @comment)", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment", comment);

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