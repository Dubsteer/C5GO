using System;

namespace LogicLayer.Models
{
    public class CommentReply
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime PostedOn { get; set; }
        public int CommentId { get; set; }

        public string Username { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public CommentReply() { }

        public CommentReply(int id, string content, DateTime postedOn, int commentId, User user)
        {
            Id = id;
            Content = content;
            PostedOn = postedOn;
            CommentId = commentId;
            User = user;
            Username = user.Username;
        }

        public CommentReply(int id, string content, DateTime postedOn, int commentId, string username)
        {
            Id = id;
            Content = content;
            PostedOn = postedOn;
            CommentId = commentId;
            Username = username;
        }
    }
}
