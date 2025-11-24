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

        // --------------------------------------------------------------------
        // ADD COMMENT
        // --------------------------------------------------------------------
        public void AddComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO comment (authorid, content, posted_on, post_id)
                VALUES (@aid, @content, @posted, @postid)
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@aid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.Content);
            cmd.Parameters.AddWithValue("@posted", comment.Posted_on);
            cmd.Parameters.AddWithValue("@postid", comment.PostId);

            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------
        // GET ALL COMMENTS FOR ONE POST
        // --------------------------------------------------------------------
        public List<Comment> GetAllCommentsByPostId(int id)
        {
            var cmd = new MySqlCommand(@"
                SELECT c.id, c.authorid, c.content, c.posted_on, c.post_id,
                       u.first_name, u.last_name, u.age, u.username, u.email,
                       u.password, u.is_moderator, u.steam_id
                FROM comment c
                JOIN user u ON u.id = c.authorid
                WHERE c.post_id = @pid
                ORDER BY c.posted_on DESC
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@pid", id);

            var comments = new List<Comment>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var user = new User(
                    reader.GetInt32("authorid"),
                    reader.GetString("first_name"),
                    reader.GetString("last_name"),
                    reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32("age"),
                    reader.GetString("username"),
                    reader.GetString("email"),
                    reader.GetString("password"),
                    reader.GetBoolean("is_moderator"),
                    reader.GetString("steam_id")
                );

                comments.Add(new Comment(
                    reader.GetInt32("id"),
                    user,
                    reader.GetString("content"),
                    reader.GetDateTime("posted_on"),
                    reader.GetInt32("post_id")
                ));
            }

            return comments;
        }

        // --------------------------------------------------------------------
        // LEGACY: GET ONE COMMENT BY USER ID (required by old code)
        // Returns the latest comment from a user
        // --------------------------------------------------------------------
        public Comment GetCommentByUserId(int userId)
        {
            var cmd = new MySqlCommand(@"
                SELECT c.id, c.authorid, c.content, c.posted_on, c.post_id,
                       u.first_name, u.last_name, u.age, u.username, u.email,
                       u.password, u.is_moderator, u.steam_id
                FROM comment c
                JOIN user u ON u.id = c.authorid
                WHERE c.authorid = @uid
                ORDER BY c.posted_on DESC
                LIMIT 1
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@uid", userId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var user = new User(
                reader.GetInt32("authorid"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.IsDBNull(reader.GetOrdinal("age")) ? 0 : reader.GetInt32("age"),
                reader.GetString("username"),
                reader.GetString("email"),
                reader.GetString("password"),
                reader.GetBoolean("is_moderator"),
                reader.GetString("steam_id")
            );

            return new Comment(
                reader.GetInt32("id"),
                user,
                reader.GetString("content"),
                reader.GetDateTime("posted_on"),
                reader.GetInt32("post_id")
            );
        }

        // --------------------------------------------------------------------
        // UPDATE
        // --------------------------------------------------------------------
        public void UpdateComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                UPDATE comment
                SET authorid=@aid, content=@content, posted_on=@posted, post_id=@pid
                WHERE id=@id
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", comment.Id);
            cmd.Parameters.AddWithValue("@aid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.Content);
            cmd.Parameters.AddWithValue("@posted", comment.Posted_on);
            cmd.Parameters.AddWithValue("@pid", comment.PostId);

            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------
        // DELETE
        // --------------------------------------------------------------------
        public void DeleteComment(Comment comment)
        {
            var cmd = new MySqlCommand(
                "DELETE FROM comment WHERE id=@id",
                conn.GetInnerConn()
            );

            cmd.Parameters.AddWithValue("@id", comment.Id);
            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------
        // GET COMMENT BY ID
        // --------------------------------------------------------------------
        public Comment GetCommentById(int id)
        {
            var cmd = new MySqlCommand(@"
                SELECT * FROM comment WHERE id=@id
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new Comment(
                reader.GetInt32("id"),
                new User(reader.GetInt32("authorid")),
                reader.GetString("content"),
                reader.GetDateTime("posted_on"),
                reader.GetInt32("post_id")
            );
        }

        // --------------------------------------------------------------------
        // GET ALL COMMENTS
        // --------------------------------------------------------------------
        public List<Comment> GetAllComments()
        {
            var cmd = new MySqlCommand(@"
                SELECT id, authorid, content, posted_on, post_id
                FROM comment
            ", conn.GetInnerConn());

            var comments = new List<Comment>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comments.Add(new Comment(
                    reader.GetInt32("id"),
                    new User(reader.GetInt32("authorid")),
                    reader.GetString("content"),
                    reader.GetDateTime("posted_on"),
                    reader.GetInt32("post_id")
                ));
            }

            return comments;
        }

        // --------------------------------------------------------------------
        // ADD REPLY
        // --------------------------------------------------------------------
        public void AddReply(CommentReply reply)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO commentreply (content, posted_on, comment_id)
                VALUES (@content, @posted, @cid)
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@content", reply.Content);
            cmd.Parameters.AddWithValue("@posted", DateTime.Now);
            cmd.Parameters.AddWithValue("@cid", reply.CommentId);

            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------------------------
        // GET REPLIES
        // --------------------------------------------------------------------
        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            var cmd = new MySqlCommand(@"
                SELECT * FROM commentreply
                WHERE comment_id=@cid
                ORDER BY posted_on ASC
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@cid", commentId);

            var replies = new List<CommentReply>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                replies.Add(new CommentReply(
                    reader.GetInt32("id"),
                    reader.GetString("content"),
                    reader.GetDateTime("posted_on"),
                    reader.GetInt32("comment_id")
                ));
            }

            return replies;
        }

        // --------------------------------------------------------------------
        // CHECK IF COMMENT TEXT EXISTS
        // --------------------------------------------------------------------
        public bool CheckIfCommentExists(string text)
        {
            var cmd = new MySqlCommand(@"
                SELECT EXISTS(
                    SELECT * FROM comment WHERE content=@text
                )
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@text", text);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }
    }
}
