using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;

namespace Website.Pages.Admin.Posts
{
    public class EditModel : PageModel
    {
        private readonly PostManager postManager;

        public EditModel(PostManager postManager)
        {
            this.postManager = postManager;
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        [BindProperty]
        public IFormFile? Image { get; set; }

        public IActionResult OnGet(int id)
        {
            var post = postManager.GetPostById(id);
            if (post == null)
                return NotFound();

            Post = post;
            return Page();
        }

        public IActionResult OnPost()
        {
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
