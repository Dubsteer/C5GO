using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Teams
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly TeamManager teamManager;
        private readonly UserManager userManager;

        public Team Team { get; set; } = null!;
        public List<User> Members { get; set; } = [];
        public List<TeamJoinRequest> Pending { get; set; } = [];
        public User CurrentUser { get; set; } = null!;

        public bool IsCaptain { get; set; }

        public DetailsModel(TeamManager tm, UserManager um)
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
            return currentUserId;
        }

        public IActionResult OnGet(int id)
        {
            if (LoadUser() is null)
                return Challenge();

            var team = teamManager.GetTeam(id);
            if (team == null)
                return RedirectToPage("/Teams/Teams");

            Team = team;
            Members = Team.Members;
            Pending = teamManager.GetJoinRequests(id);

            IsCaptain = Team.Captain.Id == CurrentUser.Id;

            return Page();
        }

        public IActionResult OnPostApprove(int requestId, int teamId)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.ApproveRequest(requestId, currentUserId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

        public IActionResult OnPostReject(int requestId, int teamId)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.RejectRequest(requestId, currentUserId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

        public IActionResult OnPostLeave(int teamId)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.LeaveTeam(currentUserId);
                TempData["Message"] = "You left the team.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Teams");
        }

        public IActionResult OnPostKick(int userId, int teamId)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.KickMember(currentUserId, userId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

        public IActionResult OnPostJoinTeam(int teamId)
        {
            if (LoadUser() is not int currentUserId)
                return Challenge();

            try
            {
                teamManager.RequestJoinTeam(teamId, currentUserId);
                TempData["Success"] = "Join request sent successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }
    }
}
