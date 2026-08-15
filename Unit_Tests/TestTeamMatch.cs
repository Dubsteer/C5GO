using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestTeamMatch
    {
        private List<TeamMatch> matches = null!;
        private TeamMatchManager manager = null!;

        [TestInitialize]
        public void Setup()
        {
            matches = new List<TeamMatch>();
            manager = new TeamMatchManager(new MockTeamMatchRepo(matches));
        }

        [TestMethod]
        public void TestGenerateTeamBracketCreatesOpeningRound()
        {
            manager.GenerateTeamBracket(Enumerable.Range(1, 8).ToList(), 10);

            Assert.AreEqual(4, matches.Count);
            Assert.IsTrue(matches.All(match => match.TournamentId == 10));
            Assert.IsTrue(matches.All(match => match.RoundNumber == 1));
            CollectionAssert.AreEqual(
                new[] { 1, 2, 3, 4 },
                matches.OrderBy(match => match.BracketPosition).Select(match => match.BracketPosition).ToArray());
            Assert.AreEqual(8, matches.SelectMany(match => new[] { match.Team1Id, match.Team2Id }).Distinct().Count());
        }

        [TestMethod]
        public void TestInvalidTeamCountDoesNotDeleteExistingBracket()
        {
            matches.Add(CreateMatch(1));

            Assert.ThrowsExactly<InvalidOperationException>(
                () => manager.GenerateTeamBracket(Enumerable.Range(1, 6).ToList(), 1, true));
            Assert.AreEqual(1, matches.Count);
        }

        [TestMethod]
        public void TestExistingBracketRequiresExplicitReplacement()
        {
            matches.Add(CreateMatch(1));

            Assert.ThrowsExactly<InvalidOperationException>(
                () => manager.GenerateTeamBracket(Enumerable.Range(1, 8).ToList(), 1));
            Assert.AreEqual(1, matches.Count);
        }

        [TestMethod]
        public void TestUpdateTeamResult()
        {
            var match = CreateMatch(1);
            matches.Add(match);

            manager.UpdateResult(1, 1, 13, 7, Status.Closed, DateTime.Now.AddHours(1));

            Assert.AreEqual(13, match.Team1Score);
            Assert.AreEqual(7, match.Team2Score);
            Assert.AreEqual(Status.Closed, match.Status);
        }

        [TestMethod]
        public void TestClosedTeamResultRequiresWinner()
        {
            matches.Add(CreateMatch(1));

            Assert.ThrowsExactly<InvalidOperationException>(
                () => manager.UpdateResult(1, 1, 10, 10, Status.Closed, DateTime.Now));
        }

        [TestMethod]
        public void TestTeamHistoryContainsOnlyTeamCompletedMatches()
        {
            var recent = CreateMatch(1);
            recent.Status = Status.Closed;
            recent.MatchDate = DateTime.Now.AddDays(-1);
            var older = CreateMatch(2);
            older.Status = Status.Closed;
            older.MatchDate = DateTime.Now.AddDays(-2);
            var otherTeamMatch = new TeamMatch(
                3,
                1,
                new Team(3, "Other One", null),
                new Team(4, "Other Two", null),
                13,
                9,
                DateTime.Now,
                Status.Closed);
            matches.AddRange([older, otherTeamMatch, recent]);

            var result = manager.GetCompletedMatchesForTeam(1, 1);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(recent.Id, result[0].Id);
        }

        [TestMethod]
        public void TestRecentTeamResultsExcludeOpenMatches()
        {
            var closed = CreateMatch(1);
            closed.Status = Status.Closed;
            matches.AddRange([CreateMatch(2), closed]);

            var result = manager.GetRecentCompletedMatches();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(closed.Id, result[0].Id);
        }

        private static TeamMatch CreateMatch(int id) => new(
            id,
            1,
            new Team(1, "Team One", null!),
            new Team(2, "Team Two", null!),
            0,
            0,
            DateTime.Now,
            Status.Open);
    }
}
