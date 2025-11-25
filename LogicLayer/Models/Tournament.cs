using LogicLayer.Enums;
using System.Collections.Generic;

namespace LogicLayer.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Status Status { get; set; }

        // Solo players (1v1)
        public List<Player> Players { get; set; } = new();

        // Matches
        public List<Match> Matches { get; set; } = new();

        // 🔥 TEAM TOURNAMENT
        public bool IsTeamTournament { get; set; } = false;
        public int TeamSizeRequired { get; set; } = 1;  // 1 = solo, 5 = CS2

        public bool IsOpen => Status == Status.Open;
        public bool IsClosed => Status == Status.Closed;
        public bool IsInProgress => Status == Status.InProgress;

        public Tournament() { }

        public Tournament(int id, string name, string description, Status status,
                          bool isTeam, int teamSize)
        {
            Id = id;
            Name = name;
            Description = description;
            Status = status;
            IsTeamTournament = isTeam;
            TeamSizeRequired = teamSize;
        }
    }
}
