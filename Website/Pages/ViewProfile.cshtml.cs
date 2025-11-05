using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Website.Pages
{
    [Authorize] // ensures only logged-in users can access this page
    public class ViewProfileModel : PageModel
    {
        public User PageUser { get; set; }
        public List<Match> Matches { get; set; } = new();

        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;
        private readonly MatchManager _matchManager;

        [BindProperty]
        [MaxLength(11)]
        public string SteamId { get; set; }

        public ViewProfileModel(UserManager userManager, PlayerManager playerManager, MatchManager matchManager)
        {
            _userManager = userManager;
            _playerManager = playerManager;
            _matchManager = matchManager;
        }

        // ✅ Restored to match .cshtml usage
        public bool checkPlayer()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return false;

            try
            {
                var players = _playerManager.GetAllPlayers();
                return players.Any(p => p.Id == userId.Value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error checking player: " + ex.Message);
                return false;
            }
        }

        private int? GetCurrentUserId()
        {
            try
            {
                var claim = User?.FindFirst("id");
                if (claim == null)
                    return null;

                return Convert.ToInt32(claim.Value);
            }
            catch
            {
                return null;
            }
        }

        public IActionResult OnGet()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToPage("/Login");

            try
            {
                PageUser = _userManager.GetUserById(userId.Value);
                Matches = _matchManager.GetPastMatches(PageUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnGet error: " + ex.Message);
                return Content($"Error loading profile: {ex.Message}\n\n{ex.StackTrace}");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToPage("/Login");

            try
            {
                PageUser = _userManager.GetUserById(userId.Value);

                if (ModelState.IsValid)
                {
                    var newPlayer = new Player(
                        PageUser.Id ?? 0,
                        PageUser.Firstname,
                        PageUser.Lastname,
                        PageUser.Age,
                        PageUser.Username,
                        PageUser.Gmail,
                        PageUser.Password,
                        SteamId ?? "0",
                        PageUser.IsAdmin
                    );

                    _playerManager.InitializeRole(newPlayer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnPost error: " + ex.Message);
                return Content($"Error updating profile: {ex.Message}\n\n{ex.StackTrace}");
            }

            return Page();
        }
    }
}
