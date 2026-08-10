using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Website.Pages
{
    [Authorize]
    public class TournamentsModel : PageModel
    {
        public List<Tournament> Tournaments { get; set; } = new();
        public Player? CurrentPlayer { get; set; }
        public Team? MyTeam { get; set; }

        public string? Message { get; set; }
        public string? Error { get; set; }

        public string Filter { get; set; } = "all";
        public bool IsPlayer => CurrentPlayer != null;

        private readonly UserManager userManager;
        private readonly TournamentManager tournamentManager;
        private readonly PlayerManager playerManager;
        private readonly TeamManager teamManager;

        public TournamentsModel(
            TournamentManager tournamentManager,
            PlayerManager playerManager,
            UserManager userManager,
            TeamManager teamManager)
        {
            this.tournamentManager = tournamentManager;
            this.playerManager = playerManager;
            this.userManager = userManager;
            this.teamManager = teamManager;
        }

        private void LoadCurrent()
        {
            CurrentPlayer = null;
            MyTeam = null;

            var claim = User.FindFirst("id")?.Value;
            if (!int.TryParse(claim, out var userId))
                return;

            try
            {
                var user = userManager.GetUserById(userId);

                if (user == null || string.IsNullOrWhiteSpace(user.SteamId) || user.SteamId == "0")
                    return;

                CurrentPlayer = playerManager.GetPlayer(user);
                MyTeam = teamManager.GetTeamOfUser(userId);
            }
            catch
            {
                CurrentPlayer = null;
            }
        }

        public IActionResult OnGet(string filter = "all")
        {
            LoadCurrent();
            Filter = filter;

            if (TempData.ContainsKey("Message"))
                Message = TempData["Message"]?.ToString();

            if (TempData.ContainsKey("Error"))
                Error = TempData["Error"]?.ToString();

            Tournaments = tournamentManager.GetAllTournaments();

            foreach (var t in Tournaments)
            {
                t.Players = tournamentManager.GetAllPlayersInTournament(t);
                t.Matches = tournamentManager.GetAllMatchesInTournament(t);

                t.TeamIds = tournamentManager.GetTeamsInTournament(t);
                t.Teams = new List<Team>();

                foreach (var tid in t.TeamIds)
                {
                    var team = teamManager.GetTeam(tid);
                    if (team != null)
                        t.Teams.Add(team);
                }

                if (CurrentPlayer != null)
                {
                    if (!t.IsTeamTournament)
                        t.CanLeave = t.Players.Any(p => p.Id == CurrentPlayer.Id);
                    else if (MyTeam != null)
                        t.CanLeave = t.TeamIds.Contains(MyTeam.Id);
                }
            }

            Tournaments = filter switch
            {
                "solo" => Tournaments.Where(t => !t.IsTeamTournament).ToList(),
                "team" => Tournaments.Where(t => t.IsTeamTournament).ToList(),
                _ => Tournaments
            };

            return Page();
        }

        public IActionResult OnPostApplySolo(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
            {
                TempData["RequireSteam"] = true;
                return RedirectToPage("/ViewProfile");
            }

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.AddTournamentApp(CurrentPlayer!, t);
                TempData["Message"] = "Successfully joined!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "The tournament registration could not be completed.";
            }

            return RedirectToPage("/Tournaments");
        }

        public IActionResult OnPostApplyTeam(int id)
        {
            LoadCurrent();

            if (MyTeam == null)
            {
                TempData["Error"] = "Create or join a complete team before registering for a team tournament.";
                return RedirectToPage("/Teams/Teams");
            }

            if (MyTeam.Captain.Id != CurrentPlayer?.Id)
            {
                TempData["Error"] = "Only the team captain can register the team.";
                return RedirectToPage("/Tournaments");
            }

            try
            {
                var tournament = tournamentManager.GetTournamentById(id);
                if (MyTeam.Members.Count < tournament.TeamSizeRequired)
                {
                    TempData["Error"] = $"Your team needs {tournament.TeamSizeRequired} players before registering.";
                    return RedirectToPage("/Tournaments");
                }

                tournamentManager.AddTeamToTournament(MyTeam.Id, tournament.Id);
                TempData["Message"] = "Team registered successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "The team could not be registered.";
            }

            return RedirectToPage("/Tournaments");
        }

        public IActionResult OnPostLeave(int id)
        {
            LoadCurrent();

            try
            {
                var tournament = tournamentManager.GetTournamentById(id);

                if (tournament.IsTeamTournament)
                {
                    if (MyTeam == null || MyTeam.Captain.Id != CurrentPlayer?.Id)
                        throw new InvalidOperationException("Only the team captain can withdraw the team.");

                    tournamentManager.RemoveTeamFromTournament(MyTeam.Id, tournament.Id);
                    TempData["Message"] = "Team withdrawn from the tournament.";
                }
                else
                {
                    if (CurrentPlayer == null)
                        throw new InvalidOperationException("Player profile was not found.");

                    tournamentManager.RemovePlayerFromTournament(CurrentPlayer, tournament);
                    TempData["Message"] = "You left the tournament.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch
            {
                TempData["Error"] = "The tournament registration could not be changed.";
            }

            return RedirectToPage("/Tournaments");
        }
    }
}
