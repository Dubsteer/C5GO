using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.Managers
{
    public class TeamMatchManager
    {
        private readonly ITeamMatchRepo repo;

        public TeamMatchManager(ITeamMatchRepo repo)
        {
            this.repo = repo;
        }

        // ================================
        // BASIC GETTERS
        // ================================
        public List<TeamMatch> GetAllTeamMatches()
        {
            return repo.GetAllTeamMatches();
        }

        public List<TeamMatch> GetTeamMatchesByTournament(int tournamentId)
        {
            return repo.GetAllTeamMatches()
                       .Where(m => m.TournamentId == tournamentId)
                       .ToList();
        }

        // ================================
        // ADD / UPDATE / REMOVE
        // ================================
        public void AddTeamMatch(TeamMatch match)
        {
            repo.AddTeamMatch(match);
        }

        public void UpdateTeamMatch(TeamMatch match)
        {
            repo.UpdateTeamMatch(match);
        }

        public void RemoveTeamMatch(TeamMatch match)
        {
            repo.RemoveTeamMatch(match);
        }

        // ================================
        // GENERATE TEAM BRACKET
        // ================================
        public void GenerateTeamBracket(List<int> teamIds, int tournamentId)
        {
            // CLEAN OLD MATCHES
            var existing = GetTeamMatchesByTournament(tournamentId);
            foreach (var m in existing)
                RemoveTeamMatch(m);

            // SHUFFLE TEAMS
            var rnd = new Random();
            var shuffled = teamIds.OrderBy(x => rnd.Next()).ToList();

            // ❗ TURNIR MORA BITI 8, 12 ILI 16 TIMOVA
            if (!(shuffled.Count == 8 || shuffled.Count == 12 || shuffled.Count == 16))
                throw new Exception("Team bracket requires 8, 12 or 16 teams.");

            // PAIRING (ROUND 1)
            for (int i = 0; i < shuffled.Count - 1; i += 2)
            {
                var match = new TeamMatch(
                    0,
                    tournamentId,
                    new Team(shuffled[i], "", null),
                    new Team(shuffled[i + 1], "", null),
                    0,
                    0,
                    DateTime.Now,
                    Status.Open
                );

                AddTeamMatch(match);
            }
        }
    }
}
