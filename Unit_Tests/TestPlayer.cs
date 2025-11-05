using System.Collections.Generic;
using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestPlayer
    {
        private static List<Player> players;
        private static MockPlayerRepo mockPlayerRepo;
        private static PlayerManager playerManager;

        [ClassInitialize]
        public static void TestClassSetup(TestContext context)
        {
            players = new List<Player>();
            mockPlayerRepo = new MockPlayerRepo(players);
            playerManager = new PlayerManager(mockPlayerRepo);
        }

        [TestInitialize]
        public void Setup()
        {
            players.Clear();
        }

        [TestMethod]
        public void TestInitializeRole()
        {
            // Arrange
            var player = new Player(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", "steam123", false);

            // Act
            playerManager.InitializeRole(player);

            // Assert
            var allPlayers = playerManager.GetAllPlayers();
            Assert.AreEqual(1, allPlayers.Count);
            Assert.AreEqual(player.Steamaccountid, allPlayers[0].Steamaccountid);
        }

        [TestMethod]
        public void TestDeletePlayerRole()
        {
            // Arrange
            var player = new Player(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", "steam123", false);
            playerManager.InitializeRole(player);

            // Act
            playerManager.DeletePlayerRole(player);

            // Assert
            Assert.AreEqual(0, players.Count);
        }

        [TestMethod]
        public void TestGetAllPlayers()
        {
            // Arrange
            var player1 = new Player(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", "steam123", false);
            var player2 = new Player(2, "John", "Doe", 25, "johndoe", "johndoe@gmail.com", "password", "steam456", false);
            playerManager.InitializeRole(player1);
            playerManager.InitializeRole(player2);

            // Act
            var allPlayers = playerManager.GetAllPlayers();

            // Assert
            Assert.AreEqual(2, allPlayers.Count);
        }

        [TestMethod]
        public void TestGetPlayer()
        {
            // Arrange
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            var player = new Player((int)user.Id, user.Firstname, user.Lastname, user.Age, user.Username, user.Gmail, user.Password, "steam123", false);
            playerManager.InitializeRole(player);

            // Act
            var fetchedPlayer = playerManager.GetPlayer(user);

            // Assert
            Assert.AreEqual(player, fetchedPlayer);
        }
    }
}