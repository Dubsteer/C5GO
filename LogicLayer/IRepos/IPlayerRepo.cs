using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IPlayerRepo
    {
        void InitializeRole(Player player);

        void AddPlayerToTournament(Player player, Tournament tournament);
       
        bool DeletePlayerRole(int userId);
        List<Player> GetAllPlayers();

        Player? GetPlayer(User user);
    }
}
