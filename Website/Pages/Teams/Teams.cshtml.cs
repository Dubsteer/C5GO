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

        public List<Team> AllTeams { get; set; } = [];
        public Team? MyTeam { get; set; }
        public User CurrentUser { get; set; } = null!;
        public List<int> PendingRequests { get; set; } = [];

        public TeamManager TeamManager => teamManager;

        public TeamsModel(TeamManager tm, UserManager um)
        {
            teamManager = tm;
            userManager = um;
        }

        private int? LoadUser()
        {
            if (!int.TryParse(User.FindFirst("id")?.Value, out var userId))
                return null;

            var user = userManager.GetUserById(userId);
            if (user?.Id is not int currentUserId)
                return null;

            CurrentUser = user;
            MyTeam = teamManager.GetTeamOfUser(currentUserId);
            return currentUserId;
        }

        private void LoadRequests(int userId)
        {
            PendingRequests = teamManager
                .GetRequestsForUser(userId)
                .Select(r => r.TeamId)
                .ToList();
        }

        public IActionResult OnGet()
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            LoadRequests(currentUserId);

            AllTeams = teamManager.GetAllTeams();
            return Page();
        }

        public IActionResult OnPostCreateTeam(string teamName)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

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

            teamManager.CreateTeam(teamName, currentUserId);

            TempData["Message"] = "Team created!";
            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostJoin(int teamId)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            if (string.IsNullOrWhiteSpace(CurrentUser.SteamId) || CurrentUser.SteamId == "0")
            {
                TempData["Error"] = "You must add your SteamID before requesting to join a team.";
                return RedirectToPage("/Teams/Teams");
            }

            try
            {
                teamManager.RequestJoinTeam(teamId, currentUserId);
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
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.ApproveRequest(requestId, currentUserId);
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
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.RejectRequest(requestId, currentUserId);
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
