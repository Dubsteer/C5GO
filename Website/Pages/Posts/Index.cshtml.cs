using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Posts
{
    public class IndexModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;
        private readonly CommentManager commentManager;

        public Post Post { get; set; }
        public List<Comment> Comments { get; set; }
        public User CurrentUser { get; set; }

        public bool ShowAllComments { get; set; }
        public HashSet<int> ExpandedReplies { get; set; } = new();

        [BindProperty] public CommentModel NewComment { get; set; }
        [BindProperty] public ReplyModel NewReply { get; set; }

        public IndexModel(PostManager postManager, UserManager userManager, CommentManager commentManager)
        {
            this.postManager = postManager;
            this.userManager = userManager;
            this.commentManager = commentManager;
        }

        private bool Load(int id)
        {
            Post = postManager.GetPostById(id);
            if (Post == null)
                return false;

            Comments = commentManager.GetAllCommentsWithReplies(id);

            if (User.Identity.IsAuthenticated)
            {
                int uid = int.Parse(User.FindFirst("id").Value);
                CurrentUser = userManager.GetUserById(uid);
            }

            return true;
        }

        public IActionResult OnGet(int id)
        {
            if (!Load(id))
                return RedirectToPage("/Error");

            return Page();
        }

        public IActionResult OnPostToggleComments(int id)
        {
            Load(id);
            ShowAllComments = !ShowAllComments;
            return Page();
        }

        public IActionResult OnPostToggleReplies(int id, int commentId)
        {
            Load(id);

            if (ExpandedReplies.Contains(commentId))
                ExpandedReplies.Remove(commentId);
            else
                ExpandedReplies.Add(commentId);

            return Page();
        }

        public IActionResult OnPostSubmitComment(int id)
        {
            Load(id);

            if (CurrentUser == null)
                return Unauthorized();

            var c = new Comment(
                0,
                CurrentUser,
                NewComment.CommentText,
                DateTime.Now,
                id
            );

            commentManager.AddComment(c);

            return RedirectToPage("Index", new { id });
        }

        public IActionResult OnPostSubmitReply(int id)
        {
            Load(id);

            if (CurrentUser == null)
                return Unauthorized();

            // ? OVDE JE FIX – šaljemo ceo User objekat
            var reply = new CommentReply(
                0,
                NewReply.ReplyText,
                DateTime.Now,
                NewReply.replyCommentId,
                CurrentUser     // ??? PRE JESTE BIO SAMO USERNAME
            );

            commentManager.AddReply(reply);

            // opcija: odmah otvori replye nakon dodavanja
            ExpandedReplies.Add(NewReply.replyCommentId);

            return RedirectToPage("Index", new { id });
        }

        public IActionResult OnPostDeleteComment(int id, int cid)
        {
            Load(id);
            var comment = commentManager.GetCommentById(cid);

            if (comment == null)
                return RedirectToPage("Index", new { id });

            if (CurrentUser.IsAdmin || CurrentUser.Id == comment.User.Id)
                commentManager.DeleteComment(comment);

            return RedirectToPage("Index", new { id });
        }
    }
}
