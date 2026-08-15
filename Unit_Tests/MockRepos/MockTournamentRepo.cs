using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockTournamentRepo : ITournamentRepo
    {
        public List<Tournament> Tournaments { get; }

        public MockTournamentRepo(List<Tournament> tournaments)
        {
            Tournaments = tournaments;
        }

        public List<Tournament> GetAllTournaments() => Tournaments;

        public Tournament GetTournamentById(int id) =>
            Tournaments.FirstOrDefault(t => t.Id == id)!;

        public void AddTournament(Tournament tournament) => Tournaments.Add(tournament);

        public void UpdateTournament(Tournament tournament)
        {
            var index = Tournaments.FindIndex(t => t.Id == tournament.Id);
            if (index < 0)
                throw new InvalidOperationException("Tournament not found");

            Tournaments[index] = tournament;
        }

        public void RemoveTournament(Tournament tournament) =>
            Tournaments.RemoveAll(t => t.Id == tournament.Id);

        public void AddTournamentApp(Player player, Tournament tournament)
        {
            var stored = GetTournamentById(tournament.Id);
            stored?.Players.Add(player);
        }

        public void RemovePlayerFromTournament(Player player, Tournament tournament)
        {
            var stored = GetTournamentById(tournament.Id);
            stored?.Players.RemoveAll(p => p.Id == player.Id);
        }

        public List<Player> GetAllPlayersInTournament(int tournamentId) =>
            GetTournamentById(tournamentId)?.Players ?? new List<Player>();

        public bool HasActivePlayerRegistration(int userId) =>
            Tournaments.Any(tournament =>
                tournament.Status != LogicLayer.Enums.Status.Closed &&
                tournament.Players.Any(player => player.Id == userId));

        public bool HasActiveTeamRegistration(int teamId) =>
            Tournaments.Any(tournament =>
                tournament.Status != LogicLayer.Enums.Status.Closed &&
                tournament.TeamIds.Contains(teamId));

        public void AddTeamTournamentApp(int teamId, int tournamentId)
        {
            var tournament = GetTournamentById(tournamentId);
            if (tournament != null && !tournament.TeamIds.Contains(teamId))
                tournament.TeamIds.Add(teamId);
        }

        public List<int> GetTeamApplications(int tournamentId) =>
            GetTournamentById(tournamentId)?.TeamIds ?? new List<int>();

        public void RemoveTeamTournamentApp(int teamId, int tournamentId) =>
            GetTournamentById(tournamentId)?.TeamIds.Remove(teamId);
    }
}
