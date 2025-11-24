using System;

namespace LogicLayer.Models
{
    public class CommentReply
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedOn { get; set; }
        public int CommentId { get; set; }

        // Username prikaz – koristi se na UI-ju
        public string Username { get; set; }

        // Pravi korisnik koji je napisao reply
        public User User { get; set; }

        public CommentReply() { }

        // Konstruktor za kreiranje reply-a sa pravim korisnikom
        public CommentReply(int id, string content, DateTime postedOn, int commentId, User user)
        {
            Id = id;
            Content = content;
            PostedOn = postedOn;
            CommentId = commentId;
            User = user;
            Username = user?.Username; // automatski upisujemo username
        }

        // Konstruktor za SELECT iz baze gde vraćamo samo username
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
