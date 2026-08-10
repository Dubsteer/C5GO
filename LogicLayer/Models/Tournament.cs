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

        public bool IsTeamTournament { get; set; }
        public int TeamSizeRequired { get; set; }

        public List<Player> Players { get; set; } = [];
        public List<Match> Matches { get; set; } = [];
        public List<int> TeamIds { get; set; } = [];
        public List<Team> Teams { get; set; } = [];

        public int PlayersCount { get; set; }
        public int TeamsCount { get; set; }
        public int MatchesCount { get; set; }
        public bool CanLeave { get; set; }

        public Tournament() { }
    }
}
