using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;

namespace Website.Pages.Admin.Posts
{
    public class CreateModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;

        public CreateModel(PostManager postManager, UserManager userManager)
        {
            this.postManager = postManager;
            this.userManager = userManager;
        }

        [BindProperty] public string Title { get; set; } = "";
        [BindProperty] public string PostContent { get; set; } = "";
        [BindProperty] public IFormFile? Image { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            var author = userManager.GetByUsername(User.Identity!.Name!);
            if (author == null)
                return StatusCode(403);

            string? imagePath = null;

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

            var post = new Post
            {
                Title = Title,
                Content = PostContent,
                Posted_on = DateTime.Now,
                ImagePath = imagePath,
                User = author
            };

            postManager.AddPost(post);

            return RedirectToPage("./Manage");
        }
    }
}
