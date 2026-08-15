using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using System.Collections.Generic;
using System.Linq;
using LogicLayer.Enums;
using Website.Models;

namespace Website.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly TournamentManager tournamentManager;
        private readonly TeamManager teamManager;

        public Tournament Tournament { get; set; } = null!;

        public List<Match> SoloMatches { get; set; } = new();
        public List<Player> SoloPlayers { get; set; } = new();

        public List<TeamMatch> TeamMatches { get; set; } = new();
        public Dictionary<int, Team> TeamsById { get; set; } = new();
        public int ParticipantCount => Tournament.IsTeamTournament ? TeamsById.Count : SoloPlayers.Count;
        public Player? SoloChampion => GetSoloChampion();
        public Team? TeamChampion => GetTeamChampion();

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

            if (!Tournament.IsTeamTournament)
            {
                SoloPlayers = tournamentManager.GetAllPlayersInTournament(Tournament)
                                               .OrderBy(p => p.Username)
                                               .ToList();

                Tournament.Players = SoloPlayers;

                SoloMatches = tournamentManager.GetAllMatchesInTournament(Tournament)
                                               .OrderBy(m => m.RoundNumber)
                                               .ThenBy(m => m.BracketPosition)
                                               .ToList();
            }

            else
            {
                TeamMatches = tournamentManager.GetAllTeamMatchesInTournament(Tournament)
                                               .OrderBy(m => m.RoundNumber)
                                               .ThenBy(m => m.BracketPosition)
                                               .ToList();

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

        public string GetRoundName(int roundNumber, int matchCount) =>
            BracketRoundPresentation.GetName(ParticipantCount, roundNumber, matchCount);

        private Player? GetSoloChampion()
        {
            var final = SoloMatches
                .Where(match => match.Status == Status.Closed)
                .OrderByDescending(match => match.RoundNumber)
                .FirstOrDefault();

            if (Tournament.Status != Status.Closed || final == null ||
                SoloMatches.Count(match => match.RoundNumber == final.RoundNumber) != 1)
            {
                return null;
            }

            return final.Player1Score > final.Player2Score ? final.User1 : final.User2;
        }

        private Team? GetTeamChampion()
        {
            var final = TeamMatches
                .Where(match => match.Status == Status.Closed)
                .OrderByDescending(match => match.RoundNumber)
                .FirstOrDefault();

            if (Tournament.Status != Status.Closed || final == null ||
                TeamMatches.Count(match => match.RoundNumber == final.RoundNumber) != 1)
            {
                return null;
            }

            return final.Team1Score > final.Team2Score ? final.Team1 : final.Team2;
        }
    }
}
