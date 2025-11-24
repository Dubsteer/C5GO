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

        // ADD COMMENT
        public void AddComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO comment (authorid, content, posted_on, post_id)
                VALUES (@aid, @content, @posted, @pid)
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@aid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.Content);
            cmd.Parameters.AddWithValue("@posted", comment.Posted_on);
            cmd.Parameters.AddWithValue("@pid", comment.PostId);

            cmd.ExecuteNonQuery();
        }

        // GET ALL COMMENTS FOR POST
        public List<Comment> GetAllCommentsByPostId(int postId)
        {
            var cmd = new MySqlCommand(@"
                SELECT 
                    c.id, c.authorid, c.content, c.posted_on, c.post_id,
                    u.first_name, u.last_name, u.age, u.username, u.email,
                    u.password, u.is_moderator, u.steam_id
                FROM comment c
                JOIN user u ON u.id = c.authorid
                WHERE c.post_id = @pid
                ORDER BY c.posted_on DESC
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@pid", postId);

            List<Comment> comments = new();

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                var user = new User(
                    r.GetInt32("authorid"),
                    r.GetString("first_name"),
                    r.GetString("last_name"),
                    r.IsDBNull(r.GetOrdinal("age")) ? 0 : r.GetInt32("age"),
                    r.GetString("username"),
                    r.GetString("email"),
                    r.GetString("password"),
                    r.GetBoolean("is_moderator"),
                    r.GetString("steam_id")
                );

                comments.Add(new Comment(
                    r.GetInt32("id"),
                    user,
                    r.GetString("content"),
                    r.GetDateTime("posted_on"),
                    r.GetInt32("post_id")
                ));
            }

            return comments;
        }

        public Comment GetCommentByUserId(int userId)
        {
            var cmd = new MySqlCommand(@"
                SELECT * FROM comment 
                WHERE authorid = @uid
                ORDER BY posted_on DESC
                LIMIT 1
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@uid", userId);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new Comment(
                r.GetInt32("id"),
                new User(r.GetInt32("authorid")),
                r.GetString("content"),
                r.GetDateTime("posted_on"),
                r.GetInt32("post_id")
            );
        }

        public Comment GetCommentById(int id)
        {
            var cmd = new MySqlCommand("SELECT * FROM comment WHERE id=@id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new Comment(
                r.GetInt32("id"),
                new User(r.GetInt32("authorid")),
                r.GetString("content"),
                r.GetDateTime("posted_on"),
                r.GetInt32("post_id")
            );
        }

        public void UpdateComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                UPDATE comment SET
                authorid=@aid, content=@content, posted_on=@posted, post_id=@pid
                WHERE id=@id
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", comment.Id);
            cmd.Parameters.AddWithValue("@aid", comment.User.Id);
            cmd.Parameters.AddWithValue("@content", comment.Content);
            cmd.Parameters.AddWithValue("@posted", comment.Posted_on);
            cmd.Parameters.AddWithValue("@pid", comment.PostId);

            cmd.ExecuteNonQuery();
        }

        public void DeleteComment(Comment comment)
        {
            var cmd = new MySqlCommand("DELETE FROM comment WHERE id=@id", conn.GetInnerConn());
            cmd.Parameters.AddWithValue("@id", comment.Id);
            cmd.ExecuteNonQuery();
        }

        public List<Comment> GetAllComments()
        {
            var cmd = new MySqlCommand("SELECT * FROM comment", conn.GetInnerConn());
            List<Comment> list = new();

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Comment(
                    r.GetInt32("id"),
                    new User(r.GetInt32("authorid")),
                    r.GetString("content"),
                    r.GetDateTime("posted_on"),
                    r.GetInt32("post_id")
                ));
            }

            return list;
        }

        // ADD REPLY
        public void AddReply(CommentReply reply)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO commentreply (content, posted_on, comment_id, user_id)
                VALUES (@content, @posted, @cid, @uid)
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@content", reply.Content);
            cmd.Parameters.AddWithValue("@posted", reply.PostedOn);
            cmd.Parameters.AddWithValue("@cid", reply.CommentId);
            cmd.Parameters.AddWithValue("@uid", reply.User.Id);

            cmd.ExecuteNonQuery();
        }

        // GET ALL REPLIES FOR COMMENT
        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            var cmd = new MySqlCommand(@"
                SELECT 
                    cr.id,
                    cr.content,
                    cr.posted_on,
                    cr.comment_id,
                    u.username
                FROM commentreply cr
                JOIN user u ON u.id = cr.user_id
                WHERE cr.comment_id=@cid
                ORDER BY cr.posted_on ASC
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@cid", commentId);

            List<CommentReply> list = new();

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new CommentReply(
                    r.GetInt32("id"),
                    r.GetString("content"),
                    r.GetDateTime("posted_on"),
                    r.GetInt32("comment_id"),
                    r.GetString("username")
                ));
            }

            return list;
        }

        public bool CheckIfCommentExists(string text)
        {
            var cmd = new MySqlCommand(
                "SELECT EXISTS(SELECT * FROM comment WHERE content = @txt)",
                conn.GetInnerConn()
            );

            cmd.Parameters.AddWithValue("@txt", text);
            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }
    }
}
