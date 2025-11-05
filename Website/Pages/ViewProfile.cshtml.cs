using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace Website.Pages
{
    [Authorize]
    public class ViewProfileModel : PageModel
    {
        // ✔ Public values koje koristi .cshtml
        public User PageUser { get; set; }
        public List<Match> Matches { get; set; } = new();

        // ✔ Injectovani menadžeri
        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;
        private readonly MatchManager _matchManager;

        public ViewProfileModel(UserManager userManager, PlayerManager playerManager, MatchManager matchManager)
        {
            _userManager = userManager;
            _playerManager = playerManager;
            _matchManager = matchManager;
        }

        // ----------------------------
        // ✔ Utičemo ID logged-in usera
        // ----------------------------
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

        // ----------------------------
        // ✔ GET: Učitavanje profila
        // ----------------------------
        public IActionResult OnGet()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return RedirectToPage("/Login");

            try
            {
                // Učitaj User info (iz tabele user)
                PageUser = _userManager.GetUserById(userId.Value);

                // Učitaj istoriju mečeva (prema tvom Match modelu)
                Matches = _matchManager.GetPastMatches(PageUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnGet error: " + ex.Message);
                return Content($"Error loading profile: {ex.Message}\n\n{ex.StackTrace}");
            }

            return Page();
        }

        // ----------------------------
        // ✔ POST: Update Steam ID
        // ----------------------------
        [BindProperty]
        public string SteamId { get; set; }

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
                    // Pretvori User u Player model
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

                    // Dodaj ili update Player u tabeli players
                    _playerManager.InitializeRole(newPlayer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnPost error: " + ex.Message);
                return Content($"Error updating profile: {ex.Message}\n\n{ex.StackTrace}");
            }

            return RedirectToPage();
        }
    }
}
