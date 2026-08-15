using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Website.Services;

namespace Website.Pages.Admin.Posts
{
    [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
    public class EditModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly PostImageStorage imageStorage;

        public EditModel(PostManager postManager, PostImageStorage imageStorage)
        {
            this.postManager = postManager;
            this.imageStorage = imageStorage;
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        [BindProperty]
        public bool RemoveImage { get; set; }

        public IActionResult OnGet(int id)
        {
            var post = postManager.GetPostById(id);
            if (post == null)
                return NotFound();

            Post = post;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var postFromDb = postManager.GetPostById(Post.Id);
            if (postFromDb == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(Post.Title))
            {
                ModelState.AddModelError("Post.Title", "Title is required.");
            }

            if (string.IsNullOrWhiteSpace(Post.Content))
            {
                ModelState.AddModelError("Post.Content", "Content is required.");
            }

            if (!ModelState.IsValid)
            {
                Post.ImagePath = postFromDb.ImagePath;
                return Page();
            }

            var existingImagePath = postFromDb.ImagePath;
            string? newImagePath = null;

            if (Image != null)
            {
                try
                {
                    newImagePath = await imageStorage.SaveAsync(Image, cancellationToken);
                }
                catch (ImageUploadException exception)
                {
                    Post.ImagePath = postFromDb.ImagePath;
                    ModelState.AddModelError(nameof(Image), exception.Message);
                    return Page();
                }
            }

            postFromDb.Title = Post.Title.Trim();
            postFromDb.Content = Post.Content.Trim();
            postFromDb.ImagePath = newImagePath ?? (RemoveImage ? null : existingImagePath);

            try
            {
                postManager.UpdatePost(postFromDb);
            }
            catch
            {
                imageStorage.Delete(newImagePath);
                throw;
            }

            if ((RemoveImage || newImagePath != null) && existingImagePath != null)
                imageStorage.Delete(existingImagePath);

            return RedirectToPage("./Manage");
        }
    }
}
