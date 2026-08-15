using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestMatch
    {
        private static List<Match> matches = null!;
        private static MatchManager matchManager = null!;

        [ClassInitialize]
        public static void TestClassSetup(TestContext _)
        {
            matches = new List<Match>();
            matchManager = new MatchManager(new MockMatchRepo(matches));
        }

        [TestInitialize]
        public void Setup() => matches.Clear();

        [TestMethod]
        public void TestAddMatch()
        {
            var match = CreateMatch(1);

            matchManager.AddMatch(match);

            Assert.AreEqual(1, matches.Count);
            Assert.AreSame(match, matches[0]);
        }

        [TestMethod]
        public void TestGetAllMatches()
        {
            var match1 = CreateMatch(1);
            var match2 = CreateMatch(2);
            matches.AddRange(new[] { match1, match2 });

            var result = matchManager.GetAllMatches();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, match1);
            CollectionAssert.Contains(result, match2);
        }

        [TestMethod]
        public void TestRemoveMatch()
        {
            var match = CreateMatch(1);
            matches.Add(match);

            matchManager.RemoveMatch(match);

            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void TestUpdateResult()
        {
            var match = CreateMatch(1);
            matches.Add(match);
            var matchDate = DateTime.Now.AddDays(1);

            matchManager.UpdateResult(1, 1, 13, 8, Status.Closed, matchDate);

            Assert.AreEqual(13, match.Player1Score);
            Assert.AreEqual(8, match.Player2Score);
            Assert.AreEqual(Status.Closed, match.Status);
            Assert.AreEqual(matchDate, match.MatchDate);
        }

        [TestMethod]
        public void TestClosedResultRequiresWinner()
        {
            matches.Add(CreateMatch(1));

            Assert.ThrowsExactly<InvalidOperationException>(
                () => matchManager.UpdateResult(1, 1, 13, 13, Status.Closed, DateTime.Now));
        }

        [TestMethod]
        public void TestResultCannotBeMovedToAnotherTournament()
        {
            matches.Add(CreateMatch(1));

            Assert.ThrowsExactly<InvalidOperationException>(
                () => matchManager.UpdateResult(1, 2, 13, 8, Status.Closed, DateTime.Now));
        }

        [TestMethod]
        public void TestPlayerHistoryContainsOnlyCompletedMatchesInNewestOrder()
        {
            var recent = CreateMatch(1);
            recent.Status = Status.Closed;
            recent.MatchDate = DateTime.Now.AddDays(-1);
            var older = CreateMatch(2);
            older.Status = Status.Closed;
            older.MatchDate = DateTime.Now.AddDays(-3);
            var open = CreateMatch(3);
            open.MatchDate = DateTime.Now;
            matches.AddRange([older, open, recent]);

            var result = matchManager.GetPastMatches(recent.User1, 1);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(recent.Id, result[0].Id);
        }

        [TestMethod]
        public void TestRecentCompletedMatchesExcludeOpenMatches()
        {
            var closed = CreateMatch(1);
            closed.Status = Status.Closed;
            matches.AddRange([CreateMatch(2), closed]);

            var result = matchManager.GetRecentCompletedMatches();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(closed.Id, result[0].Id);
        }

        private static Match CreateMatch(int id)
        {
            var player1 = new Player(1, "Player", "One", 20, "player1", "p1@test.local", "password", "steam1", false);
            var player2 = new Player(2, "Player", "Two", 21, "player2", "p2@test.local", "password", "steam2", false);
            return new Match(id, 1, player1, player2, 0, 0, DateTime.Now, Status.Open);
        }
    }
}
