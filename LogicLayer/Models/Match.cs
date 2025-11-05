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

        public Match(int id, int tournamentId, Player u1, Player u2,
                     int p1, int p2, DateTime date, Status status)
        {
            Id = id;
            TournamentId = tournamentId;
            User1 = u1;
            User2 = u2;
            Player1Score = p1;
            Player2Score = p2;
            MatchDate = date;
            Status = status;
        }

        // ========================================
        // EXTRA PROPERTIES USED BY ViewProfile.cshtml
        // ========================================

        // 👉 Ovo puniš u ViewProfile.cshtml.cs
        public string TournamentName { get; set; } = string.Empty;

        // 👉 UI-friendly date format
        public DateTime Date => MatchDate;

        // 👉 "5 : 2" format za skor
        public string PlayerScore => $"{Player1Score} : {Player2Score}";
    }
}
