using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LogicLayer.Managers;
using LogicLayer.Models;

namespace Website.Pages   // ? OVO je klju?no
{
    public class UserProfileModel : PageModel
    {
        public User ViewedUser { get; set; }
        public Player ViewedPlayer { get; set; }

        private readonly UserManager _userManager;
        private readonly PlayerManager _playerManager;

        public UserProfileModel(UserManager um, PlayerManager pm)
        {
            _userManager = um;
            _playerManager = pm;
        }

        public IActionResult OnGet(int id)
        {
            ViewedUser = _userManager.GetUserById(id);

            if (ViewedUser == null)
                return Redirect("/Error");

            ViewedPlayer = _playerManager.GetPlayer(ViewedUser);

            return Page();
        }
    }
}
