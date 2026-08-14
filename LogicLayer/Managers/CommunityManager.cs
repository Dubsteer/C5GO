using LogicLayer.Enums;
using LogicLayer.FormModels;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Models.Community;
using LogicLayer.Services;

namespace LogicLayer.Managers;

public class CommunityManager
{
    public static readonly IReadOnlyList<string> ReportReasons =
    [
        "Spam",
        "Harassment",
        "Hate or abusive content",
        "Misleading information",
        "Inappropriate media",
        "Other"
    ];

    private readonly ICommunityRepo communityRepo;
    private readonly IUserRepo userRepo;
    private readonly INotificationRepo notificationRepo;
    private readonly RoleManager roleManager;

    public CommunityManager(
        ICommunityRepo communityRepo,
        IUserRepo userRepo,
        INotificationRepo notificationRepo,
        RoleManager roleManager)
    {
        this.communityRepo = communityRepo;
        this.userRepo = userRepo;
        this.notificationRepo = notificationRepo;
        this.roleManager = roleManager;
    }

    public IReadOnlyList<CommunityCategory> GetCategories()
    {
        return communityRepo.GetCategories();
    }

    public IReadOnlyDictionary<int, CommunityContributionStats> GetContributionStats()
    {
        return communityRepo.GetContributionStats();
    }

    public PagedResult<Discussion> GetDiscussions(
        int? categoryId,
        CommunitySort sort,
        int page,
        int pageSize,
        int? viewerId)
    {
        pageSize = Math.Clamp(pageSize, 1, 50);
        var totalCount = communityRepo.GetDiscussionCount(categoryId);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        page = Math.Clamp(page, 1, totalPages);

        var discussions = communityRepo.GetDiscussions(
            categoryId,
            sort,
            page,
            pageSize,
            viewerId);

        return new PagedResult<Discussion>(
            discussions,
            page,
            pageSize,
            totalCount);
    }

    public Discussion? GetDiscussion(int discussionId, int? viewerId = null)
    {
        return discussionId > 0
            ? communityRepo.GetDiscussionById(discussionId, viewerId)
            : null;
    }

    public int CreateDiscussion(
        int authorId,
        DiscussionFormModel form,
        string? imagePath)
    {
        EnsureUserExists(authorId);
        var discussion = BuildDiscussion(form, imagePath);
        discussion.AuthorId = authorId;
        discussion.CreatedAt = DateTime.UtcNow;
        discussion.Status = CommunityContentStatus.Published;
        return communityRepo.CreateDiscussion(discussion);
    }

    public bool UpdateDiscussion(
        int discussionId,
        int authorId,
        DiscussionFormModel form,
        string? imagePath)
    {
        var existing = GetRequiredDiscussion(discussionId);
        if (existing.AuthorId != authorId)
            throw new InvalidOperationException("You can edit only your own discussion.");
        if (existing.IsLocked || existing.Status != CommunityContentStatus.Published)
            throw new InvalidOperationException("This discussion cannot be edited.");

        var updated = BuildDiscussion(form, imagePath);
        updated.Id = discussionId;
        updated.AuthorId = authorId;
        updated.UpdatedAt = DateTime.UtcNow;
        return communityRepo.UpdateDiscussion(updated, authorId);
    }

    public bool RemoveOwnDiscussion(int discussionId, int authorId)
    {
        var discussion = GetRequiredDiscussion(discussionId);
        if (discussion.AuthorId != authorId)
            throw new InvalidOperationException("You can remove only your own discussion.");

        return communityRepo.RemoveOwnDiscussion(discussionId, authorId);
    }

    public int SetDiscussionVote(int discussionId, int userId, sbyte voteValue)
    {
        ValidateVote(voteValue);
        EnsureUserExists(userId);
        var discussion = GetRequiredDiscussion(discussionId);
        if (discussion.Status != CommunityContentStatus.Published)
            throw new InvalidOperationException("This discussion is not available for voting.");

        return communityRepo.SetDiscussionVote(discussionId, userId, voteValue);
    }

    public IReadOnlyList<DiscussionComment> GetComments(
        int discussionId,
        int? viewerId = null)
    {
        var comments = communityRepo.GetComments(discussionId, viewerId);
        var topLevel = comments
            .Where(comment => comment.ParentCommentId == null)
            .ToDictionary(comment => comment.Id);

        foreach (var comment in topLevel.Values)
        {
            comment.Replies = comments
                .Where(reply => reply.ParentCommentId == comment.Id)
                .OrderBy(reply => reply.CreatedAt)
                .ToArray();
        }

        return topLevel.Values
            .OrderBy(comment => comment.CreatedAt)
            .ToArray();
    }

    public int CreateComment(int userId, DiscussionCommentFormModel form)
    {
        EnsureUserExists(userId);
        var discussion = GetRequiredDiscussion(form.DiscussionId);
        if (discussion.Status != CommunityContentStatus.Published || discussion.IsLocked)
            throw new InvalidOperationException("Comments are closed for this discussion.");

        var content = form.Content?.Trim() ?? string.Empty;
        if (content.Length is < 1 or > 2000)
            throw new ArgumentException("A comment must contain between 1 and 2000 characters.");

        DiscussionComment? parent = null;
        if (form.ParentCommentId is int parentId)
        {
            parent = communityRepo.GetCommentById(parentId)
                ?? throw new InvalidOperationException("The parent comment was not found.");

            if (parent.DiscussionId != form.DiscussionId || parent.ParentCommentId != null)
                throw new InvalidOperationException("Replies can be added only to a top-level comment.");
        }

        var commentId = communityRepo.CreateComment(new DiscussionComment
        {
            DiscussionId = form.DiscussionId,
            AuthorId = userId,
            ParentCommentId = form.ParentCommentId,
            Content = content,
            Status = CommunityContentStatus.Published,
            CreatedAt = DateTime.UtcNow
        });

        var notificationUserId = parent?.AuthorId ?? discussion.AuthorId;
        if (notificationUserId != userId)
        {
            notificationRepo.Create(
                notificationUserId,
                parent == null
                    ? "Someone commented on your discussion."
                    : "Someone replied to your comment.",
                $"/Community/Details?id={discussion.Id}#comment-{commentId}");
        }

        return commentId;
    }

    public bool RemoveOwnComment(int commentId, int userId)
    {
        var comment = communityRepo.GetCommentById(commentId)
            ?? throw new InvalidOperationException("Comment was not found.");
        if (comment.AuthorId != userId)
            throw new InvalidOperationException("You can remove only your own comment.");

        return communityRepo.RemoveOwnComment(commentId, userId);
    }

    public int SetCommentVote(int commentId, int userId, sbyte voteValue)
    {
        ValidateVote(voteValue);
        EnsureUserExists(userId);
        var comment = communityRepo.GetCommentById(commentId)
            ?? throw new InvalidOperationException("Comment was not found.");
        if (comment.Status != CommunityContentStatus.Published)
            throw new InvalidOperationException("This comment is not available for voting.");

        return communityRepo.SetCommentVote(commentId, userId, voteValue);
    }

    public bool CreateReport(int reporterId, ContentReportFormModel form)
    {
        EnsureUserExists(reporterId);
        var hasDiscussion = form.DiscussionId is > 0;
        var hasComment = form.CommentId is > 0;
        if (hasDiscussion == hasComment)
            throw new ArgumentException("Choose one item to report.");

        var reason = form.Reason?.Trim() ?? string.Empty;
        if (!ReportReasons.Contains(reason, StringComparer.Ordinal))
            throw new ArgumentException("Choose a valid report reason.");

        int targetAuthorId;
        if (hasDiscussion)
        {
            targetAuthorId = GetRequiredDiscussion(form.DiscussionId!.Value).AuthorId;
        }
        else
        {
            targetAuthorId = communityRepo.GetCommentById(form.CommentId!.Value)?.AuthorId
                ?? throw new InvalidOperationException("Comment was not found.");
        }

        if (targetAuthorId == reporterId)
            throw new InvalidOperationException("You cannot report your own content.");

        return communityRepo.CreateReport(new ContentReport
        {
            ReporterId = reporterId,
            DiscussionId = hasDiscussion ? form.DiscussionId : null,
            CommentId = hasComment ? form.CommentId : null,
            Reason = reason,
            Details = NormalizeOptional(form.Details, 1000),
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });
    }

    public IReadOnlyList<ContentReport> GetPendingReports(int moderatorId)
    {
        var moderatorRole = roleManager.GetHighestRole(moderatorId);
        if (!RolePolicy.CanModerateContent(moderatorRole))
            throw new InvalidOperationException("Moderator access is required.");

        return communityRepo.GetPendingReports()
            .Where(report => RolePolicy.CanModerate(
                moderatorRole,
                roleManager.GetHighestRole(report.TargetAuthorId)))
            .ToArray();
    }

    public bool ReviewReport(
        long reportId,
        int moderatorId,
        ReportStatus status,
        string? resolutionNote)
    {
        var report = communityRepo.GetPendingReports()
            .FirstOrDefault(item => item.Id == reportId)
            ?? throw new InvalidOperationException("Report was not found.");
        EnsureCanModerateUser(moderatorId, report.TargetAuthorId);
        if (status is not (ReportStatus.Resolved or ReportStatus.Rejected))
            throw new ArgumentException("Choose a valid report result.", nameof(status));

        return communityRepo.ReviewReport(
            reportId,
            moderatorId,
            status,
            NormalizeOptional(resolutionNote, 500));
    }

    public bool ModerateDiscussion(
        int discussionId,
        int moderatorId,
        ModerationActionType action,
        string? reason)
    {
        var discussion = GetRequiredDiscussion(discussionId);
        EnsureCanModerateUser(moderatorId, discussion.AuthorId);
        if (action is not (
            ModerationActionType.LockDiscussion or
            ModerationActionType.UnlockDiscussion or
            ModerationActionType.PinDiscussion or
            ModerationActionType.UnpinDiscussion or
            ModerationActionType.RemoveDiscussion or
            ModerationActionType.RestoreDiscussion))
        {
            throw new ArgumentException("Invalid discussion moderation action.", nameof(action));
        }

        return communityRepo.ApplyDiscussionModeration(
            discussionId,
            moderatorId,
            action,
            NormalizeOptional(reason, 500));
    }

    public bool ModerateComment(
        int commentId,
        int moderatorId,
        ModerationActionType action,
        string? reason)
    {
        var comment = communityRepo.GetCommentById(commentId)
            ?? throw new InvalidOperationException("Comment was not found.");
        EnsureCanModerateUser(moderatorId, comment.AuthorId);
        if (action is not (
            ModerationActionType.RemoveComment or
            ModerationActionType.RestoreComment))
        {
            throw new ArgumentException("Invalid comment moderation action.", nameof(action));
        }

        return communityRepo.ApplyCommentModeration(
            commentId,
            moderatorId,
            action,
            NormalizeOptional(reason, 500));
    }

    private Discussion BuildDiscussion(DiscussionFormModel form, string? imagePath)
    {
        var category = communityRepo.GetCategories()
            .FirstOrDefault(item => item.Id == form.CategoryId && item.IsActive)
            ?? throw new ArgumentException("Choose a valid category.");

        var title = form.Title?.Trim() ?? string.Empty;
        if (title.Length is < 5 or > 160)
            throw new ArgumentException("The title must contain between 5 and 160 characters.");

        var content = NormalizeOptional(form.Content, 10000);
        var normalizedImagePath = NormalizeOptional(imagePath, 255);
        string? videoId = null;

        if (!string.IsNullOrWhiteSpace(form.YouTubeUrl) &&
            !PostContentParser.TryGetYouTubeVideoId(form.YouTubeUrl.Trim(), out videoId))
        {
            throw new ArgumentException("Enter a valid YouTube video URL.");
        }

        if (content == null && normalizedImagePath == null && videoId == null)
            throw new ArgumentException("Add text, an image or a YouTube video.");

        return new Discussion
        {
            CategoryId = category.Id,
            Title = title,
            Content = content,
            ImagePath = normalizedImagePath,
            YouTubeVideoId = videoId,
            IsSpoiler = form.IsSpoiler
        };
    }

    private Discussion GetRequiredDiscussion(int discussionId)
    {
        return communityRepo.GetDiscussionById(discussionId, null)
            ?? throw new InvalidOperationException("Discussion was not found.");
    }

    private void EnsureUserExists(int userId)
    {
        if (userId <= 0 || userRepo.GetUserById(userId) == null)
            throw new InvalidOperationException("User was not found.");
    }

    private void EnsureCanModerateUser(int moderatorId, int targetUserId)
    {
        var moderatorRole = roleManager.GetHighestRole(moderatorId);
        var targetRole = roleManager.GetHighestRole(targetUserId);
        if (!RolePolicy.CanModerate(moderatorRole, targetRole))
            throw new InvalidOperationException("You cannot moderate this user's content.");
    }

    private static void ValidateVote(sbyte voteValue)
    {
        if (voteValue is not (-1 or 1))
            throw new ArgumentOutOfRangeException(nameof(voteValue));
    }

    private static string? NormalizeOptional(string? value, int maximumLength)
    {
        var normalized = value?.Trim();
        if (normalized?.Length > maximumLength)
            throw new ArgumentException($"The value must not exceed {maximumLength} characters.");

        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
