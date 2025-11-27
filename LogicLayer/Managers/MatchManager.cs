using LogicLayer.Exceptions;
using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
            return GetAllMatches().FirstOrDefault(m => m.Id == id)
                ?? throw new MatchNotFoundException("Match not found");
        }

        public List<Match> GetAllMatches()
        {
            return matchRepo.GetAllMatches();
        }

        public List<Match> GetMatchesByTournamentId(int tid)
        {
            return GetAllMatches().Where(m => m.TournamentId == tid).ToList();
        }

        // OVO JE BILO POTREBNO A NIJE POSTOJALO
        public List<Match> GetAllMatchesInTournament(Tournament t)
        {
            return GetMatchesByTournamentId(t.Id);
        }

        public void AddMatch(Match match)
        {
            matchRepo.AddMatch(match);
        }

        public void RemoveMatch(Match match)
        {
            matchRepo.RemoveMatch(match);
        }

        public void UpdateMatch(Match match)
        {
            matchRepo.UpdateMatch(match);
        }

        public List<Match> GetPastMatches(User user)
        {
            return GetAllMatches()
                .Where(m => m.User1.Id == user.Id || m.User2.Id == user.Id)
                .OrderByDescending(m => m.MatchDate)
                .ToList();
        }

        // AUTO MATCH GENERATOR
        public void GenerateMatches(List<Player> players, int tournamentId, DateTime startDate, int rounds)
        {
            var rnd = new Random();
            var shuffled = players.OrderBy(x => rnd.Next()).ToList();

            for (int round = 0; round < rounds; round++)
            {
                for (int i = 0; i < shuffled.Count - 1; i += 2)
                {
                    var match = new Match(
                        0, // id se NE KORISTI jer DB auto-increment radi posao
                        tournamentId,
                        shuffled[i],
                        shuffled[i + 1],
                        0,
                        0,
                        startDate.AddDays(round),
                        Enums.Status.InProgress
                    );

                    AddMatch(match);
                }
            }
        }
    }
}
