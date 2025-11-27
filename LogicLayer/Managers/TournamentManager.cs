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

        // BASIC
        public List<Tournament> GetAllTournaments() => repo.GetAllTournaments();
        public Tournament GetTournamentById(int id) => repo.GetTournamentById(id);
        public void AddTournament(Tournament t) => repo.AddTournament(t);
        public void RemoveTournament(Tournament t) => repo.RemoveTournament(t);

        // PLAYERS
        public void AddTournamentApp(Player p, Tournament t) => repo.AddTournamentApp(p, t);
        public void RemovePlayerFromTournament(Player p, Tournament t) => repo.RemovePlayerFromTournament(p, t);
        public List<Player> GetAllPlayersInTournament(Tournament t) => repo.GetAllPlayersInTournament(t.Id);

        // TEAMS
        public void AddTeamToTournament(int teamId, int tournamentId) => repo.AddTeamTournamentApp(teamId, tournamentId);
        public List<int> GetTeamsInTournament(Tournament t) => repo.GetTeamApplications(t.Id);
        public void RemoveTeamFromTournament(int teamId, int tournamentId) => repo.RemoveTeamTournamentApp(teamId, tournamentId);

        // SOLO MATCHES
        public List<Match> GetAllMatchesInTournament(Tournament t)
            => matchManager.GetMatchesByTournamentId(t.Id);

        // TEAM MATCHES
        public List<TeamMatch> GetAllTeamMatchesInTournament(Tournament t)
            => teamMatchManager.GetTeamMatchesByTournament(t.Id);

        // STATUS
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

        // GENERATE SOLO BRACKET
        public void GenerateSoloBracket(List<Player> players, Tournament t)
        {
            matchManager.GenerateMatches(players, t.Id, DateTime.Now, 1);
        }

        // GENERATE TEAM BRACKET
        public void GenerateTeamBracket(List<int> teamIds, Tournament t)
        {
            teamMatchManager.GenerateTeamBracket(teamIds, t.Id);
        }

        public void SetStatus(Tournament t, Status s)
        {
            t.Status = s;
            repo.UpdateTournament(t);
        }

    }
}
