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
        public void TestInitializeRole()
        {
            var player = CreatePlayer(1, "dubsteer", "steam123");

            playerManager.InitializeRole(player);

            Assert.AreEqual(1, playerManager.GetAllPlayers().Count);
            Assert.AreEqual(player.SteamId, playerManager.GetAllPlayers()[0].SteamId);
        }

        [TestMethod]
        public void TestGetAllPlayers()
        {
            playerManager.InitializeRole(CreatePlayer(1, "player1", "steam1"));
            playerManager.InitializeRole(CreatePlayer(2, "player2", "steam2"));

            var allPlayers = playerManager.GetAllPlayers();

            Assert.AreEqual(2, allPlayers.Count);
        }

        [TestMethod]
        public void TestGetPlayer()
        {
            var player = CreatePlayer(1, "dubsteer", "steam123");
            playerManager.InitializeRole(player);

            var fetchedPlayer = playerManager.GetPlayer(player);

            Assert.AreSame(player, fetchedPlayer);
        }

        private static Player CreatePlayer(int id, string username, string steamId) =>
            new Player(id, "Test", "Player", 22, username, $"{username}@test.local", "password", steamId, false);
    }
}
