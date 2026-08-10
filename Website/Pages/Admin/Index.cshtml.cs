using LogicLayer.Managers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly PostManager postManager;

        public int UserCount { get; private set; }
        public int PlayerCount { get; private set; }
        public int PostCount { get; private set; }

        public IndexModel(UserManager userManager, PostManager postManager)
        {
            this.userManager = userManager;
            this.postManager = postManager;
        }

        public void OnGet()
        {
            var users = userManager.GetAllUsers();
            UserCount = users.Count;
            PlayerCount = users.Count(user =>
                !string.IsNullOrWhiteSpace(user.SteamId) && user.SteamId != "0");
            PostCount = postManager.GetAllPosts().Count;
        }
    }
}
