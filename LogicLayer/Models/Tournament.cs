using LogicLayer.Enums;
using System.Collections.Generic;

namespace LogicLayer.Models
{
    public class Tournament
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }

        public bool IsTeamTournament { get; set; }
        public int TeamSizeRequired { get; set; }

        // Loaded collections
        public List<Player> Players { get; set; } = new();
        public List<Match> Matches { get; set; } = new();
        public List<int> TeamIds { get; set; } = new();
        public List<Team> Teams { get; set; } = new();

        // UI helpers
        public int PlayersCount => Players.Count;
        public int TeamsCount => Teams.Count;
        public int MatchesCount => Matches.Count;

        public bool CanLeave { get; set; }

        public Tournament() { }
    }
}
