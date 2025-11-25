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

        public TournamentManager(ITournamentRepo repo, MatchManager matchManager)
        {
            this.repo = repo;
            this.matchManager = matchManager;
        }

        // =========================
        // CRUD
        // =========================

        public List<Tournament> GetAllTournaments()
        {
            return repo.GetAllTournaments();
        }

        public Tournament GetTournamentById(int id)
        {
            return repo.GetAllTournaments().FirstOrDefault(t => t.Id == id)
                ?? throw new Exception("Tournament not found");
        }

        public void UpdateTournament(Tournament t)
        {
            repo.UpdateTournament(t);
        }

        public void RemoveTournament(Tournament t)
        {
            repo.RemoveTournament(t);
        }

        // =========================
        // APPS
        // =========================

        public List<Player> GetAllPlayersInTournament(Tournament t)
        {
            return repo.GetAllPlayersInTournament(t.Id);
        }

        public void AddTournamentApp(Player p, Tournament t)
        {
            var players = GetAllPlayersInTournament(t);

            if (players.Any(x => x.Id == p.Id))
                throw new Exception("Player already in this tournament");

            repo.AddTournamentApp(p, t);
        }

        public void RemovePlayerFromTournament(Player p, Tournament t)
        {
            repo.RemovePlayerFromTournament(p, t);
        }

        // =========================
        // TEAM APPLY
        // =========================

        public void ApplyTeamTournament(Team team, Tournament t)
        {
            if (!t.IsTeamTournament)
                throw new Exception("This is not a team tournament.");

            if (team.Members.Count != t.TeamSizeRequired)
                throw new Exception("Team does not meet required size.");

            foreach (var member in team.Members)
            {
                var player = new Player(
                    member.Id!.Value,
                    member.Firstname,
                    member.Lastname,
                    member.Age,
                    member.Username,
                    member.Gmail,
                    member.Password,
                    member.SteamId,
                    member.IsAdmin
                );

                AddTournamentApp(player, t);
            }
        }

        // =========================
        // MATCHES
        // =========================

        public List<Match> GetAllMatchesInTournament(Tournament t)
        {
            return matchManager.GetMatchesByTournamentId(t.Id);
        }

        // =========================
        // AUTO STATUS MANAGER
        // =========================

        public void UpdateTournamentStatus(Tournament t)
        {
            var players = GetAllPlayersInTournament(t);
            var matches = GetAllMatchesInTournament(t);

            // If tournament is CLOSED, do not modify
            if (t.Status == Status.Closed)
                return;

            // If no matches yet but players joined → OPEN
            if (matches.Count == 0)
            {
                t.Status = Status.Open;
                repo.UpdateTournament(t);
                return;
            }

            // If any match is not finished → IN PROGRESS
            if (matches.Any(m => m.Status == Status.InProgress))
            {
                t.Status = Status.InProgress;
                repo.UpdateTournament(t);
                return;
            }

            // All matches finished → CLOSED
            t.Status = Status.Closed;
            repo.UpdateTournament(t);
        }

        // Helper
        public void SetStatus(Tournament t, Status status)
        {
            t.Status = status;
            repo.UpdateTournament(t);
        }
    }
}
