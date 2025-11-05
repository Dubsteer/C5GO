using System;

namespace LogicLayer.Models
{
    public class Player : User
    {
        public string Steamaccountid { get; set; }

        public Player(int id, string firstname, string lastname, int age, string username, string gmail,
                      string password, string steamaccountid, bool isAdmin)
            : base(id, firstname, lastname, age, username, gmail, password, isAdmin)
        {
            Steamaccountid = steamaccountid;
        }

        public override string ToString()
        {
            return $"{Firstname} {Lastname}";
        }
    }
}
