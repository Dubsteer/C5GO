using LogicLayer.IRepos;
using LogicLayer.Models;

namespace LogicLayer.Managers
{
    public class PlayerManager
    {
        private readonly IPlayerRepo repo;

        public PlayerManager(IPlayerRepo repo)
        {
            this.repo = repo;
        }

        public void InitializeRole(Player p) => repo.InitializeRole(p);

        public Player GetPlayer(User u) => repo.GetPlayer(u);

        public List<Player> GetAllPlayers() => repo.GetAllPlayers();

        public void RemovePlayerRole(int userId)
        {
            if (!repo.DeletePlayerRole(userId))
                throw new InvalidOperationException("The selected user does not have a player profile.");
        }
    }
}
