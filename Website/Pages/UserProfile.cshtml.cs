using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LogicLayer.Managers;
using LogicLayer.Models;

namespace Website.Pages
{
    public class UserProfileModel : PageModel
    {
        public User ViewedUser { get; set; } = null!;
        public Player? ViewedPlayer { get; set; }

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

            return Page();
        }
    }
}
