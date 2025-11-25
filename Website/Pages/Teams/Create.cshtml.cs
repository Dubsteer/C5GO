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
        public string TeamName { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }   // <--- FIX

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
            var id = int.Parse(User.FindFirst("id").Value);
            var user = userManager.GetUserById(id);

            if (string.IsNullOrWhiteSpace(TeamName))
            {
                ErrorMessage = "Team name cannot be empty.";
                return Page();
            }

            try
            {
                teamManager.CreateTeam(TeamName, user.Id.Value);
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
