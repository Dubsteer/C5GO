using System;
using System.Collections.Generic;

namespace LogicLayer.Models
{
    public class User
    {
        private string email;

        public int? Id { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public int Age { get; set; }
        public DateTime? Birthday { get; set; }

        public string Username { get; set; }

        public string Gmail
        {
            get => email;
            set => email = value?.Trim();
        }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        // ✅ FIX: SteamId više NEMA default "0"
        // Ako user ne unese SteamID → NULL
        public string? SteamId { get; set; }

        public int CommentAuthorId { get; set; }

        public IReadOnlyList<Match> Matches { get; private set; }

        // =========================
        // EMAIL VERIFICATION
        // =========================
        public bool EmailConfirmed { get; set; }
        public string EmailToken { get; set; }
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
            SteamId = null; // ✅ FIX
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
            SteamId = null; // ✅ FIX
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
            SteamId = steamId; // može biti NULL ili stvarni SteamID
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
            List<Match> matches
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
            SteamId = null; // ✅ FIX
            Matches = matches ?? new List<Match>();
        }

        // ⛔ VERY IMPORTANT — REPO MINIMAL CONSTRUCTOR
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
