using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Website.Pages.Admin.Posts
{
    public class ManageModel : AdminPageModel
    {
        private readonly PostManager postManager;

        public List<Post> Posts { get; set; } = new();

        public ManageModel(PostManager postManager, UserManager userManager)
            : base(userManager) // ?? OVO JE KLJU?NO
        {
            this.postManager = postManager;
        }

        public IActionResult OnGet()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            Posts = postManager.GetAllPosts();
            return Page();
        }
    }
}
