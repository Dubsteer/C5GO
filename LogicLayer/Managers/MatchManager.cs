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

        private Match GetMatchById(int id)
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

            if (GetMatchesByTournamentId(tournamentId).Any(candidate => candidate.RoundNumber > match.RoundNumber))
                throw new InvalidOperationException("A completed bracket round cannot be changed after the next round starts.");

            ValidateResult(player1Score, player2Score, status, matchDate);

            match.Player1Score = player1Score;
            match.Player2Score = player2Score;
            match.Status = status;
            match.MatchDate = matchDate;
            matchRepo.UpdateMatch(match);
        }

        public List<Match> GetPastMatches(User user, int limit = 20)
        {
            ArgumentNullException.ThrowIfNull(user);
            if (user.Id is not int userId)
                return [];

            ValidateHistoryLimit(limit);

            return GetAllMatches()
                .Where(m =>
                    m.Status == Status.Closed &&
                    (m.User1.Id == userId || m.User2.Id == userId))
                .OrderByDescending(m => m.MatchDate)
                .Take(limit)
                .ToList();
        }

        public List<Match> GetRecentCompletedMatches(int limit = 20)
        {
            ValidateHistoryLimit(limit);

            return GetAllMatches()
                .Where(match => match.Status == Status.Closed)
                .OrderByDescending(match => match.MatchDate)
                .Take(limit)
                .ToList();
        }

        public void GenerateOpeningRound(List<Player> players, int tournamentId, DateTime startDate)
        {
            ValidateRoundParticipants(players);

            var shuffled = BracketPlanner.Shuffle(players);
            var openingCount = BracketPlanner.GetOpeningParticipantCount(shuffled.Count);
            GenerateRound(shuffled.Take(openingCount).ToList(), tournamentId, startDate, 1);
        }

        public void GenerateRound(
            IReadOnlyList<Player> players,
            int tournamentId,
            DateTime matchDate,
            int roundNumber)
        {
            ValidateRoundParticipants(players);
            ArgumentOutOfRangeException.ThrowIfLessThan(roundNumber, 1);

            for (var index = 0; index < players.Count; index += 2)
            {
                AddMatch(new Match(
                    0,
                    tournamentId,
                    players[index],
                    players[index + 1],
                    0,
                    0,
                    matchDate,
                    Status.Open,
                    roundNumber,
                    index / 2 + 1));
            }
        }

        private static void ValidateRoundParticipants(IReadOnlyList<Player>? players)
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

        private static void ValidateHistoryLimit(int limit)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(limit, 100);
        }
    }
}
