using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using System.Diagnostics;

namespace Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly TournamentManager tournamentManager;
        private readonly MatchManager matchManager;
        
        public List<Post> Posts { get; set; }


        public IndexModel(PostManager postManager)
        {
            this.postManager = postManager;

        }
        public IActionResult OnGet()
        {
            ViewData["Message"] = TempData["Message"];
            TempData.Clear();
            try
            {
                Posts = postManager.GetAllPosts();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // internal server error
                return StatusCode(500);
            }
            return Page();

        }
       
        public string TruncateString(string input, int maxLength)
        {
            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength) + "...";
            }
            else
            {
                return input;
            }
        }
    }
}