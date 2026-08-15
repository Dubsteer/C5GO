using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ITournamentRepo
    {
        List<Tournament> GetAllTournaments();
        Tournament? GetTournamentById(int id);

        void AddTournament(Tournament tournament);
        void UpdateTournament(Tournament tournament);
        void RemoveTournament(Tournament tournament);

        void AddTournamentApp(Player player, Tournament tournament);
        void RemovePlayerFromTournament(Player player, Tournament tournament);

        List<Player> GetAllPlayersInTournament(int tournamentId);
        bool HasActivePlayerRegistration(int userId);
        bool HasActiveTeamRegistration(int teamId);

        void AddTeamTournamentApp(int teamId, int tournamentId);
        List<int> GetTeamApplications(int tournamentId);

        void RemoveTeamTournamentApp(int teamId, int tournamentId);
    }
}
