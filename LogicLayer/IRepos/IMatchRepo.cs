using LogicLayer.Models;

namespace LogicLayer.IRepos
{
    public interface IMatchRepo
    {
        public List<Match> GetAllMatches();
        public void AddMatch(Match match);
        public void RemoveMatch(Match match);
        public void UpdateMatch(Match match);
    }
}
