using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Website.Pages.Teams
{
    [Authorize]
    public class TeamsModel : PageModel
    {
        private readonly TeamManager teamManager;
        private readonly UserManager userManager;

        public List<Team> AllTeams { get; set; } = new();
        public Team MyTeam { get; set; }
        public User CurrentUser { get; set; }
        public List<int> PendingRequests { get; set; } = new();

        public TeamManager TeamManager => teamManager;

        public TeamsModel(TeamManager tm, UserManager um)
        {
            teamManager = tm;
            userManager = um;
        }

        private void LoadUser()
        {
            var id = int.Parse(User.FindFirst("id").Value);
            CurrentUser = userManager.GetUserById(id);
            MyTeam = teamManager.GetTeamOfUser(id);
        }

        private void LoadRequests()
        {
            if (CurrentUser?.Id == null)
                return;

            PendingRequests = teamManager
                .GetRequestsForUser(CurrentUser.Id.Value)
                .Select(r => r.TeamId)
                .ToList();
        }

        public IActionResult OnGet()
        {
            LoadUser();
            LoadRequests();

            AllTeams = teamManager.GetAllTeams();
            return Page();
        }

        public IActionResult OnPostCreateTeam(string teamName)
        {
            LoadUser();

            if (MyTeam != null)
            {
                TempData["Error"] = "You are already in a team.";
                return RedirectToPage("/Teams/Teams");
            }

            if (string.IsNullOrWhiteSpace(CurrentUser.SteamId) || CurrentUser.SteamId == "0")
            {
                TempData["Error"] = "You must add your SteamID before creating a team.";
                return Redirect("/ViewProfile");
            }

            teamManager.CreateTeam(teamName, CurrentUser.Id.Value);

            TempData["Message"] = "Team created!";
            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostJoin(int teamId)
        {
            LoadUser();

            // ? HARD BLOCK — NULL ILI "0"
            if (string.IsNullOrWhiteSpace(CurrentUser.SteamId) || CurrentUser.SteamId == "0")
            {
                TempData["Error"] = "You must add your SteamID before requesting to join a team.";
                return RedirectToPage("/Teams/Teams");
            }

            try
            {
                teamManager.RequestJoinTeam(teamId, CurrentUser.Id.Value);
                TempData["Message"] = "Join request sent!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Teams");
        }



        public IActionResult OnPostApprove(int requestId)
        {
            LoadUser();

            try
            {
                teamManager.ApproveRequest(requestId, CurrentUser.Id.Value);
                TempData["Message"] = "Player approved!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostReject(int requestId)
        {
            LoadUser();

            try
            {
                teamManager.RejectRequest(requestId, CurrentUser.Id.Value);
                TempData["Message"] = "Request rejected.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Teams");
        }
    }
}
