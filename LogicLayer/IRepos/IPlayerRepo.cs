using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IPlayerRepo
    {

        public void InitializeRole(Player player);

        public void AddPlayerToTournament(Player player, Tournament tournament);
       
        public void DeletePlayerRole(Player player);
        public List<Player> GetAllPlayers();

        Player GetPlayer(User user);
    }
}
