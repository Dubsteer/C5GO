using System;
using LogicLayer.Enums;

namespace LogicLayer.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }

        public int Player1Score { get; set; }
        public int Player2Score { get; set; }

        public Player User1 { get; set; }
        public Player User2 { get; set; }

        public DateTime MatchDate { get; set; }
        public Status Status { get; set; }
        public int RoundNumber { get; set; }
        public int BracketPosition { get; set; }

        public Match(int id, int tournamentId, Player u1, Player u2,
                     int p1, int p2, DateTime date, Status status,
                     int roundNumber = 1, int bracketPosition = 1)
        {
            Id = id;
            TournamentId = tournamentId;
            User1 = u1;
            User2 = u2;
            Player1Score = p1;
            Player2Score = p2;
            MatchDate = date;
            Status = status;
            RoundNumber = roundNumber;
            BracketPosition = bracketPosition;
        }

        public string TournamentName { get; set; } = string.Empty;

        public DateTime Date => MatchDate;

        public string PlayerScore => $"{Player1Score} : {Player2Score}";
    }
}
