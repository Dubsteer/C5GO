using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IPlayerRepo
    {
        bool DeletePlayerRole(int userId);
        List<Player> GetAllPlayers();

        Player? GetPlayer(User user);
    }
}
