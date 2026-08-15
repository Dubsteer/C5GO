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

        public bool DeletePlayerRole(int userId)
        {
            var existingPlayer = players.Find(p => p.Id == userId);
            return existingPlayer != null && players.Remove(existingPlayer);
        }

        public List<Player> GetAllPlayers()
        {
            return players;
        }

        public Player? GetPlayer(User user)
        {
            return players.Find(p => p.Id == user.Id);
        }

    }
}
