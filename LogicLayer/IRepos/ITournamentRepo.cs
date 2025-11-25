using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ITournamentRepo
    {
        List<Tournament> GetAllTournaments();
        void AddTournament(Tournament tournament);
        void UpdateTournament(Tournament tournament);
        void RemoveTournament(Tournament tournament);

        // PLAYER APPS
        void AddTournamentApp(Player player, Tournament tournament);
        void RemovePlayerFromTournament(Player player, Tournament tournament);

        List<Player> GetAllPlayersInTournament(int tournamentId);

        // TEAM APPS
        void AddTeamTournamentApp(int teamId, int tournamentId);
        List<int> GetTeamApplications(int tournamentId);
    }
}
