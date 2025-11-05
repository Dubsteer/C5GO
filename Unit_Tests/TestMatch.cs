using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer.Enums;
using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestMatch
    {
        private static List<Match> matches;
        private static MockMatchRepo mockMatchRepo;
        private static MatchManager MatchManager;

        [ClassInitialize]
        public static void TestClassSetuo(TestContext context)
        {
            matches = new List<Match>();
            mockMatchRepo = new MockMatchRepo(matches);
            MatchManager = new MatchManager(mockMatchRepo);
        }

        [TestInitialize]
        public void Setup()
        {
            matches.Clear();
        }

        [TestMethod]
        public void TestAddMatch()
        {
            // Arrange
            var match = new Match(1, 1, 0, 0, DateTime.Now, Status.Open);

            // Act
            MatchManager.AddMatch(match);

            // Assert
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(match, matches[0]);
        }

        [TestMethod]
        public void GetAllMatches()
        {
            // Arrange
            var match1 = new Match(1, 1, 0, 0, DateTime.Now, Status.Open);
            var match2 = new Match(2, 1, 0, 0, DateTime.Now, Status.Open);
            matches.Add(match1);
            matches.Add(match2);

            // Act
            var result = MatchManager.GetAllMatches();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, match1);
            CollectionAssert.Contains(result, match2);
        }

        [TestMethod]
        public void TestRemoveMatch()
        {
            // Arrange
            var match = new Match(1, 1, 0, 0, DateTime.Now, Status.Open);
            matches.Add(match);

            // Act
            MatchManager.RemoveMatch(match);

            // Assert
            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void TestUpdateMatch()
        {
            // Arrange
            var match = new Match(1, 1, 0, 0, DateTime.Now, Status.Open);
            matches.Add(match);

            // Act
            MatchManager.UpdateMatch(match);

            // Assert
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual(match, matches[0]);
        }
    }
}
