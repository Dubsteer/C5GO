using System.Security.Claims;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly PlayerManager playerManager;

        public List<User> Users { get; private set; } = new();
        public int CurrentAdminId { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IndexModel(UserManager userManager, PlayerManager playerManager)
        {
            this.userManager = userManager;
            this.playerManager = playerManager;
        }

        public IActionResult OnGet()
        {
            if (!TryLoadCurrentAdminId())
                return Forbid();

            LoadUsers();
            return Page();
        }

        public IActionResult OnPostRemovePlayer(int id)
        {
            if (!TryLoadCurrentAdminId())
                return Forbid();

            try
            {
                playerManager.RemovePlayerRole(id);
                TempData["SuccessMessage"] = "Player profile removed.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The player profile could not be removed.";
            }

            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!TryLoadCurrentAdminId())
                return Forbid();

            try
            {
                userManager.DeleteUserAsAdmin(id, CurrentAdminId);
                TempData["SuccessMessage"] = "User account deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] =
                    "The account could not be deleted because it is still connected to other data.";
            }

            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public bool IsPlayer(User user) =>
            !string.IsNullOrWhiteSpace(user.SteamId) && user.SteamId != "0";

        private bool TryLoadCurrentAdminId()
        {
            var claim = User.FindFirstValue("id");
            if (!int.TryParse(claim, out var currentAdminId))
                return false;

            CurrentAdminId = currentAdminId;
            return true;
        }

        private void LoadUsers()
        {
            Users = string.IsNullOrWhiteSpace(SearchTerm)
                ? userManager.GetAllUsers()
                : userManager.SearchUser(SearchTerm.Trim());

            Users = Users.OrderBy(user => user.Username).ToList();
        }
    }
}
