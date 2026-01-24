using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Website.Pages.Admin.Posts
{
    public class DeleteModel : AdminPageModel
    {
        private readonly PostManager postManager;

        public DeleteModel(PostManager postManager, UserManager userManager)
            : base(userManager) // ?? OBAVEZNO
        {
            this.postManager = postManager;
        }

        public Post Post { get; set; } = default!;

        public IActionResult OnGet(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var post = postManager.GetPostById(id);
            if (post == null)
                return NotFound();

            Post = post;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var post = postManager.GetPostById(id);
            if (post == null)
                return NotFound();

            postManager.DeletePost(post);

            return RedirectToPage("./Manage");
        }
    }
}
