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
            var cmd = new MySqlCommand("INSERT INTO comment (authorid, content, posted_on, post_id) VALUES (@authorid, @content, @posted_on, @post_id)", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@authorid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.CommentContent);
            cmd.Parameters.AddWithValue("@posted_on", comment.CommentPostedOn);
            cmd.Parameters.AddWithValue("@post_id", comment.commentPostId);
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

        public List<Comment> GetAllCommentsByPostId( int id)
        {
            {
                var cmd = new MySqlCommand("SELECT comment.*, user.* " +
                             "FROM comment " +
                             "JOIN user ON comment.authorid = user.id " +
                             "WHERE post_id = @post_id " +
                             "ORDER BY posted_on DESC", conn.GetInnerConn());
                var comments = new List<Comment>();
                try
                {

                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                    throw new Exception("Something unexpected has occurred. Please try again.", ex);
                }
                return comments;
            }
        }

        public Comment GetCommentByUserId(int id)//GetCommentByUserId
        {
            var cmd = new MySqlCommand("SELECT comment.*, user.* " +
                             "FROM comment " +
                             "JOIN user ON comment.authorid = user.id " +
                             "WHERE comment.id = @comment_id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment_id", id);

            Comment comment = null;

            using (MySqlDataReader reader = cmd.ExecuteReader())

                if (reader.Read())
                {
                    string content = reader.GetString("content");
                    DateTime postedOn = reader.GetDateTime("posted_on");
                    int post_id = reader.GetInt32("post_id");
                    int authorId = reader.GetInt32("authorid");
                    string firstName = reader.GetString("first_name");
                    string lastName = reader.GetString("last_name");
                    int age = reader.GetInt32("age");
                    string username = reader.GetString("usernames");
                    string email = reader.GetString("email");
                    string password = reader.GetString("password");
                    bool isAdmin = reader.GetBoolean("is_admin");
                    User author = new User(authorId, firstName, lastName, age, username, email, password, isAdmin);
                    Comment comment1 = new Comment(id, author, content, postedOn, post_id);
                    return comment;
                }
            return null;

        }

        public void UpdateComment(Comment comment)
        {
            var cmd = new MySqlCommand("UPDATE comment SET authorid = @AUTHORID, content = @CONTENT, posted_on = @POSTED_ON, post_id = @POST_ID  WHERE id = @ID ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", comment.CommentId.Value);
            cmd.Parameters.AddWithValue("authorid", comment.User);
            cmd.Parameters.AddWithValue("content", comment.CommentContent);
            cmd.Parameters.AddWithValue("posted_on", comment.CommentPostedOn);
            cmd.Parameters.AddWithValue("post_id", comment.commentPostId);

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
            var cmd = new MySqlCommand("delete from comment where id = @ID", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", comment.commentId.Value);

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
            var cmd = new MySqlCommand("select * from comment where id=@id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@comment_id", id);

            Comment comment = null;

            using (MySqlDataReader reader = cmd.ExecuteReader())

                if (reader.Read())
                {
                    while (reader.Read())
                        comment = new Comment(reader.GetInt32("id"),
                            new User(reader.GetInt32("authorid")),
                            reader.GetString("content"),
                            reader.GetDateTime("posted_on"),
                            reader.GetInt32("post_id")
                            );
                }

            return comment;
        }

        public List<Comment> GetAllComments()
        {
            var cmd = new MySqlCommand("SELECT id, authorid, content, posted_on, post_id FROM comments",conn.GetInnerConn());

            var comments = new List<Comment>();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    comments.Add(new Comment(
                        reader.GetInt32("id"),
                        new User(reader.GetInt32("authorid")),
                        reader.GetString("content"),
                        reader.GetDateTime("posted_on"),
                        reader.GetInt32("post_id")
                        ));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }

            return comments;
        }

        public void AddCommentReply(CommentReply reply)
        {
            var cmd = new MySqlCommand("INSERT INTO commentreply (content, posted_on, comment_id) VALUES (@content, @posted_on, @comment_id)", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@content", reply.Content);
            cmd.Parameters.AddWithValue("@posted_on", reply.CommentPostedOn);
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

        public void UpdateCommentReply(CommentReply commentReply)
        {
                throw new NotImplementedException();
        }

        public void DeleteCommentReply(CommentReply commentReply)
        {
            var cmd = new MySqlCommand("DELETE FROM commentreply WHERE id = @Id", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("ID", commentReply.CommentId);

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

        public List<CommentReply> GetAllReplies()
        {
            var cmd = new MySqlCommand("SELECT * FROM commentreply", conn.GetInnerConn());

            var commentReplies = new List<CommentReply>();
            try
            {

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                throw new Exception("Something unexpected has occurred. Please try again.", ex);
            }
            return commentReplies;
        }

        public bool CheckIfCommentExists(string comment)
        {
            var cmd = new MySqlCommand("select exists(select * from comment where comment = @COMMENT)", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("COMMENT", comment);

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