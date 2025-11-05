using LogicLayer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace LogicLayer.Models
{
    public class Post
    {
        [Display(Name = "Id")]
        [DisplayName("Id")]
        public int? Id { get; set; }

        [Display(Name = "AuthorId")]
        [DisplayName("AuthorId")]
        public User User { get; set; }

        [Display(Name = "Content")]
        [DisplayName("Conetnt")]
        public string Content { get; set; }

        [Display(Name = "Posted on")]
        [DisplayName("Posted on")]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy.")]
        public DateTime Posted_on { get; set; }

        public Post(int? id, User user, string content, DateTime posted_on)
        {
            Id = id;
            User = user;
            Content = content;
            Posted_on = posted_on;
        }
    }
}
