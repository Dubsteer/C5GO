using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.Managers
{
    public class TournamentManager
    {
        private readonly ITournamentRepo repo;
        private readonly MatchManager matchManager;
        private readonly TeamMatchManager teamMatchManager;

        public TournamentManager(
            ITournamentRepo repo,
            MatchManager matchManager,
            TeamMatchManager teamMatchManager)
        {
            this.repo = repo;
            this.matchManager = matchManager;
            this.teamMatchManager = teamMatchManager;
        }

        public List<Tournament> GetAllTournaments() => repo.GetAllTournaments();
        public Tournament GetTournamentById(int id) => repo.GetTournamentById(id)
            ?? throw new Exceptions.TournamentNotFoundException("Tournament was not found.");

        public void AddTournament(Tournament tournament)
        {
            if (tournament == null)
                throw new ArgumentNullException(nameof(tournament));

            if (!Enum.IsDefined(tournament.Status))
                throw new InvalidOperationException("Select a valid tournament status.");

            ValidateTournament(tournament);
            tournament.Name = tournament.Name.Trim();
            tournament.Description = tournament.Description.Trim();
            tournament.TeamSizeRequired = tournament.IsTeamTournament ? 5 : 1;
            repo.AddTournament(tournament);
        }

        public void UpdateTournament(
            int tournamentId,
            string name,
            string description,
            Status status)
        {
            var tournament = GetTournamentById(tournamentId);
            tournament.Name = name;
            tournament.Description = description;
            tournament.Status = status;
            if (!Enum.IsDefined(status))
                throw new InvalidOperationException("Select a valid tournament status.");
            ValidateTournament(tournament);
            tournament.Name = tournament.Name.Trim();
            tournament.Description = tournament.Description.Trim();
            repo.UpdateTournament(tournament);
        }

        public void RemoveTournament(Tournament tournament)
        {
            if (tournament == null || tournament.Id <= 0)
                throw new ArgumentException("A valid tournament is required.", nameof(tournament));

            repo.RemoveTournament(tournament);
        }

        public void AddTournamentApp(Player player, Tournament tournament)
        {
            if (player?.Id == null)
                throw new InvalidOperationException("A valid player account is required.");

            if (tournament.IsTeamTournament)
                throw new InvalidOperationException("Individual players cannot register for a team tournament.");

            EnsureRegistrationIsOpen(tournament);
            EnsureRosterCanChange(tournament);

            if (repo.GetAllPlayersInTournament(tournament.Id).Any(existing => existing.Id == player.Id))
                throw new InvalidOperationException("This player is already registered.");

            repo.AddTournamentApp(player, tournament);
        }
        public void RemovePlayerFromTournament(Player player, Tournament tournament)
        {
            if (tournament.IsTeamTournament)
                throw new InvalidOperationException("Individual players are not registered for team tournaments.");

            EnsureRosterCanChange(tournament);
            repo.RemovePlayerFromTournament(player, tournament);
        }
        public List<Player> GetAllPlayersInTournament(Tournament t) => repo.GetAllPlayersInTournament(t.Id);

        public void AddTeamToTournament(int teamId, int tournamentId)
        {
            if (teamId <= 0)
                throw new InvalidOperationException("A valid team is required.");

            var tournament = GetTournamentById(tournamentId);
            EnsureTeamTournament(tournament);
            EnsureRegistrationIsOpen(tournament);
            EnsureRosterCanChange(tournament);

            if (repo.GetTeamApplications(tournamentId).Contains(teamId))
                throw new InvalidOperationException("This team is already registered.");

            repo.AddTeamTournamentApp(teamId, tournamentId);
        }
        public List<int> GetTeamsInTournament(Tournament t) => repo.GetTeamApplications(t.Id);
        public void RemoveTeamFromTournament(int teamId, int tournamentId)
        {
            var tournament = GetTournamentById(tournamentId);
            EnsureTeamTournament(tournament);
            EnsureRosterCanChange(tournament);
            repo.RemoveTeamTournamentApp(teamId, tournamentId);
        }

        public List<Match> GetAllMatchesInTournament(Tournament t)
            => matchManager.GetMatchesByTournamentId(t.Id);

        public List<TeamMatch> GetAllTeamMatchesInTournament(Tournament t)
            => teamMatchManager.GetTeamMatchesByTournament(t.Id);

        public void UpdateTournamentStatus(Tournament t)
        {
            if (!t.IsTeamTournament)
            {
                var matches = matchManager.GetMatchesByTournamentId(t.Id);

                if (matches.Count == 0)
                    t.Status = Status.Open;
                else if (matches.Any(m => m.Status == Status.Open || m.Status == Status.InProgress))
                    t.Status = Status.InProgress;
                else
                    t.Status = Status.Closed;
            }
            else
            {
                var teamMatches = teamMatchManager.GetTeamMatchesByTournament(t.Id);

                if (teamMatches.Count == 0)
                    t.Status = Status.Open;
                else if (teamMatches.Any(m => m.Status == Status.Open || m.Status == Status.InProgress))
                    t.Status = Status.InProgress;
                else
                    t.Status = Status.Closed;
            }

            repo.UpdateTournament(t);
        }

        public void GenerateSoloBracket(List<Player> players, Tournament tournament, bool replaceExisting = false)
        {
            if (tournament.IsTeamTournament)
                throw new InvalidOperationException("This action is available only for solo tournaments.");

            var existing = matchManager.GetMatchesByTournamentId(tournament.Id);
            if (existing.Count > 0 && !replaceExisting)
                throw new InvalidOperationException("A bracket already exists for this tournament.");

            if (players == null || players.Count < 2 || players.Count % 2 != 0)
                throw new InvalidOperationException("A solo bracket requires an even number of at least two players.");

            foreach (var match in existing)
                matchManager.RemoveMatch(match);

            matchManager.GenerateMatches(players, tournament.Id, DateTime.Now, 1);
            UpdateTournamentStatus(tournament);
        }

        public void GenerateTeamBracket(List<int> teamIds, Tournament tournament, bool replaceExisting = false)
        {
            EnsureTeamTournament(tournament);
            teamMatchManager.GenerateTeamBracket(teamIds, tournament.Id, replaceExisting);
            UpdateTournamentStatus(tournament);
        }

        public void SetStatus(Tournament t, Status s)
        {
            t.Status = s;
            repo.UpdateTournament(t);
        }

        private void EnsureRosterCanChange(Tournament tournament)
        {
            var hasMatches = tournament.IsTeamTournament
                ? teamMatchManager.GetTeamMatchesByTournament(tournament.Id).Count > 0
                : matchManager.GetMatchesByTournamentId(tournament.Id).Count > 0;

            if (hasMatches)
                throw new InvalidOperationException("Participants cannot be changed after a bracket has been generated.");
        }

        private static void EnsureTeamTournament(Tournament tournament)
        {
            if (!tournament.IsTeamTournament)
                throw new InvalidOperationException("This action is available only for team tournaments.");
        }

        private static void EnsureRegistrationIsOpen(Tournament tournament)
        {
            if (tournament.Status != Status.Open)
                throw new InvalidOperationException("Registration is closed for this tournament.");
        }

        private static void ValidateTournament(Tournament tournament)
        {
            if (tournament == null)
                throw new ArgumentNullException(nameof(tournament));

            if (string.IsNullOrWhiteSpace(tournament.Name) || tournament.Name.Trim().Length > 50)
                throw new InvalidOperationException("Tournament name is required and must not exceed 50 characters.");

            if (string.IsNullOrWhiteSpace(tournament.Description) || tournament.Description.Trim().Length > 300)
                throw new InvalidOperationException("Description is required and must not exceed 300 characters.");
        }

    }
}
