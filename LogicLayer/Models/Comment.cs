using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.Models
{
    public class Comment
    {
        public int? Id { get; set; }
        public string Content { get; set; }
        public DateTime Posted_on { get; set; }
        public List<CommentReply> Replies { get; set; }
        public User User { get; set; }
        public int PostId { get; set; }

        public Comment(int? id, User? user, string content, DateTime posted_on, int postId)
        {
            Id = id;
            User = user;
            Content = content;
            Posted_on = posted_on;
            PostId = postId;
        }

        public Comment(int? id, User? user, string content, DateTime posted_on, int postId, List<CommentReply> replies)
        {
            Id = id;
            User = user;
            Content = content;
            Posted_on = posted_on;
            Replies = replies;
            PostId = postId;
        }
        public Comment() { }
    }
}