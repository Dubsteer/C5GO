using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestPlayer
    {
        private static List<Player> players = null!;
        private static PlayerManager playerManager = null!;

        [ClassInitialize]
        public static void TestClassSetup(TestContext _)
        {
            players = new List<Player>();
            playerManager = new PlayerManager(new MockPlayerRepo(players));
        }

        [TestInitialize]
        public void Setup() => players.Clear();

        [TestMethod]
        public void TestGetAllPlayers()
        {
            players.Add(CreatePlayer(1, "player1", "steam1"));
            players.Add(CreatePlayer(2, "player2", "steam2"));

            var allPlayers = playerManager.GetAllPlayers();

            Assert.AreEqual(2, allPlayers.Count);
        }

        [TestMethod]
        public void TestGetPlayer()
        {
            var player = CreatePlayer(1, "dubsteer", "steam123");
            players.Add(player);

            var fetchedPlayer = playerManager.GetPlayer(player);

            Assert.AreSame(player, fetchedPlayer);
        }

        [TestMethod]
        public void TestRemovePlayerRole()
        {
            players.Add(CreatePlayer(1, "dubsteer", "steam123"));

            playerManager.RemovePlayerRole(1);

            Assert.AreEqual(0, playerManager.GetAllPlayers().Count);
        }

        [TestMethod]
        public void TestRemoveMissingPlayerRole()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                playerManager.RemovePlayerRole(99));
        }

        private static Player CreatePlayer(int id, string username, string steamId) =>
            new Player(id, "Test", "Player", 22, username, $"{username}@test.local", "password", steamId, false);
    }
}
