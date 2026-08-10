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
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public User User { get; set; } = null!;
    }
}

