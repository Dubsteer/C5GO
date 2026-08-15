using System;

namespace LogicLayer.Models
{
    public class User
    {
        private string email = string.Empty;

        public int? Id { get; set; }

        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;

        public int Age { get; set; }
        public DateTime? Birthday { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Gmail
        {
            get => email;
            set => email = value?.Trim() ?? string.Empty;
        }

        public string Password { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public string? SteamId { get; set; }
        public bool ShowSteamProfile { get; set; }

        public bool EmailConfirmed { get; set; }
        public string? EmailToken { get; set; }
        public DateTime? TokenCreatedAt { get; set; }

        public User() { }

        public User(
            string firstname,
            string lastname,
            int age,
            string username,
            string gmail,
            string password,
            bool isAdmin
        )
        {
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;
            SteamId = null;
        }

        public User(
            int? id,
            string firstname,
            string lastname,
            int age,
            string username,
            string gmail,
            string password,
            bool isAdmin
        )
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;
            SteamId = null;
        }

        public User(
            int? id,
            string firstname,
            string lastname,
            int age,
            string username,
            string gmail,
            string password,
            bool isAdmin,
            string? steamId
        )
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            IsAdmin = isAdmin;
            SteamId = steamId;
        }

        public User(int id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"{Firstname} {Lastname} ({Username})";
        }
    }
}
