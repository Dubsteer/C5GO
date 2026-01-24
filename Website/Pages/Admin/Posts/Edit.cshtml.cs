using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Website.Pages.Admin;

namespace Website.Pages.Admin.Posts
{
    public class EditModel : AdminPageModel
    {
        private readonly PostManager postManager;

        public EditModel(PostManager postManager, UserManager userManager)
            : base(userManager)
        {
            this.postManager = postManager;
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

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

        public IActionResult OnPost()
        {
            var guard = RequireAdmin();
            if (guard != null) return guard;

            var postFromDb = postManager.GetPostById(Post.Id);
            if (postFromDb == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(Post.Title))
                Post.Title = "Untitled post";

            string? imagePath = postFromDb.ImagePath;

            if (Image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var savePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "posts",
                    fileName
                );

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                using var stream = new FileStream(savePath, FileMode.Create);
                Image.CopyTo(stream);

                imagePath = "/images/posts/" + fileName;
            }

            postFromDb.Title = Post.Title;
            postFromDb.Content = Post.Content;
            postFromDb.ImagePath = imagePath;

            postManager.UpdatePost(postFromDb);

            return RedirectToPage("./Manage");
        }
    }
}
