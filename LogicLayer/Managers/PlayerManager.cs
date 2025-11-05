using LogicLayer.Models;
using LogicLayer.IRepos;
using LogicLayer.Exceptions;

namespace LogicLayer.Managers
{
    public class PlayerManager
    {
        private readonly IPlayerRepo playerRepo;

        public PlayerManager(IPlayerRepo playerRepo)
        {
            this.playerRepo = playerRepo;
        }
        public void InitializeRole(Player player)
        {
            playerRepo.InitializeRole(player);
        }
        public void AddPlayerToTournament(Player player, Tournament tournament)
        {
            playerRepo.AddPlayerToTournament(player, tournament);
        }
        public void DeletePlayerRole(Player player)
        {
            playerRepo.DeletePlayerRole(player);
        }
        public List<Player> GetAllPlayers() 
        {
            var allUsers = playerRepo.GetAllPlayers();
            var allPlayers = new List<Player>();
            foreach ( var p in allUsers )
            {
                if(p.Steamaccountid != "0" && !string.IsNullOrEmpty(p.Steamaccountid))
                    allPlayers.Add(p);
            }
            return allPlayers;
        }
        public Player GetPlayer(User user)
        {
            return playerRepo.GetPlayer(user);
        }
    }
}
