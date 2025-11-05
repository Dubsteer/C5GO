using System;
using System.Collections.Generic;
using System.Linq;
using LogicLayer.Enums;
using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestTournament
    {
        private static List<Tournament> tournaments;
        private static MockTournamentRepo mockTournamentRepo;
        private static TournamentManager tournamentManager;
        private static List<Match> matches;
        private static MockMatchRepo mockMatchRepo;

        [ClassInitialize]
        public static void TestClassSetup(TestContext context)
        {
            tournaments = new List<Tournament>();
            mockTournamentRepo = new MockTournamentRepo(tournaments);
            matches = new List<Match>();
            mockMatchRepo = new MockMatchRepo(matches);
            var matchManager = new MatchManager(mockMatchRepo);
            tournamentManager = new TournamentManager(mockTournamentRepo, matchManager);
        }

        [TestInitialize]
        public void Setup()
        {
            tournaments.Clear();
            matches.Clear();
        }

        [TestMethod]
        public void TestAddTournament()
        {
            // Arrange
            var tournament = new Tournament(1, "Tournament 1", "Description 1");

            // Act
            tournamentManager.AddTournament(tournament);

            // Assert
            Assert.AreEqual(1, tournaments.Count);
            Assert.AreEqual(tournament, tournaments[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(TournamentNotFoundException))]
        public void TestAddDuplicateTournament()
        {
            // Arrange
            var tournament = new Tournament(1, "Tournament 1", "Description 1");
            tournamentManager.AddTournament(tournament);

            // Act
            tournamentManager.AddTournament(tournament);

            // Assert
            // TournamentNotFoundException should be thrown
        }

        [TestMethod]
        public void TestGetAllTournaments()
        {
            // Arrange
            var tournament1 = new Tournament(1, "Tournament 1", "Description 1");
            var tournament2 = new Tournament(2, "Tournament 2", "Description 2");
            tournaments.Add(tournament1);
            tournaments.Add(tournament2);

            // Act
            var result = tournamentManager.GetAllTournaments();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(tournament1));
            Assert.IsTrue(result.Contains(tournament2));
        }

        [TestMethod]
        public void TestRemoveTournament()
        {
            // Arrange
            var tournament1 = new Tournament(1, "Tournament 1", "Description 1");
            var tournament2 = new Tournament(2, "Tournament 2", "Description 2");
            tournaments.Add(tournament1);
            tournaments.Add(tournament2);

            // Act
            tournamentManager.RemoveTournament(tournament1);

            // Assert
            Assert.AreEqual(1, tournaments.Count);
            Assert.IsFalse(tournaments.Contains(tournament1));
            Assert.IsTrue(tournaments.Contains(tournament2));
        }

        [TestMethod]
        public void TestUpdateTournament()
        {
            // Arrange
            var tournament = new Tournament(1, "Tournament 1", "Description 1");
            tournaments.Add(tournament);

            // Act
            tournament.Name = "Updated Tournament";
            tournamentManager.UpdateTournament(tournament);

            // Assert
            Assert.AreEqual("Updated Tournament", tournaments[0].Name);
        }

        [TestMethod]
        public void TestTournamentLogic()
        {
            // Arrange
            var player1 = new Player(1, "John", "Doe", 25, "johndoe", "johndoe@gmail.com", "password", "steamid1", false);
            var player2 = new Player(2, "Jane", "Smith", 28, "janesmith", "janesmith@gmail.com", "password", "steamid2", false);
            var player3 = new Player(3, "Mike", "Johnson", 30, "mikejohnson", "mikejohnson@gmail.com", "password", "steamid3", false);
            var player4 = new Player(4, "Sarah", "Williams", 22, "sarahwilliams", "sarahwilliams@gmail.com", "password", "steamid4", false);

            var players = new List<Player> { player1, player2, player3, player4 };
            var tournament = new Tournament(1, "Test Tournament", "Description");

            DateTime startTime = DateTime.Now;
            int interval = 30;

            // Act
            tournamentManager.TournamentLogic(players, tournament, startTime, interval);

            // Assert
            List<Match> matches = tournamentManager.GetAllMatchesInTournament(tournament);
            Assert.AreEqual(6, matches.Count);
        }

        [TestMethod]
        public void TestGetAllMatchesInTournament()
        {
            // Arrange
            var tournament = new Tournament(1, "Tournament 1", "Description 1");
            tournaments.Add(tournament);

            var player1 = new Player(1, "Player 1", "Player 1", 25, "player1", "player1@gmail.com", "password", "steamid1", false);
            var player2 = new Player(2, "Player 2", "Player 2", 28, "player2", "player2@gmail.com", "password", "steamid2", false);
            var player3 = new Player(3, "Player 3", "Player 3", 30, "player3", "player3@gmail.com", "password", "steamid3", false);
            var player4 = new Player(4, "Player 4", "Player 4", 22, "player4", "player4@gmail.com", "password", "steamid4", false);

            var match1 = new Match(1, 1, player1, player2, 0, 0, DateTime.Now, Status.InProgress);
            var match2 = new Match(2, 1, player3, player4, 0, 0, DateTime.Now, Status.InProgress);

            matches.Add(match1);
            matches.Add(match2);

            // Act
            var result = tournamentManager.GetAllMatchesInTournament(tournament);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(match1));
            Assert.IsTrue(result.Contains(match2));
        }
    }
}