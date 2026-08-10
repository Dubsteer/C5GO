using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using LogicLayer.Managers;
using LogicLayer.Models;

namespace Website.Pages
{
    [Authorize]
    public class ViewProfileModel : PageModel
    {
        public User PageUser { get; set; } = null!;
        public Player? Player { get; set; }
        public List<Match> Matches { get; set; } = [];

        public bool RequireSteam { get; set; }

        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;
        private readonly MatchManager _matchManager;

        [BindProperty]
        [Required(ErrorMessage = "Steam ID is required")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Steam ID must be exactly 8 characters")]
        public string SteamId { get; set; } = string.Empty;

        public ViewProfileModel(UserManager u, PlayerManager p, MatchManager m)
        {
            _userManager = u;
            _playerManager = p;
            _matchManager = m;
        }

        private int? GetUserId()
        {
            return int.TryParse(User.FindFirst("id")?.Value, out var userId)
                ? userId
                : null;
        }

        public IActionResult OnGet()
        {
            var id = GetUserId();
            if (id == null) return Redirect("/Login");

            var user = _userManager.GetUserById(id.Value);
            if (user == null)
                return Challenge();

            PageUser = user;
            Player = _playerManager.GetPlayer(user);
            Matches = Player != null
                ? _matchManager.GetPastMatches(user)
                : [];

            SteamId = Player?.SteamId ?? "";

            if (TempData.ContainsKey("RequireSteam"))
                RequireSteam = true;

            return Page();
        }

        public IActionResult OnPost()
        {
            var id = GetUserId();
            if (id == null) return Redirect("/Login");

            var user = _userManager.GetUserById(id.Value);
            if (user == null)
                return Challenge();

            PageUser = user;
            Player = _playerManager.GetPlayer(user);
            Matches = Player != null
                ? _matchManager.GetPastMatches(user)
                : [];

            if (PageUser.IsAdmin)
            {
                ModelState.AddModelError("", "Admin cannot become a player.");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            if (_userManager.SteamIdExists(SteamId))
            {
                ModelState.AddModelError(
                    "SteamId",
                    "This Steam ID is already in use by another player."
                );
                return Page();
            }

            PageUser.SteamId = SteamId;

            var newPlayer = new Player(PageUser);
            _playerManager.InitializeRole(newPlayer);

            return Redirect("/ViewProfile");
        }
    }
}
