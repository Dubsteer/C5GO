using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Website.Models;

namespace Website.Pages.Posts
{
    public class PostModel : PageModel
    {
        private readonly PostManager postManager;
        private readonly UserManager userManager;
        private readonly CommentManager commentManager;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public CommentModel NewComment { get; set; } = new();

        [BindProperty]
        public ReplyModel NewReply { get; set; } = new();

        public Post Post { get; set; } = null!;
        public List<Comment> Comments { get; set; } = [];
        public User? CurrentUser { get; set; }
        public IReadOnlyList<PostContentBlock> ContentBlocks { get; set; } = [];

        public PostModel(
            PostManager postManager,
            UserManager userManager,
            CommentManager commentManager)
        {
            this.postManager = postManager;
            this.userManager = userManager;
            this.commentManager = commentManager;
        }

        public IActionResult OnGet()
        {
            return LoadData() ? Page() : NotFound();
        }

        public IActionResult OnPostSubmitComment()
        {
            if (!LoadData())
                return NotFoundResponse("Post was not found.");

            if (CurrentUser == null)
                return AuthenticationRequired();

            try
            {
                commentManager.AddComment(new Comment(
                    0,
                    CurrentUser,
                    NewComment.CommentText,
                    DateTime.Now,
                    Id));
            }
            catch (ArgumentException exception)
            {
                return ValidationError(exception.Message, nameof(NewComment));
            }

            return MutationSucceeded();
        }

        public IActionResult OnPostSubmitReply()
        {
            if (!LoadData())
                return NotFoundResponse("Post was not found.");

            if (CurrentUser == null)
                return AuthenticationRequired();

            var parentComment = commentManager.GetCommentById(NewReply.CommentId);
            if (parentComment == null || parentComment.PostId != Id)
                return NotFoundResponse("Comment was not found.");

            try
            {
                commentManager.AddReply(new CommentReply(
                    0,
                    NewReply.ReplyText,
                    DateTime.Now,
                    parentComment.Id,
                    CurrentUser));
            }
            catch (ArgumentException exception)
            {
                return ValidationError(exception.Message, nameof(NewReply));
            }

            return MutationSucceeded();
        }

        public IActionResult OnPostDeleteComment(int cid)
        {
            if (!LoadData())
                return NotFoundResponse("Post was not found.");

            if (CurrentUser == null)
                return AuthenticationRequired();

            var comment = commentManager.GetCommentById(cid);
            if (comment == null || comment.PostId != Id)
                return NotFoundResponse("Comment was not found.");

            if (!User.IsInRole("Owner") &&
                !User.IsInRole("Admin") &&
                CurrentUser.Id != comment.User.Id)
                return ForbiddenResponse();

            commentManager.DeleteComment(comment);
            return MutationSucceeded();
        }

        public IActionResult OnPostDeleteReply(int rid)
        {
            if (!LoadData())
                return NotFoundResponse("Post was not found.");

            if (CurrentUser == null)
                return AuthenticationRequired();

            var reply = commentManager.GetReplyById(rid);
            if (reply == null)
                return NotFoundResponse("Reply was not found.");

            var parentComment = commentManager.GetCommentById(reply.CommentId);
            if (parentComment == null || parentComment.PostId != Id)
                return NotFoundResponse("Reply was not found.");

            if (!User.IsInRole("Owner") &&
                !User.IsInRole("Admin") &&
                CurrentUser.Id != reply.User.Id)
                return ForbiddenResponse();

            commentManager.DeleteReply(reply);
            return MutationSucceeded();
        }

        public PostCommentsViewModel CreateCommentsViewModel()
        {
            return new PostCommentsViewModel
            {
                PostId = Id,
                Comments = Comments,
                CurrentUser = CurrentUser,
                IsAuthenticated = User.Identity?.IsAuthenticated == true
            };
        }

        private bool LoadData()
        {
            if (Id <= 0)
                return false;

            var post = postManager.GetPostById(Id);
            if (post == null)
                return false;

            Post = post;
            ContentBlocks = PostContentParser.Parse(post.Content);
            Comments = commentManager.GetAllCommentsWithReplies(Id);

            if (User.Identity?.IsAuthenticated == true &&
                int.TryParse(User.FindFirst("id")?.Value, out var userId))
            {
                CurrentUser = userManager.GetUserById(userId);
            }

            return true;
        }

        private IActionResult MutationSucceeded()
        {
            if (!IsAjaxRequest())
                return RedirectToPage("Post", new { Id });

            Comments = commentManager.GetAllCommentsWithReplies(Id);
            var viewData = new ViewDataDictionary<PostCommentsViewModel>(ViewData, CreateCommentsViewModel());

            return new PartialViewResult
            {
                ViewName = "_CommentList",
                ViewData = viewData,
                TempData = TempData
            };
        }

        private IActionResult ValidationError(string message, string modelKey)
        {
            ModelState.AddModelError(modelKey, message);
            return IsAjaxRequest()
                ? BadRequest(new { message })
                : Page();
        }

        private IActionResult AuthenticationRequired()
        {
            return IsAjaxRequest()
                ? StatusCode(StatusCodes.Status401Unauthorized, new { message = "Your session has expired. Log in again." })
                : Challenge();
        }

        private IActionResult ForbiddenResponse()
        {
            return IsAjaxRequest()
                ? StatusCode(StatusCodes.Status403Forbidden, new { message = "You cannot delete this comment." })
                : Forbid();
        }

        private IActionResult NotFoundResponse(string message)
        {
            return IsAjaxRequest()
                ? NotFound(new { message })
                : NotFound();
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
