using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Posts
{
    public class PostModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;
        private readonly CommentManager commentManager;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Post Post { get; set; }
        public List<Comment> Comments { get; set; }
        public User CurrentUser { get; set; }

        [BindProperty] public CommentModel NewComment { get; set; }
        [BindProperty] public ReplyModel NewReply { get; set; }

        public HashSet<int> ExpandedReplies { get; set; } = new();

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
            Post = postManager.GetPostById(Id);
            if (Post == null)
                return false;

            Comments = commentManager.GetAllCommentsWithReplies(Id);

            if (User.Identity?.IsAuthenticated == true)
            {
                int uid = int.Parse(User.FindFirst("id").Value);
                CurrentUser = userManager.GetUserById(uid);
            }

            return true;
        }

        public IActionResult OnGet()
        {
            if (!LoadData())
                return RedirectToPage("/Error");

            return Page();
        }

        // TOGGLE REPLIES
        public IActionResult OnPostToggleReplies(int commentId)
        {
            LoadData();

            if (ExpandedReplies.Contains(commentId))
                ExpandedReplies.Remove(commentId);
            else
                ExpandedReplies.Add(commentId);

            return Page();
        }

        // ADD COMMENT
        public IActionResult OnPostSubmitComment()
        {
            LoadData();

            if (CurrentUser == null)
                return Unauthorized();

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

        // ADD REPLY
        public IActionResult OnPostSubmitReply()
        {
            LoadData();

            if (CurrentUser == null)
                return Unauthorized();

            var reply = new CommentReply(
                0,
                NewReply.ReplyText,
                DateTime.Now,
                NewReply.replyCommentId,
                CurrentUser
            );

            commentManager.AddReply(reply);

            ExpandedReplies.Add(NewReply.replyCommentId);
            return Page();
        }

        // DELETE COMMENT
        public IActionResult OnPostDeleteComment(int cid)
        {
            LoadData();

            var comment = commentManager.GetCommentById(cid);
            if (comment == null)
                return RedirectToPage("Post", new { Id });

            if (CurrentUser.IsAdmin || CurrentUser.Id == comment.User.Id)
                commentManager.DeleteComment(comment);

            return RedirectToPage("Post", new { Id });
        }

        // DELETE REPLY
        public IActionResult OnPostDeleteReply(int rid)
        {
            LoadData();

            var reply = commentManager.GetReplyById(rid);
            if (reply == null)
                return RedirectToPage("Post", new { Id });

            if (CurrentUser.IsAdmin || CurrentUser.Id == reply.User.Id)
                commentManager.DeleteReply(reply);

            ExpandedReplies.Add(reply.CommentId);
            return Page();
        }
    }
}
