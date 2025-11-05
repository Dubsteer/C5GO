using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer.IRepos;
using LogicLayer.Models;


namespace Unit_Tests.MockRepos
{
    public class MockTournamentRepo : ITournamentRepo
    {
        public List<Tournament> Tournaments = new();

        public MockTournamentRepo(List<Tournament> tournaments)
        {
            Tournaments = tournaments;
        }

        public void AddTournament(Tournament tournament)
        {
            Tournaments.Add(tournament);
        }

        public void AddTournamentApp(Player player, Tournament tournament)
        {
            // Find the tournament and add the player
            var tourney = Tournaments.FirstOrDefault(t => t.Id == tournament.Id);
            if (tourney != null)
            {
                if (tourney.Players == null)
                {
                    tourney.Players = new List<Player>();
                }
                tourney.Players.Add(player);
            }
        }

        public List<Player> GetAllPlayersInTournament(int tournamentId)
        {
            // Find the tournament and return its players
            var tournament = Tournaments.FirstOrDefault(t => t.Id == tournamentId);
            return tournament?.Players;
        }

        public List<Tournament> GetAllTournaments()
        {
            return Tournaments;
        }

        public void RemoveTournament(Tournament tournament)
        {
            var tourney = Tournaments.FirstOrDefault(t => t.Id == tournament.Id);
            if (tourney != null)
            {
                Tournaments.Remove(tourney);
            }
        }

        public void UpdateTournament(Tournament tournament)
        {
            for (int i = 0; i < Tournaments.Count; i++)
            {
                if (Tournaments[i].Id == tournament.Id)
                {
                    Tournaments[i] = tournament;
                    return;
                }
            }
            // If no matching tournament is found, you might want to throw an exception.
            throw new Exception("Tournament not found");
        }
    }
}
