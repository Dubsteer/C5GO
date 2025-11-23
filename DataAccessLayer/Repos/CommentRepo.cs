using LogicLayer;
using LogicLayer.IRepos;
using LogicLayer.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DataLayer.Repos
{
    public class CommentRepo : ICommentRepo
    {
        private readonly IConnection conn;

        public CommentRepo(IConnection conn)
        {
            this.conn = conn;
        }

        // --------------------------------------------------
        // ADD COMMENT
        // --------------------------------------------------
        public void AddComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO comment (authorid, content, posted_on, post_id)
                VALUES (@a, @c, @p, @pid)",
                conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@a", comment.User.Id);
            cmd.Parameters.AddWithValue("@c", comment.Content);
            cmd.Parameters.AddWithValue("@p", comment.Posted_on);
            cmd.Parameters.AddWithValue("@pid", comment.PostId);

            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------
        // GET ALL COMMENTS FOR POST — FIXED VERSION
        // --------------------------------------------------
        public List<Comment> GetAllCommentsByPostId(int id)
        {
            var comments = new List<Comment>();

            var cmd = new MySqlCommand(@"
                SELECT 
                    c.id AS comment_id,
                    c.authorid AS comment_authorid,
                    c.content AS comment_content,
                    c.posted_on AS comment_posted_on,
                    c.post_id AS comment_post_id,

                    u.id AS user_id,
                    u.first_name,
                    u.last_name,
                    u.age,
                    u.username,
                    u.email,
                    u.password,
                    u.is_moderator,
                    u.steam_id

                FROM comment c
                JOIN user u ON c.authorid = u.id
                WHERE c.post_id = @pid
                ORDER BY c.posted_on DESC;
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@pid", id);

            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    var user = new User(
                        r.GetInt32("user_id"),
                        r.GetString("first_name"),
                        r.GetString("last_name"),
                        r.IsDBNull("age") ? 0 : r.GetInt32("age"),
                        r.GetString("username"),
                        r.GetString("email"),
                        r.GetString("password"),
                        r.GetBoolean("is_moderator"),
                        r.IsDBNull("steam_id") ? "0" : r.GetString("steam_id")
                    );

                    var comment = new Comment(
                        r.GetInt32("comment_id"),
                        user,
                        r.GetString("comment_content"),
                        r.GetDateTime("comment_posted_on"),
                        r.GetInt32("comment_post_id")
                    );

                    comments.Add(comment);
                }
            }

            return comments;
        }

        // --------------------------------------------------
        // GET COMMENT BY USER ID
        // --------------------------------------------------
        public Comment GetCommentByUserId(int id)
        {
            var cmd = new MySqlCommand(@"
                SELECT 
                    c.id AS comment_id,
                    c.authorid AS comment_authorid,
                    c.content AS comment_content,
                    c.posted_on AS comment_posted_on,
                    c.post_id AS comment_post_id,

                    u.id AS user_id,
                    u.first_name,
                    u.last_name,
                    u.age,
                    u.username,
                    u.email,
                    u.password,
                    u.is_moderator,
                    u.steam_id

                FROM comment c
                JOIN user u ON c.authorid = u.id
                WHERE c.id = @cid;
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@cid", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            var user = new User(
                r.GetInt32("user_id"),
                r.GetString("first_name"),
                r.GetString("last_name"),
                r.IsDBNull("age") ? 0 : r.GetInt32("age"),
                r.GetString("username"),
                r.GetString("email"),
                r.GetString("password"),
                r.GetBoolean("is_moderator"),
                r.IsDBNull("steam_id") ? "0" : r.GetString("steam_id")
            );

            return new Comment(
                r.GetInt32("comment_id"),
                user,
                r.GetString("comment_content"),
                r.GetDateTime("comment_posted_on"),
                r.GetInt32("comment_post_id")
            );
        }

        // --------------------------------------------------
        // UPDATE
        // --------------------------------------------------
        public void UpdateComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                UPDATE comment
                SET content = @c
                WHERE id = @id;
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@c", comment.Content);
            cmd.Parameters.AddWithValue("@id", comment.Id);

            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------
        // DELETE
        // --------------------------------------------------
        public void DeleteComment(Comment comment)
        {
            var cmd = new MySqlCommand(@"
                DELETE FROM comment WHERE id = @id;
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", comment.Id);

            cmd.ExecuteNonQuery();
        }

        // --------------------------------------------------
        // GET BY ID
        // --------------------------------------------------
        public Comment GetCommentById(int id)
        {
            var cmd = new MySqlCommand(@"
                SELECT 
                    c.id AS comment_id,
                    c.authorid AS comment_authorid,
                    c.content AS comment_content,
                    c.posted_on AS comment_posted_on,
                    c.post_id AS comment_post_id
                FROM comment c
                WHERE c.id = @id;
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;

            return new Comment(
                r.GetInt32("comment_id"),
                new User(r.GetInt32("comment_authorid")),
                r.GetString("comment_content"),
                r.GetDateTime("comment_posted_on"),
                r.GetInt32("comment_post_id")
            );
        }

        // --------------------------------------------------
        // GET ALL COMMENTS — MINIMAL
        // --------------------------------------------------
        public List<Comment> GetAllComments()
        {
            var list = new List<Comment>();

            var cmd = new MySqlCommand(@"
                SELECT id, authorid, content, posted_on, post_id
                FROM comment;
            ", conn.GetInnerConn());

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

        // --------------------------------------------------
        // REPLIES
        // --------------------------------------------------
        public void AddReply(CommentReply reply)
        {
            var cmd = new MySqlCommand(@"
                INSERT INTO commentreply (content, posted_on, comment_id)
                VALUES (@c, @p, @cid);
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@c", reply.Content);
            cmd.Parameters.AddWithValue("@p", reply.PostedOn);
            cmd.Parameters.AddWithValue("@cid", reply.CommentId);

            cmd.ExecuteNonQuery();
        }

        public List<CommentReply> GetAllRepliesByCommentId(int commentId)
        {
            var list = new List<CommentReply>();

            var cmd = new MySqlCommand(@"
                SELECT id, content, posted_on, comment_id
                FROM commentreply
                WHERE comment_id = @cid
                ORDER BY posted_on DESC;
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@cid", commentId);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new CommentReply(
                    r.GetInt32("id"),
                    r.GetString("content"),
                    r.GetDateTime("posted_on"),
                    r.GetInt32("comment_id")
                ));
            }

            return list;
        }

        public bool CheckIfCommentExists(string comment)
        {
            var cmd = new MySqlCommand(@"
                SELECT EXISTS(SELECT 1 FROM comment WHERE content = @c);
            ", conn.GetInnerConn());

            cmd.Parameters.AddWithValue("@c", comment);

            return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
        }
    }
}
