using System;

namespace LogicLayer.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public DateTime Posted_on { get; set; }
        public string? ImagePath { get; set; }


        public User User { get; set; } = null!;

        public Post(int id, User user, string title, string content, DateTime posted_on)
        {
            Id = id;
            User = user;
            Title = title;
            Content = content;
            Posted_on = posted_on;
        }

        public Post() { }
    }
}
