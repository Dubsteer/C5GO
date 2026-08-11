using System;
using System.Collections.Generic;

namespace LogicLayer.Models
{
    public class Comment
    {
        public string Content { get; set; } = string.Empty;

        public int Id { get; set; }

        public DateTime Posted_on { get; set; }
        public int PostId { get; set; }

        public User User { get; set; } = null!;

        public List<CommentReply> Replies { get; set; } = [];

        public Comment(int id, User user, string content, DateTime posted_on, int postId)
        {
            Id = id;
            User = user;
            Content = content;
            Posted_on = posted_on;
            PostId = postId;
        }

        public Comment() { }
    }
}
