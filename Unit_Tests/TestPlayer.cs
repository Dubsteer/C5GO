using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestPlayer
    {
        private List<Player> players = null!;
        private List<Tournament> tournaments = null!;
        private MockTeamRepo teamRepo = null!;
        private PlayerManager playerManager = null!;

        [TestInitialize]
        public void Setup()
        {
            players = [];
            tournaments = [];
            teamRepo = new MockTeamRepo(players);
            playerManager = new PlayerManager(
                new MockPlayerRepo(players),
                teamRepo,
                new MockTournamentRepo(tournaments));
        }

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

        [TestMethod]
        public void TestCannotRemovePlayerRoleWhileInTeam()
        {
            var player = CreatePlayer(1, "dubsteer", "76561198012345678");
            players.Add(player);
            teamRepo.SeedTeam(new Team(1, "Team", player), player);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                playerManager.RemovePlayerRole(player.Id!.Value));
            Assert.HasCount(1, players);
        }

        [TestMethod]
        public void TestCannotRemovePlayerRoleDuringActiveTournament()
        {
            var player = CreatePlayer(1, "dubsteer", "76561198012345678");
            players.Add(player);
            tournaments.Add(new Tournament
            {
                Id = 1,
                Name = "Open Tournament",
                Description = "Description",
                Status = LogicLayer.Enums.Status.Open,
                Players = [player]
            });

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                playerManager.RemovePlayerRole(player.Id!.Value));
            Assert.HasCount(1, players);
        }

        [TestMethod]
        public void TestClosedTournamentHistoryDoesNotBlockRoleRemoval()
        {
            var player = CreatePlayer(1, "dubsteer", "76561198012345678");
            players.Add(player);
            tournaments.Add(new Tournament
            {
                Id = 1,
                Name = "Closed Tournament",
                Description = "Description",
                Status = LogicLayer.Enums.Status.Closed,
                Players = [player]
            });

            playerManager.RemovePlayerRole(player.Id!.Value);

            Assert.HasCount(0, players);
        }

        private static Player CreatePlayer(int id, string username, string steamId) =>
            new Player(id, "Test", "Player", 22, username, $"{username}@test.local", "password", steamId, false);
    }
}
