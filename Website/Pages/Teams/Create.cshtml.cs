using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Teams
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly TeamManager teamManager;
        private readonly UserManager userManager;

        [BindProperty]
        public string TeamName { get; set; } = string.Empty;

        [BindProperty]
        public string ErrorMessage { get; set; } = string.Empty;

        public CreateModel(TeamManager tm, UserManager um)
        {
            teamManager = tm;
            userManager = um;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!int.TryParse(User.FindFirst("id")?.Value, out var userId))
                return Challenge();

            var user = userManager.GetUserById(userId);
            if (user?.Id is not int captainId)
                return Challenge();

            if (string.IsNullOrWhiteSpace(TeamName))
            {
                ErrorMessage = "Team name cannot be empty.";
                return Page();
            }

            try
            {
                teamManager.CreateTeam(TeamName, captainId);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }

            return RedirectToPage("/Teams/Teams");
        }
    }
}
