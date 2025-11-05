using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Models
{
    public class Player : User
    {
        public string Steamaccountid { get; set; }
        public Player(int id, string firstname, string lastname, int age, string username, string gmail, string password, string steamaccountid, bool isAdmin) : base(id, firstname, lastname, age, username, gmail, password,isAdmin)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Age = age;
            Username = username;
            Gmail = gmail;
            Password = password;
            Steamaccountid = steamaccountid;
            IsAdmin = isAdmin;
        }

        public override string ToString()
       {
           return Firstname.ToString() + Lastname.ToString();
       }

    }
}
