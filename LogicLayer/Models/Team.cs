using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public User Captain { get; set; } = null!;
        public List<User> Members { get; set; } = [];

        public Team() { }

        public Team(int id, string name, User? captain)
        {
            Id = id;
            Name = name;
            Captain = captain!;
        }
    }
}

