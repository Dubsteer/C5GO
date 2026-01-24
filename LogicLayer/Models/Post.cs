using System;

namespace LogicLayer.Models
{
    public class Post
    {
        public int Id { get; set; }

        // Novi Title — dodajemo ga
        public string Title { get; set; }

        public string Content { get; set; }
        public DateTime Posted_on { get; set; }
        public string? ImagePath { get; set; }


        public User User { get; set; }

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
