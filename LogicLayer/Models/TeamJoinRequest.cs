using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.Models
{
    public class TeamJoinRequest
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestedAt { get; set; }

        public User User { get; set; }
        public Team Team { get; set; }
    }
}

