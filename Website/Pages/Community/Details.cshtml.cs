using LogicLayer.Enums;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models.Community;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Website.Configuration;
using Website.Models;
using Website.Services;

namespace Website.Pages.Community;

[EnableRateLimiting("community")]
public class DetailsModel : CommunityPageModel
{
    private readonly CommunityManager communityManager;
    private readonly CommunityImageStorage imageStorage;

    public DetailsModel(
        CommunityManager communityManager,
        CommunityImageStorage imageStorage,
        IOptions<FeatureOptions> features)
        : base(features)
    {
        this.communityManager = communityManager;
        this.imageStorage = imageStorage;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public DiscussionCommentFormModel NewComment { get; set; } = new();

    [BindProperty]
    public ContentReportFormModel NewReport { get; set; } = new();

    public Discussion Discussion { get; private set; } = null!;
    public IReadOnlyList<DiscussionComment> Comments { get; private set; } = [];
    public bool CanModerate =>
        User.IsInRole(PlatformRole.Owner.ToString()) ||
        User.IsInRole(PlatformRole.Admin.ToString()) ||
        User.IsInRole(PlatformRole.Moderator.ToString());
    public IReadOnlyList<string> ReportReasons => CommunityManager.ReportReasons;

    public IActionResult OnGet()
    {
        if (!CommunityEnabled)
            return NotFound();

        return LoadData() ? Page() : NotFound();
    }

    public IActionResult OnPostVoteDiscussion(sbyte value)
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            var score = communityManager.SetDiscussionVote(Id, userId.Value, value);
            return IsAjaxRequest()
                ? new JsonResult(new { score })
                : RedirectToPage(new { id = Id });
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostVoteComment(int commentId, sbyte value)
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            var score = communityManager.SetCommentVote(commentId, userId.Value, value);
            return IsAjaxRequest()
                ? new JsonResult(new { score })
                : RedirectToPage(new { id = Id });
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostComment()
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        NewComment.DiscussionId = Id;
        try
        {
            communityManager.CreateComment(userId.Value, NewComment);
            return CommentsChanged("Comment published.");
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostDeleteComment(int commentId)
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            communityManager.RemoveOwnComment(commentId, userId.Value);
            return CommentsChanged("Comment removed.");
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostDeleteDiscussion()
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            if (!communityManager.RemoveOwnDiscussion(Id, userId.Value))
                throw new InvalidOperationException("The discussion could not be removed.");

            imageStorage.Delete(Discussion.ImagePath);
            return RedirectToPage("./Index");
        }
        catch (InvalidOperationException exception)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostReport()
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            if (!communityManager.CreateReport(userId.Value, NewReport))
                throw new InvalidOperationException("You have already reported this content.");

            return IsAjaxRequest()
                ? new JsonResult(new { message = "Report submitted." })
                : RedirectToPage(new { id = Id });
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostModerateDiscussion(
        ModerationActionType action,
        string? reason)
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            communityManager.ModerateDiscussion(Id, userId.Value, action, reason);
            return RedirectToPage(new { id = Id });
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public IActionResult OnPostModerateComment(
        int commentId,
        ModerationActionType action,
        string? reason)
    {
        if (!LoadData())
            return NotFoundResponse("Discussion was not found.");

        var userId = RequireCurrentUserId();
        if (!userId.HasValue)
            return AuthenticationRequired();

        try
        {
            communityManager.ModerateComment(commentId, userId.Value, action, reason);
            return CommentsChanged("Moderation action applied.");
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            return MutationError(exception.Message);
        }
    }

    public CommunityCommentsViewModel CreateCommentsViewModel()
    {
        return new CommunityCommentsViewModel
        {
            DiscussionId = Id,
            Comments = Comments,
            CurrentUserId = CurrentUserId,
            IsAuthenticated = User.Identity?.IsAuthenticated == true,
            CanModerate = CanModerate,
            ReportReasons = ReportReasons
        };
    }

    private bool LoadData()
    {
        if (!CommunityEnabled || Id <= 0)
            return false;

        var discussion = communityManager.GetDiscussion(Id, CurrentUserId);
        if (discussion == null)
            return false;

        if (discussion.Status == CommunityContentStatus.Removed &&
            discussion.AuthorId != CurrentUserId &&
            !CanModerate)
        {
            return false;
        }

        Discussion = discussion;
        Comments = communityManager.GetComments(Id, CurrentUserId);
        return true;
    }

    private IActionResult CommentsChanged(string successMessage)
    {
        if (!IsAjaxRequest())
            return RedirectToPage(new { id = Id });

        Comments = communityManager.GetComments(Id, CurrentUserId);
        var viewData = new ViewDataDictionary<CommunityCommentsViewModel>(
            ViewData,
            CreateCommentsViewModel());

        Response.Headers["X-Community-Message"] = successMessage;
        return new PartialViewResult
        {
            ViewName = "_Comments",
            ViewData = viewData,
            TempData = TempData
        };
    }

    private int? RequireCurrentUserId()
    {
        return User.Identity?.IsAuthenticated == true ? CurrentUserId : null;
    }

    private IActionResult AuthenticationRequired()
    {
        return IsAjaxRequest()
            ? StatusCode(
                StatusCodes.Status401Unauthorized,
                new { message = "Log in to continue." })
            : Challenge();
    }

    private IActionResult MutationError(string message)
    {
        if (IsAjaxRequest())
            return BadRequest(new { message });

        TempData["CommunityError"] = message;
        return RedirectToPage(new { id = Id });
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
