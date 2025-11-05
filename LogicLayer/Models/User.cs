 using LogicLayer.Managers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LogicLayer.Models
{
    public class User
    {
        private string email;

        public int? Id { get; set; }

        [Display(Name = "First name")]
        [DisplayName("First name")]
        public string Firstname { get; set; }

        [Display(Name = "Last name")]
        [DisplayName("Last name")]
        public string Lastname { get; set; }

        [Display(Name = "Age")]
        [DisplayName("Age")]
        public int Age { get; set; }

        [Display(Name = "Username")]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Display(Name = "Gmail")]
        [DisplayName("Gmail")]
        public string Gmail { get; set; }

        [Browsable(false)]
        public string Password { get; set; }

        [Display(Name = "Is admin")]
        [DisplayName("Is admin")]
        public bool IsAdmin { get; set; }
        [Browsable(false)]
        public int CommentAuthorId { get; }

        [Browsable(false)]
        public IReadOnlyList<Match> Matches { get; private set; }

        public User(string firstname, string lastname, int age, string username, string gmail, string password, bool isAdmin)
        {
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;
        }

        public User(int? id, string firstname, string lastname, int age, string username, string gmail, string password, bool isAdmin)
        {

            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;

        }

        public User(int? id, string firstname, string lastname, int age, string username, string gmail, string password, bool isAdmin, List<Match> matches)
        {

            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;

            Matches = new List<Match>();
        }

        public User(int commentAuthorId)
        {
            CommentAuthorId = commentAuthorId;
        }

        public User()
        {

        }

        public override string ToString()
        {
            return CommentAuthorId.ToString();
        }

        public List<Match>GetPastMatches(MatchManager matchManager) 
        {
            Matches = matchManager.GetFullMatches().Where( m => m.User1.Username == this.Username).ToList();

            return Matches.ToList();
        }
    }
}
