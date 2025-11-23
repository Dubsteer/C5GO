using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LogicLayer.Managers;

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

        public string SteamId { get; set; } = "0";

        public int CommentAuthorId { get; set; }

        public IReadOnlyList<Match> Matches { get; private set; }

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
