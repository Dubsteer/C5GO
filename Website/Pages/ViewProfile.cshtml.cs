using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Models;

namespace Website.Pages
{
    [Authorize]
    public class ViewProfileModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly PlayerManager playerManager;
        private readonly MatchManager matchManager;

        public ViewProfileModel(
            UserManager userManager,
            PlayerManager playerManager,
            MatchManager matchManager)
        {
            this.userManager = userManager;
            this.playerManager = playerManager;
            this.matchManager = matchManager;
        }

        public User PageUser { get; private set; } = null!;
        public Player? Player { get; private set; }
        public List<Match> Matches { get; private set; } = [];
        public PlayerMatchHistoryViewModel MatchHistory { get; private set; } = new();
        public bool RequireSteam { get; private set; }
        public bool NeedsSteamUpdate { get; private set; }
        public string? SteamProfileUrl { get; private set; }

        public IActionResult OnGet()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Login");

            var user = userManager.GetUserById(userId.Value);
            if (user == null)
                return Challenge();

            PageUser = user;
            Player = playerManager.GetPlayer(user);
            Matches = Player != null
                ? matchManager.GetPastMatches(user)
                : [];
            MatchHistory = new PlayerMatchHistoryViewModel
            {
                UserId = user.Id.GetValueOrDefault(),
                Matches = Matches,
                EmptyMessage = "Your completed C5GO solo tournament matches will appear here."
            };

            RequireSteam = TempData.ContainsKey("RequireSteam");
            NeedsSteamUpdate = Player == null &&
                               !string.IsNullOrWhiteSpace(user.SteamId) &&
                               user.SteamId != "0";

            if (Player?.SteamId is string steamId)
                SteamProfileUrl = SteamIdParser.BuildProfileUrl(steamId);

            return Page();
        }

        private int? GetUserId()
        {
            return int.TryParse(User.FindFirst("id")?.Value, out var userId)
                ? userId
                : null;
        }
    }
}
