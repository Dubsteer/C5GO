using LogicLayer.IRepos;
using LogicLayer.Models;

namespace Unit_Tests.MockRepos
{
    public class MockTeamMatchRepo : ITeamMatchRepo
    {
        private readonly List<TeamMatch> matches = new();

        public List<TeamMatch> GetAllTeamMatches() => matches;

        public void AddTeamMatch(TeamMatch match) => matches.Add(match);

        public void UpdateTeamMatch(TeamMatch match)
        {
            var index = matches.FindIndex(m => m.Id == match.Id);
            if (index >= 0)
                matches[index] = match;
        }

        public void RemoveTeamMatch(TeamMatch match) =>
            matches.RemoveAll(m => m.Id == match.Id);
    }
}
