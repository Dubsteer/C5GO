using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Website.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly TournamentManager tournamentManager;
        private readonly TeamManager teamManager;

        public Tournament Tournament { get; set; }

        // SOLO
        public List<Match> SoloMatches { get; set; } = new();
        public List<Player> SoloPlayers { get; set; } = new();

        // TEAMS
        public List<TeamMatch> TeamMatches { get; set; } = new();
        public Dictionary<int, Team> TeamsById { get; set; } = new();

        public DetailsModel(TournamentManager tournamentManager, TeamManager teamManager)
        {
            this.tournamentManager = tournamentManager;
            this.teamManager = teamManager;
        }

        public IActionResult OnGet(int id)
        {
            Tournament = tournamentManager.GetTournamentById(id);
            if (Tournament == null)
                return NotFound();

            // ======================================
            // SOLO TOURNAMENT
            // ======================================
            if (!Tournament.IsTeamTournament)
            {
                // LOAD SOLO PLAYERS
                SoloPlayers = tournamentManager.GetAllPlayersInTournament(Tournament)
                                               .OrderBy(p => p.Username)
                                               .ToList();

                Tournament.Players = SoloPlayers;

                // LOAD SOLO MATCHES
                SoloMatches = tournamentManager.GetAllMatchesInTournament(Tournament)
                                               .OrderBy(m => m.MatchDate)
                                               .ToList();
            }

            // ======================================
            // TEAM TOURNAMENT
            // ======================================
            else
            {
                // LOAD TEAM MATCHES
                TeamMatches = tournamentManager.GetAllTeamMatchesInTournament(Tournament)
                                               .OrderBy(m => m.MatchDate)
                                               .ToList();

                // LOAD TEAMS
                var teamIds = tournamentManager.GetTeamsInTournament(Tournament);

                foreach (var tid in teamIds)
                {
                    var team = teamManager.GetTeam(tid);
                    if (team != null)
                        TeamsById[tid] = team;
                }
            }

            return Page();
        }
    }
}
