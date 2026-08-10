using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.Posts
{
    public class DeleteModel : PageModel
    {
        private readonly PostManager postManager;

        public DeleteModel(PostManager postManager)
        {
            this.postManager = postManager;
        }

        public Post Post { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var post = postManager.GetPostById(id);
            if (post == null)
                return NotFound();

            Post = post;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var post = postManager.GetPostById(id);
            if (post == null)
                return NotFound();

            postManager.DeletePost(post);

            return RedirectToPage("./Manage");
        }
    }
}
