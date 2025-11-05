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

        // --------------------------
        // BASIC CRUD
        // --------------------------
        public List<Tournament> GetAllTournaments()
        {
            return repo.GetAllTournaments();
        }

        public Tournament GetTournamentById(int id)
        {
            return repo.GetAllTournaments().FirstOrDefault(t => t.Id == id)
                ?? throw new Exception("Tournament not found");
        }

        public void AddTournament(Tournament t)
        {
            repo.AddTournament(t);
        }

        public void UpdateTournament(Tournament t)
        {
            repo.UpdateTournament(t);
        }

        public void RemoveTournament(Tournament t)
        {
            repo.RemoveTournament(t);
        }

        // --------------------------
        // PLAYERS
        // --------------------------
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

        // --------------------------
        // MATCHES
        // --------------------------
        public List<Match> GetAllMatchesInTournament(Tournament t)
        {
            return matchManager.GetMatchesByTournamentId(t.Id);
        }

        // --------------------------
        // STATUS
        // --------------------------
        public void SetStatus(Tournament t, Status status)
        {
            t.Status = status;
            repo.UpdateTournament(t);
        }

        public void OpenTournament(Tournament t) => SetStatus(t, Status.Open);
        public void CloseTournament(Tournament t) => SetStatus(t, Status.Closed);
        public void SetInProgress(Tournament t) => SetStatus(t, Status.InProgress);

        // --------------------------
        // DESKTOP APP – RUN TOURNAMENT
        // --------------------------
        public void TournamentLogic(List<Player> players, Tournament t, DateTime startDate, int rounds)
        {
            if (players.Count < 2)
                throw new Exception("Not enough players");

            // set status → InProgress
            SetInProgress(t);

            // GENERATE MATCHES
            matchManager.GenerateMatches(players, t.Id, startDate, rounds);

            // finish → Closed
            CloseTournament(t);
        }
    }
}
