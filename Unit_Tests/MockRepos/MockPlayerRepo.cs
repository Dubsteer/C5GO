using System.Collections.Generic;
using LogicLayer.Models;
using LogicLayer.IRepos;

namespace Unit_Tests.MockRepos
{
    public class MockPlayerRepo : IPlayerRepo
    {
        private readonly List<Player> players;

        public MockPlayerRepo(List<Player> players)
        {
            this.players = players;
        }

        public void AddPlayerToTournament(Player player, Tournament tournament)
        {
            // You can add your logic to associate the player with the tournament in memory
        }

        public void DeletePlayerRole(Player player)
        {
            var existingPlayer = players.Find(p => p.Id == player.Id);
            if (existingPlayer != null)
            {
                players.Remove(existingPlayer);
            }
        }

        public List<Player> GetAllPlayers()
        {
            return players;
        }

        public Player GetPlayer(User user)
        {
            return players.Find(p => p.Id == user.Id);
        }

        public void InitializeRole(Player player)
        {
            players.Add(player);
        }
    }
}