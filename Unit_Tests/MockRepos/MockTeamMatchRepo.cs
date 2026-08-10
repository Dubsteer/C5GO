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

        public void AddTeamMatch(TeamMatch match) => Matches.Add(match);

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
