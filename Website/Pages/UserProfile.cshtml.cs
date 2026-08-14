using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Services;
using LogicLayer.Enums;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Pages
{
    public class UserProfileModel : PageModel
    {
        public User ViewedUser { get; set; } = null!;
        public Player? ViewedPlayer { get; set; }
        public string? SteamProfileUrl { get; set; }
        public PlatformRole HighestRole { get; private set; }

        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;
        private readonly RoleManager roleManager;
        private readonly FeatureOptions features;

        public UserProfileModel(
            UserManager um,
            PlayerManager pm,
            RoleManager roleManager,
            IOptions<FeatureOptions> features)
        {
            _userManager = um;
            _playerManager = pm;
            this.roleManager = roleManager;
            this.features = features.Value;
        }

        public IActionResult OnGet(int id)
        {
            var user = _userManager.GetUserById(id);
            if (user == null)
                return NotFound();

            ViewedUser = user;
            ViewedPlayer = _playerManager.GetPlayer(user);
            HighestRole = features.CommunityEnabled
                ? roleManager.GetHighestRole(id)
                : user.IsAdmin ? PlatformRole.Admin : PlatformRole.Member;

            if (ViewedPlayer?.SteamId is string steamId)
                SteamProfileUrl = SteamIdParser.BuildProfileUrl(steamId);

            return Page();
        }
    }
}
