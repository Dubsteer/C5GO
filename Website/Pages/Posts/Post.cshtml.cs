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

        public bool ShowAllComments { get; set; }
        public HashSet<int> ExpandedReplies { get; set; } = new();

        public PostModel(PostManager postManager, UserManager userManager, CommentManager commentManager)
        {
            this.postManager = postManager;
            this.userManager = userManager;
            this.commentManager = commentManager;
        }

        // Central loader
        private bool LoadPostData()
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

        // GET
        public IActionResult OnGet()
        {
            if (!LoadPostData())
                return RedirectToPage("/Error");

            return Page();
        }

        // Toggle all comments
        public IActionResult OnPostToggleComments()
        {
            LoadPostData();
            ShowAllComments = !ShowAllComments;
            return Page();
        }

        // Toggle replies
        public IActionResult OnPostToggleReplies(int commentId)
        {
            LoadPostData();

            if (ExpandedReplies.Contains(commentId))
                ExpandedReplies.Remove(commentId);
            else
                ExpandedReplies.Add(commentId);

            return Page();
        }

        // Add comment
        public IActionResult OnPostSubmitComment()
        {
            LoadPostData();

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

        // Add reply
        public IActionResult OnPostSubmitReply()
        {
            LoadPostData();

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

            return RedirectToPage("Post", new { Id });
        }

        // Delete comment
        public IActionResult OnPostDeleteComment(int cid)
        {
            LoadPostData();

            var comment = commentManager.GetCommentById(cid);
            if (comment == null)
                return RedirectToPage("Post", new { Id });

            if (CurrentUser.IsAdmin || CurrentUser.Id == comment.User.Id)
                commentManager.DeleteComment(comment);

            return RedirectToPage("Post", new { Id });
        }
    }
}
