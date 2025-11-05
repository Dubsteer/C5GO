using LogicLayer.Models;
using LogicLayer.IRepos;
using LogicLayer.Exceptions;


namespace LogicLayer.Managers
{
    public class MatchManager
    {
        private readonly IMatchRepo matchRepo;
        public MatchManager(IMatchRepo matchRepo)
        {
            this.matchRepo = matchRepo;
        }

        public Match GetMatchById(int id)
        {
            foreach (Match match in GetAllMatches())
            {
                if (match.Id == id)
                {
                    return match;
                }
            }
            throw new MatchNotFoundException("Match not found");
        }
        public List<Match> GetAllMatches()
        {
            return matchRepo.GetAllMatches();
        }

        public void AddMatch(Match match)
        {
            foreach (Match matches in GetAllMatches())
            {
                if (matches.Id == match.Id)
                {
                    throw new DuplicateMatchException("Match already exists");
                }
            }
            matchRepo.AddMatch(match);
        }

        public void RemoveMatch(Match match)
        {
            foreach (Match matches in GetAllMatches())
            {
                if(match.Id == match.Id)
                {
                    matchRepo.RemoveMatch(matches);
                    return;
                }
            }
        }

        public void UpdateMatch(Match match)
        {
            matchRepo.UpdateMatch(match);
            return;
        }

        public List<Match> GetFullMatches()
        {
            return matchRepo.GetAllMatches().Select(
                m => new Match(
                    m.Id,
                    m.TournamentId,
                    m.User1,
                    m.User2,
                    m.Player1Score,
                    m.Player2Score,
                    m.MatchDate,
                    m.Status)
                ).ToList();
        }

        public List<Match> GetPastMatches(User user)
        {
            return GetAllMatches()
                .Where(m => m.User1.Id == user.Id || m.User2.Id == user.Id)
                .OrderByDescending(m => m.MatchDate)
                .ToList();
        }
    }
}
