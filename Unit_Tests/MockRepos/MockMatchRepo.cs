using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer.IRepos;
using LogicLayer.Models;


namespace Unit_Tests.MockRepos
{
    public class MockMatchRepo : IMatchRepo
    {
        public List<Match> Matches = new();

        public MockMatchRepo(List<Match> matches) 
        {
            Matches = matches;
        }

        public void AddMatch(Match match)
        {
            Matches.Add(match);
        }

        public List<Match> GetAllMatches()
        {
            return Matches;
        }

        public void RemoveMatch(Match match)
        {
            foreach (var m in Matches.ToArray())
            {
                if (m.Id == match.Id)
                {
                    Matches.Remove(m);
                }
            }
        }

        public void UpdateMatch(Match match)
        {
            foreach (var m in Matches.ToArray())
            {
                if (m.Id == match.Id)
                {
                    Matches.Remove(m);
                }
            }
            Matches.Add(match);
        }
    }
}
