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

        public Team Team { get; set; }
        public List<User> Members { get; set; }
        public List<TeamJoinRequest> Pending { get; set; }
        public User CurrentUser { get; set; }

        // ? NOVO
        public bool IsCaptain { get; set; }

        public DetailsModel(TeamManager tm, UserManager um)
        {
            teamManager = tm;
            userManager = um;
        }

        private void LoadUser()
        {
            var id = int.Parse(User.FindFirst("id").Value);
            CurrentUser = userManager.GetUserById(id);
        }

        public IActionResult OnGet(int id)
        {
            LoadUser();

            Team = teamManager.GetTeam(id);
            if (Team == null)
                return RedirectToPage("/Teams/Teams");

            Members = Team.Members;
            Pending = teamManager.GetJoinRequests(id);

            // ? KLJU?NA LINIJA
            IsCaptain = Team.Captain.Id == CurrentUser.Id;

            return Page();
        }

        public IActionResult OnPostApprove(int requestId, int teamId)
        {
            LoadUser();

            try
            {
                teamManager.ApproveRequest(requestId, CurrentUser.Id.Value);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

        public IActionResult OnPostReject(int requestId, int teamId)
        {
            LoadUser();

            try
            {
                teamManager.RejectRequest(requestId, CurrentUser.Id.Value);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

        public IActionResult OnPostLeave(int teamId)
        {
            LoadUser();

            try
            {
                teamManager.LeaveTeam(CurrentUser.Id.Value);
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
            LoadUser();

            try
            {
                teamManager.KickMember(CurrentUser.Id.Value, userId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToPage("/Teams/Details", new { id = teamId });
        }

        public IActionResult OnPostJoinTeam(int teamId)
        {
            LoadUser();

            try
            {
                teamManager.RequestJoinTeam(teamId, CurrentUser.Id.Value);
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
