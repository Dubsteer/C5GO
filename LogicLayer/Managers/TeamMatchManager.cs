using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.Managers
{
    public class TeamMatchManager
    {
        private readonly ITeamMatchRepo repo;

        public TeamMatchManager(ITeamMatchRepo repo)
        {
            this.repo = repo;
        }

        public List<TeamMatch> GetAllTeamMatches()
        {
            return repo.GetAllTeamMatches();
        }

        public List<TeamMatch> GetTeamMatchesByTournament(int tournamentId)
        {
            return repo.GetAllTeamMatches()
                       .Where(m => m.TournamentId == tournamentId)
                       .ToList();
        }

        public List<TeamMatch> GetRecentCompletedMatches(int limit = 20)
        {
            ValidateHistoryLimit(limit);

            return repo.GetAllTeamMatches()
                .Where(match => match.Status == Status.Closed)
                .OrderByDescending(match => match.MatchDate)
                .Take(limit)
                .ToList();
        }

        public List<TeamMatch> GetCompletedMatchesForTeam(int teamId, int limit = 5)
        {
            if (teamId <= 0)
                return [];

            ValidateHistoryLimit(limit);

            return repo.GetAllTeamMatches()
                .Where(match =>
                    match.Status == Status.Closed &&
                    (match.Team1Id == teamId || match.Team2Id == teamId))
                .OrderByDescending(match => match.MatchDate)
                .Take(limit)
                .ToList();
        }

        private TeamMatch GetTeamMatchById(int id)
        {
            return GetAllTeamMatches().FirstOrDefault(match => match.Id == id)
                ?? throw new InvalidOperationException("Team match was not found.");
        }

        public void AddTeamMatch(TeamMatch match)
        {
            repo.AddTeamMatch(match);
        }

        public void UpdateResult(
            int matchId,
            int tournamentId,
            int team1Score,
            int team2Score,
            Status status,
            DateTime matchDate)
        {
            var match = GetTeamMatchById(matchId);

            if (match.TournamentId != tournamentId)
                throw new InvalidOperationException("Match does not belong to this tournament.");

            if (GetTeamMatchesByTournament(tournamentId).Any(candidate => candidate.RoundNumber > match.RoundNumber))
                throw new InvalidOperationException("A completed bracket round cannot be changed after the next round starts.");

            ValidateResult(team1Score, team2Score, status, matchDate);

            match.Team1Score = team1Score;
            match.Team2Score = team2Score;
            match.Status = status;
            match.MatchDate = matchDate;
            repo.UpdateTeamMatch(match);
        }

        public void RemoveTeamMatch(TeamMatch match)
        {
            repo.RemoveTeamMatch(match);
        }

        public void GenerateTeamBracket(List<int> teamIds, int tournamentId, bool replaceExisting = false)
        {
            ArgumentNullException.ThrowIfNull(teamIds);

            var uniqueTeamIds = teamIds.Distinct().ToList();
            if (uniqueTeamIds.Count != teamIds.Count)
                throw new InvalidOperationException("A team can appear only once in a bracket.");

            if (!(uniqueTeamIds.Count == 8 || uniqueTeamIds.Count == 12 || uniqueTeamIds.Count == 16))
                throw new InvalidOperationException("Team bracket requires 8, 12 or 16 teams.");

            var existing = GetTeamMatchesByTournament(tournamentId);
            if (existing.Count > 0 && !replaceExisting)
                throw new InvalidOperationException("A bracket already exists for this tournament.");

            foreach (var match in existing)
                RemoveTeamMatch(match);

            var shuffled = BracketPlanner.Shuffle(uniqueTeamIds);
            var openingCount = BracketPlanner.GetOpeningParticipantCount(shuffled.Count);
            GenerateRound(shuffled.Take(openingCount).ToList(), tournamentId, DateTime.Now, 1);
        }

        public void GenerateRound(
            IReadOnlyList<int> teamIds,
            int tournamentId,
            DateTime matchDate,
            int roundNumber)
        {
            if (teamIds.Count < 2 || teamIds.Count % 2 != 0)
                throw new InvalidOperationException("A bracket round requires an even number of at least two teams.");

            if (teamIds.Any(teamId => teamId <= 0) || teamIds.Distinct().Count() != teamIds.Count)
                throw new InvalidOperationException("Bracket teams must be unique and valid.");

            ArgumentOutOfRangeException.ThrowIfLessThan(roundNumber, 1);

            for (var index = 0; index < teamIds.Count; index += 2)
            {
                AddTeamMatch(new TeamMatch(
                    0,
                    tournamentId,
                    new Team(teamIds[index], string.Empty, null!),
                    new Team(teamIds[index + 1], string.Empty, null!),
                    0,
                    0,
                    matchDate,
                    Status.Open,
                    roundNumber,
                    index / 2 + 1));
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
