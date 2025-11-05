using LogicLayer.Models;
using LogicLayer.IRepos;
using LogicLayer.Exceptions;
using LogicLayer.Enums;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LogicLayer.Managers
{
    public class TournamentManager
    {
        private readonly ITournamentRepo tournamentrepo;
        private MatchManager MatchManager;

        public TournamentManager(ITournamentRepo tournamentrepo, MatchManager matchManager)
        {
            this.tournamentrepo = tournamentrepo;
            MatchManager = matchManager;
        }

        public Tournament GetTournamentById(int id)
        {
            var tournament = GetAllTournaments().FirstOrDefault(t => t.Id == id);
            if (tournament == null)
            {
                throw new TournamentNotFoundException("Tournament not found");
            }
            return tournament;
        }

        public List<Tournament> GetAllTournaments()
        {
            var tournaments = tournamentrepo.GetAllTournaments();
            foreach (Tournament t in tournaments)
            {
                t.Players = GetAllPlayersInTournament(t);
            }
            return tournaments;
        }

        public void AddTournament(Tournament tournament)
        {
            tournamentrepo.AddTournament(tournament);
        }

        public void CloseTournament(Tournament tournament)
        {
            tournament.Closed = true;
            tournamentrepo.UpdateTournament(tournament);
        }

        public void AddTournamentApp(Player player, Tournament tournament)
        {
            if (tournament.Closed)
            {
                throw new TournamentClosedException("This tournament is not accepting new players");
            }

            tournamentrepo.AddTournamentApp(player, tournament);
        }

        public void RemoveTournament(Tournament tournament)
        {
            foreach (Tournament tournaments in GetAllTournaments())
            {
                if (tournament.Id == tournament.Id)
                {
                    tournamentrepo.RemoveTournament(tournament);
                    return;
                }
            }
        }

        public void UpdateTournament(Tournament tournament)
        {
            foreach (Tournament tournaments in GetAllTournaments())
            {
                if (tournaments.Id == tournament.Id)
                {
                    tournamentrepo.UpdateTournament(tournament);
                    return;
                }
            }
        }

        public List<Player> GetAllPlayersInTournament(Tournament tournament)
        {
            return tournamentrepo.GetAllPlayersInTournament(tournament.Id);
        }

        public List<Match> GetAllMatchesInTournament(Tournament tournament)
        {
            return MatchManager.GetAllMatches().Where(m => m.TournamentId == tournament.Id).ToList();
        }

        public void TournamentLogic(List<Player> players, Tournament tournament, DateTime startTime, int interval)
        {
            int numberOfPlayers = players.Count;

            if (numberOfPlayers != 4 && numberOfPlayers != 6 && numberOfPlayers != 8 && numberOfPlayers != 10)
            {
                throw new ArgumentException("A tournament requires 4, 6, 8, or 10 players");
            }

            DateTime StartTime = startTime;

            int matchId = 1;
            for (int i = 0; i < numberOfPlayers - 1; i++)
            {
                for (int j = i + 1; j < numberOfPlayers; j++)
                {
                    var match = new Match(matchId, tournament.Id, players[i], players[j], 0, 0, StartTime, Status.InProgress);
                    MatchManager.AddMatch(match);
                    StartTime = StartTime.AddMinutes(interval);
                    matchId++;
                }
            }
        }
    }
}