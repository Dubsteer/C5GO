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

        // 🧩 Database has both 'birthday' (DATE) and 'age' (INT)
        // We'll keep Age as main property for simplicity
        [Display(Name = "Age")]
        [DisplayName("Age")]
        public int Age { get; set; }

        [Display(Name = "Birthday")]
        [DisplayName("Birthday")]
        public DateTime? Birthday { get; set; }   // Optional — if you later read from 'birthday'

        [Display(Name = "Username")]
        [DisplayName("Username")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        [DisplayName("Email")]
        public string Gmail
        {
            get => email;
            set => email = value?.Trim();
        }

        [Browsable(false)]
        public string Password { get; set; }

        // ⚙️ In DB it's 'is_moderator' but your code uses IsAdmin — fine
        [Display(Name = "Is moderator")]
        [DisplayName("Is moderator")]
        public bool IsAdmin { get; set; }

        // ✅ Added missing SteamId to match DB
        [Display(Name = "Steam ID")]
        [DisplayName("Steam ID")]
        public string SteamId { get; set; } = "0";

        [Browsable(false)]
        public int CommentAuthorId { get; }

        [Browsable(false)]
        public IReadOnlyList<Match> Matches { get; private set; }

        // 🧩 Constructors
        public User() { }

        public User(string firstname, string lastname, int age, string username, string gmail, string password, bool isAdmin)
        {
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;
            SteamId = "0";
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
            SteamId = "0";
        }

        public User(int? id, string firstname, string lastname, int age, string username, string gmail, string password, bool isAdmin, string steamId)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;
            SteamId = steamId ?? "0";
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
            SteamId = "0";
            Matches = matches ?? new List<Match>();
        }

        public User(int commentAuthorId)
        {
            CommentAuthorId = commentAuthorId;
        }

        public override string ToString()
        {
            return $"{Firstname} {Lastname} ({Username})";
        }

        public List<Match> GetPastMatches(MatchManager matchManager)
        {
            Matches = matchManager.GetFullMatches()
                .Where(m => m.User1.Username == this.Username)
                .ToList();

            return Matches.ToList();
        }
    }
}
