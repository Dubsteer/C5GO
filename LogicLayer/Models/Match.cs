using LogicLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public Match(int id,int tournamentId, int player1Score, int player2Score, DateTime matchdate, Status status) 
        {
            Id = id;
            TournamentId = tournamentId;
            Player1Score = player1Score;
            Player2Score = player2Score;
            MatchDate = matchdate;
            Status = status;
            
        }
        public Match(int id, int tournamentId, Player user1, Player user2, int player1Score, int player2Score, DateTime matchdate, Status status)
        {
            Id = id;
            TournamentId = tournamentId;
            User1 = user1;
            User2 = user2;
            Player1Score = player1Score;
            Player2Score = player2Score;
            MatchDate = matchdate;
            Status = status;

        }


    }
} 
