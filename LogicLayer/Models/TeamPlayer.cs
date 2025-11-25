using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Models
{
    public class TeamPlayer
    {
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } // Captain or Member
        public string Status { get; set; } // Pending or Approved

        public User User { get; set; }
    }
}

