using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Website.Services;

namespace Website.Pages.Admin.Posts
{
    public class DeleteModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly PostImageStorage imageStorage;

        public DeleteModel(PostManager postManager, PostImageStorage imageStorage)
        {
            this.postManager = postManager;
            this.imageStorage = imageStorage;
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
            imageStorage.Delete(post.ImagePath);

            return RedirectToPage("./Manage");
        }
    }
}
