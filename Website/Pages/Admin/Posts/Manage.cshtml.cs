using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Website.Pages.Admin.Posts
{
    public class ManageModel : PageModel
    {
        private readonly PostManager postManager;

        public List<Post> Posts { get; set; } = new();

        public ManageModel(PostManager postManager)
        {
            this.postManager = postManager;
        }

        public IActionResult OnGet()
        {
            Posts = postManager.GetAllPosts();
            return Page();
        }
    }
}
