using LogicLayer.Exceptions;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.Managers
{
    public class MatchManager
    {
        private readonly IMatchRepo matchRepo;

        public MatchManager(IMatchRepo matchRepo)
        {
            this.matchRepo = matchRepo;
        }

        public Match GetMatchById(int id)
        {
            return GetAllMatches().FirstOrDefault(m => m.Id == id)
                ?? throw new MatchNotFoundException("Match not found");
        }

        public List<Match> GetAllMatches()
        {
            return matchRepo.GetAllMatches();
        }

        public List<Match> GetMatchesByTournamentId(int tid)
        {
            return GetAllMatches().Where(m => m.TournamentId == tid).ToList();
        }

        public List<Match> GetAllMatchesInTournament(Tournament t)
        {
            return GetMatchesByTournamentId(t.Id);
        }

        public void AddMatch(Match match)
        {
            matchRepo.AddMatch(match);
        }

        public void RemoveMatch(Match match)
        {
            matchRepo.RemoveMatch(match);
        }

        public void UpdateMatch(Match match)
        {
            matchRepo.UpdateMatch(match);
        }

        public void UpdateResult(
            int matchId,
            int tournamentId,
            int player1Score,
            int player2Score,
            Status status,
            DateTime matchDate)
        {
            var match = GetMatchById(matchId);

            if (match.TournamentId != tournamentId)
                throw new InvalidOperationException("Match does not belong to this tournament.");

            ValidateResult(player1Score, player2Score, status, matchDate);

            match.Player1Score = player1Score;
            match.Player2Score = player2Score;
            match.Status = status;
            match.MatchDate = matchDate;
            matchRepo.UpdateMatch(match);
        }

        public List<Match> GetPastMatches(User user)
        {
            return GetAllMatches()
                .Where(m => m.User1.Id == user.Id || m.User2.Id == user.Id)
                .OrderByDescending(m => m.MatchDate)
                .ToList();
        }

        public void GenerateMatches(List<Player> players, int tournamentId, DateTime startDate, int rounds)
        {
            if (players == null || players.Count < 2)
                throw new InvalidOperationException("At least two players are required to generate a bracket.");

            if (players.Count % 2 != 0)
                throw new InvalidOperationException("An even number of players is required to generate a bracket.");

            if (players.Any(player => player.Id == null) ||
                players.Select(player => player.Id).Distinct().Count() != players.Count)
            {
                throw new InvalidOperationException("Bracket players must be unique and have valid accounts.");
            }

            if (rounds < 1)
                throw new ArgumentOutOfRangeException(nameof(rounds));

            var rnd = new Random();
            var shuffled = players.OrderBy(x => rnd.Next()).ToList();

            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < shuffled.Count - 1; i += 2)
                {
                    var match = new Match(
                        0,
                        tournamentId,
                        shuffled[i],
                        shuffled[i + 1],
                        0,
                        0,
                        startDate.AddDays(round),
                        Status.Open
                    );

                    AddMatch(match);
                }
            }
        }

        private static void ValidateResult(int score1, int score2, Status status, DateTime matchDate)
        {
            if (!Enum.IsDefined(status))
                throw new InvalidOperationException("Select a valid match status.");

            if (matchDate.Year < 2000 || matchDate.Year > 2100)
                throw new InvalidOperationException("Select a valid match date.");

            if (score1 < 0 || score1 > 99 || score2 < 0 || score2 > 99)
                throw new InvalidOperationException("Scores must be between 0 and 99.");

            if (status == Status.Closed && score1 == score2)
                throw new InvalidOperationException("A closed bracket match must have a winner.");
        }
    }
}
