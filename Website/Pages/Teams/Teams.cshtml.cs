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
                throw new Exception("Already in a team.");

            teamManager.CreateTeam(teamName, CurrentUser.Id.Value);

            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostJoin(int teamId)
        {
            LoadUser();
            teamManager.RequestJoinTeam(teamId, CurrentUser.Id.Value);
            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostApprove(int requestId)
        {
            LoadUser();
            teamManager.ApproveRequest(requestId, CurrentUser.Id.Value);
            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostReject(int requestId)
        {
            LoadUser();
            teamManager.RejectRequest(requestId, CurrentUser.Id.Value);
            return RedirectToPage("/Teams/Teams");
        }
    }
}
