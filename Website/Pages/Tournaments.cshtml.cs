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
        public Player CurrentPlayer { get; set; }
        public Team MyTeam { get; set; }

        public string Message { get; set; }
        public string Error { get; set; }

        public string Filter { get; set; } = "all";
        public bool IsPlayer => CurrentPlayer != null;
        public bool IsAdmin { get; set; }

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
            try
            {
                var uid = int.Parse(User.FindFirst("id").Value);
                var user = userManager.GetUserById(uid);

                if (string.IsNullOrWhiteSpace(user.SteamId) || user.SteamId == "0")
                {
                    CurrentPlayer = null;
                    MyTeam = null;
                    return;
                }

                CurrentPlayer = playerManager.GetPlayer(user);
                MyTeam = teamManager.GetTeamOfUser(uid);
            }
            catch
            {
                CurrentPlayer = null;
                MyTeam = null;
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
                // SOLO
                t.Players = tournamentManager.GetAllPlayersInTournament(t);
                t.Matches = tournamentManager.GetAllMatchesInTournament(t);

                // TEAM
                t.TeamIds = tournamentManager.GetTeamsInTournament(t);
                t.Teams = new List<Team>();

                foreach (var tid in t.TeamIds)
                {
                    var team = teamManager.GetTeam(tid);
                    if (team != null)
                        t.Teams.Add(team);
                }

                // Status update
                tournamentManager.UpdateTournamentStatus(t);

                // CAN LEAVE?
                if (CurrentPlayer != null)
                {
                    if (!t.IsTeamTournament)
                    {
                        t.CanLeave = t.Players.Any(p => p.Id == CurrentPlayer.Id);
                    }
                    else if (MyTeam != null)
                    {
                        t.CanLeave = t.TeamIds.Contains(MyTeam.Id);
                    }
                }
            }

            // IS ADMIN?
            IsAdmin = CurrentPlayer != null && CurrentPlayer.IsAdmin;

            // FILTERING
            Tournaments = filter switch
            {
                "solo" => Tournaments.Where(t => !t.IsTeamTournament).ToList(),
                "team" => Tournaments.Where(t => t.IsTeamTournament).ToList(),
                _ => Tournaments
            };

            return Page();
        }

        // SOLO APPLY
        public IActionResult OnPostApplySolo(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
                return RedirectToPage("/ViewProfile");

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                t.Players = tournamentManager.GetAllPlayersInTournament(t);

                if (!t.Players.Any(p => p.Id == CurrentPlayer.Id))
                {
                    tournamentManager.AddTournamentApp(CurrentPlayer, t);
                    TempData["Message"] = "Successfully joined!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Tournaments");
        }

        // TEAM APPLY
        public IActionResult OnPostApplyTeam(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
                return RedirectToPage("/ViewProfile");

            if (MyTeam == null)
                return RedirectToPage("/Teams/Teams");

            if (MyTeam.Captain.Id != CurrentPlayer.Id)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);

                var teamIds = tournamentManager.GetTeamsInTournament(t);
                if (teamIds.Contains(MyTeam.Id))
                    throw new Exception("Team already registered.");

                if (MyTeam.Members.Count != t.TeamSizeRequired)
                    throw new Exception($"Team must have exactly {t.TeamSizeRequired} players.");

                tournamentManager.AddTeamToTournament(MyTeam.Id, id);
                TempData["Message"] = "Team joined!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Tournaments");
        }

        // LEAVE SOLO
        public IActionResult OnPostLeave(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
                return RedirectToPage("/ViewProfile");

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.RemovePlayerFromTournament(CurrentPlayer, t);
                TempData["Message"] = "Left tournament.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Tournaments");
        }

        // LEAVE TEAM
        public IActionResult OnPostLeaveTeam(int id)
        {
            LoadCurrent();

            if (MyTeam == null || MyTeam.Captain.Id != CurrentPlayer.Id)
                return Unauthorized();

            try
            {
                tournamentManager.RemoveTeamFromTournament(MyTeam.Id, id);
                TempData["Message"] = "Team left tournament.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Tournaments");
        }

        // ADMIN CLOSE
        public IActionResult OnPostClose(int id)
        {
            LoadCurrent();

            if (!IsPlayer || !CurrentPlayer.IsAdmin)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.SetStatus(t, LogicLayer.Enums.Status.Closed);
                TempData["Message"] = "Closed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Tournaments");
        }

        // DELETE
        public IActionResult OnPostDelete(int id)
        {
            LoadCurrent();

            if (!IsPlayer || !CurrentPlayer.IsAdmin)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.RemoveTournament(t);
                TempData["Message"] = "Deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Tournaments");
        }
    }
}
