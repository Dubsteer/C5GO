using LogicLayer.Enums;
using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Models;

namespace Website.Pages.Admin.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly TournamentManager tournamentManager;
        private readonly MatchManager matchManager;
        private readonly TeamMatchManager teamMatchManager;
        private readonly TeamManager teamManager;

        public Tournament Tournament { get; private set; } = null!;
        public List<Player> Players { get; private set; } = new();
        public List<Match> SoloMatches { get; private set; } = new();
        public List<Team> RegisteredTeams { get; private set; } = new();
        public List<Team> AvailableTeams { get; private set; } = new();
        public List<TeamMatch> TeamMatches { get; private set; } = new();
        public bool HasBracket => Tournament.IsTeamTournament
            ? TeamMatches.Count > 0
            : SoloMatches.Count > 0;
        public int ParticipantCount => Tournament.IsTeamTournament
            ? RegisteredTeams.Count
            : Players.Count;
        public int LatestRoundNumber => Tournament.IsTeamTournament
            ? TeamMatches.Select(match => match.RoundNumber).DefaultIfEmpty(0).Max()
            : SoloMatches.Select(match => match.RoundNumber).DefaultIfEmpty(0).Max();
        public Player? SoloChampion => GetSoloChampion();
        public Team? TeamChampion => GetTeamChampion();

        public DetailsModel(
            TournamentManager tournamentManager,
            MatchManager matchManager,
            TeamMatchManager teamMatchManager,
            TeamManager teamManager)
        {
            this.tournamentManager = tournamentManager;
            this.matchManager = matchManager;
            this.teamMatchManager = teamMatchManager;
            this.teamManager = teamManager;
        }

        public IActionResult OnGet(int id)
        {
            return LoadPage(id) ? Page() : NotFound();
        }

        public IActionResult OnPostUpdateSettings(
            int id,
            string name,
            string description,
            Status status)
        {
            try
            {
                tournamentManager.UpdateTournament(id, name, description, status);
                TempData["SuccessMessage"] = "Tournament settings updated.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "Tournament settings could not be updated.";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostAddTeam(int id, int teamId)
        {
            try
            {
                var team = teamManager.GetTeam(teamId)
                    ?? throw new InvalidOperationException("Team was not found.");

                tournamentManager.AddTeamToTournament(team.Id, id);
                TempData["SuccessMessage"] = $"{team.Name} added to the tournament.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The team could not be added.";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRemoveTeam(int id, int teamId)
        {
            try
            {
                tournamentManager.RemoveTeamFromTournament(teamId, id);
                TempData["SuccessMessage"] = "Team removed from the tournament.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The team could not be removed.";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostRemovePlayer(int id, int playerId)
        {
            try
            {
                var tournament = tournamentManager.GetTournamentById(id);
                var player = tournamentManager.GetAllPlayersInTournament(tournament)
                    .FirstOrDefault(candidate => candidate.Id == playerId)
                    ?? throw new InvalidOperationException("Player is not registered for this tournament.");

                tournamentManager.RemovePlayerFromTournament(player, tournament);
                TempData["SuccessMessage"] = "Player removed from the tournament.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The player could not be removed.";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostGenerateBracket(int id, bool replaceExisting = false)
        {
            try
            {
                var tournament = tournamentManager.GetTournamentById(id);

                if (tournament.IsTeamTournament)
                {
                    var teamIds = tournamentManager.GetTeamsInTournament(tournament);
                    var teams = teamIds
                        .Select(teamManager.GetTeam)
                        .Where(team => team != null)
                        .Cast<Team>()
                        .ToList();

                    if (teams.Count != teamIds.Count)
                        throw new InvalidOperationException("One or more registered teams no longer exist.");

                    var incompleteTeams = teams
                        .Where(team => team.Members.Count < tournament.TeamSizeRequired)
                        .Select(team => team.Name)
                        .ToList();

                    if (incompleteTeams.Count > 0)
                    {
                        throw new InvalidOperationException(
                            $"Complete these teams before generating the bracket: {string.Join(", ", incompleteTeams)}.");
                    }

                    tournamentManager.GenerateTeamBracket(teamIds, tournament, replaceExisting);
                }
                else
                {
                    var players = tournamentManager.GetAllPlayersInTournament(tournament);
                    tournamentManager.GenerateSoloBracket(players, tournament, replaceExisting);
                }

                TempData["SuccessMessage"] = replaceExisting
                    ? "Bracket regenerated. Previous matches were removed."
                    : "Full tournament bracket generated.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The bracket could not be generated.";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostUpdateSoloMatch(
            int id,
            int matchId,
            int score1,
            int score2,
            Status matchStatus,
            DateTime matchDate)
        {
            try
            {
                matchManager.UpdateResult(matchId, id, score1, score2, matchStatus, matchDate);
                RefreshTournamentStatus(id);
                TempData["SuccessMessage"] = "Match result updated.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The match result could not be updated.";
            }

            return RedirectToPage(new { id });
        }

        public IActionResult OnPostUpdateTeamMatch(
            int id,
            int matchId,
            int score1,
            int score2,
            Status matchStatus,
            DateTime matchDate)
        {
            try
            {
                teamMatchManager.UpdateResult(matchId, id, score1, score2, matchStatus, matchDate);
                RefreshTournamentStatus(id);
                TempData["SuccessMessage"] = "Team match result updated.";
            }
            catch (TournamentNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The team match result could not be updated.";
            }

            return RedirectToPage(new { id });
        }

        private bool LoadPage(int id)
        {
            try
            {
                Tournament = tournamentManager.GetTournamentById(id);
            }
            catch (TournamentNotFoundException)
            {
                return false;
            }

            if (Tournament.IsTeamTournament)
            {
                var registeredIds = tournamentManager.GetTeamsInTournament(Tournament);
                var allTeams = teamManager.GetAllTeams();
                RegisteredTeams = allTeams
                    .Where(team => registeredIds.Contains(team.Id))
                    .OrderBy(team => team.Name)
                    .ToList();
                AvailableTeams = allTeams
                    .Where(team => !registeredIds.Contains(team.Id))
                    .OrderBy(team => team.Name)
                    .ToList();
                TeamMatches = teamMatchManager.GetTeamMatchesByTournament(id)
                    .OrderBy(match => match.RoundNumber)
                    .ThenBy(match => match.BracketPosition)
                    .ToList();
            }
            else
            {
                Players = tournamentManager.GetAllPlayersInTournament(Tournament)
                    .OrderBy(player => player.Username)
                    .ToList();
                SoloMatches = matchManager.GetMatchesByTournamentId(id)
                    .OrderBy(match => match.RoundNumber)
                    .ThenBy(match => match.BracketPosition)
                    .ToList();
            }

            return true;
        }

        private void RefreshTournamentStatus(int id)
        {
            var tournament = tournamentManager.GetTournamentById(id);
            tournamentManager.UpdateTournamentStatus(tournament);
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
