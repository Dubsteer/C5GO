using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System;
using Website.Services;

namespace Website.Pages.Admin.Posts
{
    [RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
    public class CreateModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;
        private readonly PostImageStorage imageStorage;

        public CreateModel(
            PostManager postManager,
            UserManager userManager,
            PostImageStorage imageStorage)
        {
            this.postManager = postManager;
            this.userManager = userManager;
            this.imageStorage = imageStorage;
        }

        [BindProperty]
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "";

        [BindProperty]
        [Required]
        public string PostContent { get; set; } = "";

        [BindProperty] public IFormFile? Image { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return Page();

            var author = userManager.GetByUsername(User.Identity!.Name!);
            if (author == null)
                return StatusCode(403);

            string? imagePath = null;

            if (Image != null)
            {
                try
                {
                    imagePath = await imageStorage.SaveAsync(Image, cancellationToken);
                }
                catch (ImageUploadException exception)
                {
                    ModelState.AddModelError(nameof(Image), exception.Message);
                    return Page();
                }
            }

            var post = new Post
            {
                Title = Title.Trim(),
                Content = PostContent.Trim(),
                Posted_on = DateTime.UtcNow,
                ImagePath = imagePath,
                User = author
            };

            try
            {
                postManager.AddPost(post);
            }
            catch
            {
                imageStorage.Delete(imagePath);
                throw;
            }

            return RedirectToPage("./Manage");
        }
    }
}
