using LogicLayer.Models;
using System.Collections.Generic;

namespace LogicLayer.IRepos
{
    public interface ITeamMatchRepo
    {
        List<TeamMatch> GetAllTeamMatches();
        void AddTeamMatch(TeamMatch match);
        void UpdateTeamMatch(TeamMatch match);
        void RemoveTeamMatch(TeamMatch match);
    }
}
