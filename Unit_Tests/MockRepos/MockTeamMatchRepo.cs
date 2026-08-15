using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockTeamMatchRepo : ITeamMatchRepo
    {
        public List<TeamMatch> Matches { get; }

        public MockTeamMatchRepo(List<TeamMatch>? matches = null)
        {
            Matches = matches ?? new List<TeamMatch>();
        }

        public List<TeamMatch> GetAllTeamMatches() => Matches;

        public void AddTeamMatch(TeamMatch match)
        {
            if (match.Id == 0)
                match.Id = Matches.Count == 0 ? 1 : Matches.Max(existing => existing.Id) + 1;
            Matches.Add(match);
        }

        public void UpdateTeamMatch(TeamMatch match)
        {
            var index = Matches.FindIndex(m => m.Id == match.Id);
            if (index >= 0)
                Matches[index] = match;
        }

        public void RemoveTeamMatch(TeamMatch match) =>
            Matches.RemoveAll(m => m.Id == match.Id);
    }
}
