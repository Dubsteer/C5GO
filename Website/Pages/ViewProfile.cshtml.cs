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
        public User PageUser { get; set; }
        public Player Player { get; set; }
        public List<Match> Matches { get; set; } = new();

        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;
        private readonly MatchManager _matchManager;

        [BindProperty]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Steam ID must be 8 characters.")]
        public string SteamId { get; set; }

        public ViewProfileModel(UserManager u, PlayerManager p, MatchManager m)
        {
            _userManager = u;
            _playerManager = p;
            _matchManager = m;
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst("id");
            return claim == null ? null : int.Parse(claim.Value);
        }

        public IActionResult OnGet()
        {
            var id = GetUserId();
            if (id == null) return Redirect("/Login");

            PageUser = _userManager.GetUserById(id.Value);

            Player = _playerManager.GetPlayer(PageUser);

            Matches = Player != null ? _matchManager.GetPastMatches(PageUser) : new List<Match>();

            SteamId = Player?.SteamId ?? "";

            return Page();
        }

        public IActionResult OnPost()
        {
            var id = GetUserId();
            if (id == null) return Redirect("/Login");

            PageUser = _userManager.GetUserById(id.Value);

            if (PageUser.IsAdmin)
            {
                ModelState.AddModelError("", "Admin cannot become a player.");
                return Page();
            }

            if (!ModelState.IsValid)
                return Page();

            _playerManager.InitializeRole(new Player(
                PageUser.Id.Value,
                PageUser.Firstname,
                PageUser.Lastname,
                PageUser.Age,
                PageUser.Username,
                PageUser.Gmail,
                PageUser.Password,
                SteamId,
                PageUser.IsAdmin
            ));

            return Redirect("/ViewProfile");
        }
    }
}
