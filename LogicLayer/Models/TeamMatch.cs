using System;
using LogicLayer.Enums;

namespace LogicLayer.Models
{
    public class TeamMatch
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }

        public int Team1Id { get; set; }
        public int Team2Id { get; set; }

        public Team Team1 { get; set; } = null!;
        public Team Team2 { get; set; } = null!;

        public int Team1Score { get; set; }
        public int Team2Score { get; set; }

        public DateTime MatchDate { get; set; }
        public Status Status { get; set; }
        public string TournamentName { get; set; } = string.Empty;

        public TeamMatch(int id, int tournamentId, Team t1, Team t2,
                         int s1, int s2, DateTime date, Status status)
        {
            Id = id;
            TournamentId = tournamentId;
            Team1 = t1;
            Team2 = t2;
            Team1Id = t1.Id;
            Team2Id = t2.Id;
            Team1Score = s1;
            Team2Score = s2;
            MatchDate = date;
            Status = status;
        }

        public TeamMatch() { }
    }
}
