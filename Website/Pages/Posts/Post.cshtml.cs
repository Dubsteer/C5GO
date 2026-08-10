using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace Website.Pages.Posts
{
    public class PostModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;
        private readonly CommentManager commentManager;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Post Post { get; set; } = null!;
        public List<Comment> Comments { get; set; } = [];
        public User? CurrentUser { get; set; }


        [BindProperty] public CommentModel NewComment { get; set; } = new();
        [BindProperty] public ReplyModel NewReply { get; set; } = new();

        public PostModel(
            PostManager postManager,
            UserManager userManager,
            CommentManager commentManager)
        {
            this.postManager = postManager;
            this.userManager = userManager;
            this.commentManager = commentManager;
        }

        private bool LoadData()
        {
            if (Id <= 0)
                return false;

            var post = postManager.GetPostById(Id);
            if (post == null)
                return false;

            Post = post;
            Post.Content = WebUtility.HtmlDecode(Post.Content);

            Comments = commentManager.GetAllCommentsWithReplies(Id);

            if (User.Identity?.IsAuthenticated == true)
            {
                if (int.TryParse(User.FindFirst("id")?.Value, out var userId))
                    CurrentUser = userManager.GetUserById(userId);
            }

            return true;
        }

        public IActionResult OnGet()
        {
            if (!LoadData())
                return RedirectToPage("/Error");

            return Page();
        }

        public IActionResult OnPostSubmitComment()
        {
            if (!LoadData())
                return RedirectToPage("/Error");

            if (CurrentUser == null)
                return Challenge();

            var comment = new Comment(
                0,
                CurrentUser,
                NewComment.CommentText,
                DateTime.Now,
                Id
            );

            commentManager.AddComment(comment);
            return RedirectToPage("Post", new { Id });
        }

        public IActionResult OnPostSubmitReply()
        {
            if (!LoadData())
                return RedirectToPage("/Error");

            if (CurrentUser == null)
                return Challenge();

            var reply = new CommentReply(
                0,
                NewReply.ReplyText,
                DateTime.Now,
                NewReply.replyCommentId,
                CurrentUser
            );

            commentManager.AddReply(reply);
            return RedirectToPage("Post", new { Id });
        }

        public IActionResult OnPostDeleteComment(int cid)
        {
            if (!LoadData())
                return RedirectToPage("/Error");

            if (CurrentUser == null)
                return Challenge();

            var comment = commentManager.GetCommentById(cid);

            if (comment != null &&
                (CurrentUser.IsAdmin || CurrentUser.Id == comment.User.Id))
            {
                commentManager.DeleteComment(comment);
            }

            return RedirectToPage("Post", new { Id });
        }


        public IActionResult OnPostDeleteReply(int rid)
        {
            if (!LoadData())
                return RedirectToPage("/Error");

            if (CurrentUser == null)
                return Challenge();

            var reply = commentManager.GetReplyById(rid);

            if (reply != null &&
                (CurrentUser.IsAdmin || CurrentUser.Id == reply.User.Id))
            {
                commentManager.DeleteReply(reply);
            }

            return RedirectToPage("Post", new { Id });
        }

    }
}
