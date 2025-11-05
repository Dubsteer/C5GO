using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface ITournamentRepo
    {
        public List<Tournament> GetAllTournaments();
        public void AddTournament(Tournament tournament);
        public void AddTournamentApp(Player player, Tournament tournament);
        public void RemoveTournament(Tournament tournament);
        public void UpdateTournament(Tournament tournament);
        public List<Player> GetAllPlayersInTournament(int tournamentId);
    }
}
