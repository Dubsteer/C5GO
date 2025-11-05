using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface ITournamentRepo
    {
        List<Tournament> GetAllTournaments();
        void AddTournament(Tournament tournament);
        void UpdateTournament(Tournament tournament);
        void RemoveTournament(Tournament tournament);
        void AddTournamentApp(Player player, Tournament tournament);
        List<Player> GetAllPlayersInTournament(int tournamentId);

        // ⬇️ OVO DODAJ
        void RemovePlayerFromTournament(Player player, Tournament tournament);
    }
}
