using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestTournament
    {
        private static List<Tournament> tournaments = null!;
        private static List<Match> matches = null!;
        private static TournamentManager tournamentManager = null!;

        [ClassInitialize]
        public static void TestClassSetup(TestContext _)
        {
            tournaments = new List<Tournament>();
            matches = new List<Match>();
            var matchManager = new MatchManager(new MockMatchRepo(matches));
            var teamMatchManager = new TeamMatchManager(new MockTeamMatchRepo());
            tournamentManager = new TournamentManager(
                new MockTournamentRepo(tournaments),
                matchManager,
                teamMatchManager);
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
            var tournament = CreateTournament(1, "Tournament 1");

            tournamentManager.AddTournament(tournament);

            Assert.AreEqual(1, tournaments.Count);
            Assert.AreSame(tournament, tournaments[0]);
        }

        [TestMethod]
        public void TestGetTournamentById()
        {
            var tournament = CreateTournament(1, "Tournament 1");
            tournaments.Add(tournament);

            var result = tournamentManager.GetTournamentById(1);

            Assert.AreSame(tournament, result);
        }

        [TestMethod]
        public void TestGetAllTournaments()
        {
            var tournament1 = CreateTournament(1, "Tournament 1");
            var tournament2 = CreateTournament(2, "Tournament 2");
            tournaments.AddRange(new[] { tournament1, tournament2 });

            var result = tournamentManager.GetAllTournaments();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, tournament1);
            CollectionAssert.Contains(result, tournament2);
        }

        [TestMethod]
        public void TestRemoveTournament()
        {
            var tournament = CreateTournament(1, "Tournament 1");
            tournaments.Add(tournament);

            tournamentManager.RemoveTournament(tournament);

            Assert.AreEqual(0, tournaments.Count);
        }

        [TestMethod]
        public void TestGenerateSoloBracket()
        {
            var players = new List<Player>
            {
                CreatePlayer(1),
                CreatePlayer(2),
                CreatePlayer(3),
                CreatePlayer(4)
            };
            var tournament = CreateTournament(1, "Test Tournament");
            tournaments.Add(tournament);

            tournamentManager.GenerateSoloBracket(players, tournament);

            Assert.AreEqual(2, tournamentManager.GetAllMatchesInTournament(tournament).Count);
            Assert.AreEqual(Status.InProgress, tournament.Status);
        }

        [TestMethod]
        public void TestCreateTournamentRejectsBlankName()
        {
            var tournament = CreateTournament(1, " ");

            Assert.ThrowsExactly<InvalidOperationException>(
                () => tournamentManager.AddTournament(tournament));
            Assert.AreEqual(0, tournaments.Count);
        }

        [TestMethod]
        public void TestSoloBracketRequiresEvenPlayerCount()
        {
            var tournament = CreateTournament(1, "Tournament");
            tournaments.Add(tournament);
            var players = new List<Player> { CreatePlayer(1), CreatePlayer(2), CreatePlayer(3) };

            Assert.ThrowsExactly<InvalidOperationException>(
                () => tournamentManager.GenerateSoloBracket(players, tournament));
            Assert.AreEqual(0, matches.Count);
        }

        [TestMethod]
        public void TestParticipantCannotBeRemovedAfterBracketGeneration()
        {
            var tournament = CreateTournament(1, "Tournament");
            var player1 = CreatePlayer(1);
            var player2 = CreatePlayer(2);
            tournament.Players.AddRange(new[] { player1, player2 });
            tournaments.Add(tournament);
            tournamentManager.GenerateSoloBracket(tournament.Players, tournament);

            Assert.ThrowsExactly<InvalidOperationException>(
                () => tournamentManager.RemovePlayerFromTournament(player1, tournament));
            Assert.AreEqual(2, tournament.Players.Count);
        }

        [TestMethod]
        public void TestDuplicateSoloRegistrationIsRejected()
        {
            var tournament = CreateTournament(1, "Tournament");
            var player = CreatePlayer(1);
            tournament.Players.Add(player);
            tournaments.Add(tournament);

            Assert.ThrowsExactly<InvalidOperationException>(
                () => tournamentManager.AddTournamentApp(player, tournament));
            Assert.AreEqual(1, tournament.Players.Count);
        }

        [TestMethod]
        public void TestRegistrationIsRejectedWhenTournamentIsClosed()
        {
            var tournament = CreateTournament(1, "Tournament");
            tournament.Status = Status.Closed;
            tournaments.Add(tournament);

            Assert.ThrowsExactly<InvalidOperationException>(
                () => tournamentManager.AddTournamentApp(CreatePlayer(1), tournament));
            Assert.AreEqual(0, tournament.Players.Count);
        }

        [TestMethod]
        public void TestGetAllMatchesInTournament()
        {
            var tournament = CreateTournament(1, "Tournament 1");
            var player1 = CreatePlayer(1);
            var player2 = CreatePlayer(2);
            var player3 = CreatePlayer(3);
            var player4 = CreatePlayer(4);
            var match1 = new Match(1, 1, player1, player2, 0, 0, DateTime.Now, Status.InProgress);
            var match2 = new Match(2, 1, player3, player4, 0, 0, DateTime.Now, Status.InProgress);
            matches.AddRange(new[] { match1, match2 });

            var result = tournamentManager.GetAllMatchesInTournament(tournament);

            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, match1);
            CollectionAssert.Contains(result, match2);
        }

        private static Tournament CreateTournament(int id, string name) => new()
        {
            Id = id,
            Name = name,
            Description = $"{name} description",
            Status = Status.Open
        };

        private static Player CreatePlayer(int id) =>
            new Player(id, "Player", id.ToString(), 20, $"player{id}", $"player{id}@test.local", "password", $"steam{id}", false);
    }
}
