using System;

namespace LogicLayer.Models
{
    public class CommentReply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
        public int CommentId { get; set; }

        // prikaz username autora (nije foreign key—puni se iz repo-a)
        public string AuthorUsername { get; set; }

        public CommentReply(int id, string content, DateTime postedOn, int commentId)
        {
            Id = id;
            Content = content;
            PostedOn = postedOn;
            CommentId = commentId;
        }
    }
}
