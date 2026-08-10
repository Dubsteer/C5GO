using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Services;

namespace Website.Pages
{
    public class UserProfileModel : PageModel
    {
        public User ViewedUser { get; set; } = null!;
        public Player? ViewedPlayer { get; set; }
        public string? SteamProfileUrl { get; set; }

        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;

        public UserProfileModel(UserManager um, PlayerManager pm)
        {
            _userManager = um;
            _playerManager = pm;
        }

        public IActionResult OnGet(int id)
        {
            var user = _userManager.GetUserById(id);
            if (user == null)
                return NotFound();

            ViewedUser = user;
            ViewedPlayer = _playerManager.GetPlayer(user);

            if (ViewedPlayer?.SteamId is string steamId)
                SteamProfileUrl = SteamIdParser.BuildProfileUrl(steamId);

            return Page();
        }
    }
}
