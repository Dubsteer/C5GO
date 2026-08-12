using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models.Community;

namespace Unit_Tests.MockRepos;

public class MockCommunityRepo : ICommunityRepo
{
    private readonly Dictionary<(int TargetId, int UserId), sbyte> discussionVotes = [];
    private readonly Dictionary<(int TargetId, int UserId), sbyte> commentVotes = [];

    public List<CommunityCategory> Categories { get; } =
    [
        new()
        {
            Id = 1,
            Slug = "general",
            Name = "General",
            Description = "General discussions",
            DisplayOrder = 1,
            IsActive = true
        }
    ];

    public List<Discussion> Discussions { get; } = [];
    public List<DiscussionComment> Comments { get; } = [];
    public List<ContentReport> Reports { get; } = [];

    public IReadOnlyList<CommunityCategory> GetCategories() => Categories;

    public IReadOnlyDictionary<int, CommunityContributionStats> GetContributionStats()
    {
        return Discussions
            .Select(item => item.AuthorId)
            .Concat(Comments.Select(item => item.AuthorId))
            .Distinct()
            .ToDictionary(
                userId => userId,
                userId => new CommunityContributionStats
                {
                    UserId = userId,
                    DiscussionCount = Discussions.Count(item =>
                        item.AuthorId == userId &&
                        item.Status == CommunityContentStatus.Published),
                    CommentCount = Comments.Count(item =>
                        item.AuthorId == userId &&
                        item.Status == CommunityContentStatus.Published),
                    VoteScore = Discussions
                        .Where(item => item.AuthorId == userId)
                        .Sum(item => item.Score) + Comments
                        .Where(item => item.AuthorId == userId)
                        .Sum(item => item.Score),
                    PendingReportCount = Reports.Count(item =>
                        item.Status == ReportStatus.Pending &&
                        ((item.DiscussionId.HasValue && Discussions.Any(discussion =>
                            discussion.Id == item.DiscussionId && discussion.AuthorId == userId)) ||
                         (item.CommentId.HasValue && Comments.Any(comment =>
                            comment.Id == item.CommentId && comment.AuthorId == userId))))
                });
    }

    public IReadOnlyList<Discussion> GetDiscussions(
        int? categoryId,
        CommunitySort sort,
        int page,
        int pageSize,
        int? viewerId)
    {
        var query = Discussions
            .Where(item => item.Status == CommunityContentStatus.Published);
        if (categoryId.HasValue)
            query = query.Where(item => item.CategoryId == categoryId.Value);

        query = sort switch
        {
            CommunitySort.Top => query.OrderByDescending(item => item.Score),
            _ => query.OrderByDescending(item => item.CreatedAt)
        };

        return query.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
    }

    public int GetDiscussionCount(int? categoryId) =>
        Discussions.Count(item =>
            item.Status == CommunityContentStatus.Published &&
            (!categoryId.HasValue || item.CategoryId == categoryId.Value));

    public Discussion? GetDiscussionById(int discussionId, int? viewerId) =>
        Discussions.FirstOrDefault(item => item.Id == discussionId);

    public int CreateDiscussion(Discussion discussion)
    {
        discussion.Id = Discussions.Count + 1;
        discussion.Category = Categories.First(item => item.Id == discussion.CategoryId);
        Discussions.Add(discussion);
        return discussion.Id;
    }

    public bool UpdateDiscussion(Discussion discussion, int authorId)
    {
        var index = Discussions.FindIndex(item =>
            item.Id == discussion.Id && item.AuthorId == authorId);
        if (index < 0)
            return false;

        Discussions[index] = discussion;
        return true;
    }

    public bool RemoveOwnDiscussion(int discussionId, int authorId)
    {
        var discussion = Discussions.FirstOrDefault(item =>
            item.Id == discussionId && item.AuthorId == authorId);
        if (discussion == null)
            return false;
        discussion.Status = CommunityContentStatus.Removed;
        return true;
    }

    public int SetDiscussionVote(int discussionId, int userId, sbyte voteValue)
    {
        ToggleVote(discussionVotes, discussionId, userId, voteValue);
        var score = discussionVotes
            .Where(item => item.Key.TargetId == discussionId)
            .Sum(item => item.Value);
        Discussions.First(item => item.Id == discussionId).Score = score;
        return score;
    }

    public IReadOnlyList<DiscussionComment> GetComments(int discussionId, int? viewerId) =>
        Comments.Where(item => item.DiscussionId == discussionId).ToArray();

    public DiscussionComment? GetCommentById(int commentId) =>
        Comments.FirstOrDefault(item => item.Id == commentId);

    public int CreateComment(DiscussionComment comment)
    {
        comment.Id = Comments.Count + 1;
        Comments.Add(comment);
        return comment.Id;
    }

    public bool RemoveOwnComment(int commentId, int authorId)
    {
        var comment = Comments.FirstOrDefault(item =>
            item.Id == commentId && item.AuthorId == authorId);
        if (comment == null)
            return false;
        comment.Status = CommunityContentStatus.Removed;
        return true;
    }

    public int SetCommentVote(int commentId, int userId, sbyte voteValue)
    {
        ToggleVote(commentVotes, commentId, userId, voteValue);
        var score = commentVotes
            .Where(item => item.Key.TargetId == commentId)
            .Sum(item => item.Value);
        Comments.First(item => item.Id == commentId).Score = score;
        return score;
    }

    public bool CreateReport(ContentReport report)
    {
        if (Reports.Any(item =>
                item.ReporterId == report.ReporterId &&
                item.DiscussionId == report.DiscussionId &&
                item.CommentId == report.CommentId))
        {
            return false;
        }

        report.Id = Reports.Count + 1;
        Reports.Add(report);
        return true;
    }

    public IReadOnlyList<ContentReport> GetPendingReports() =>
        Reports.Where(item => item.Status == ReportStatus.Pending).ToArray();

    public bool ReviewReport(
        long reportId,
        int reviewerId,
        ReportStatus status,
        string? resolutionNote)
    {
        var report = Reports.FirstOrDefault(item => item.Id == reportId);
        if (report == null || report.Status != ReportStatus.Pending)
            return false;
        report.Status = status;
        report.ReviewedBy = reviewerId;
        report.ResolutionNote = resolutionNote;
        return true;
    }

    public bool ApplyDiscussionModeration(
        int discussionId,
        int moderatorId,
        ModerationActionType action,
        string? reason)
    {
        var discussion = Discussions.FirstOrDefault(item => item.Id == discussionId);
        if (discussion == null)
            return false;

        switch (action)
        {
            case ModerationActionType.LockDiscussion:
                discussion.IsLocked = true;
                break;
            case ModerationActionType.UnlockDiscussion:
                discussion.IsLocked = false;
                break;
            case ModerationActionType.PinDiscussion:
                discussion.IsPinned = true;
                break;
            case ModerationActionType.UnpinDiscussion:
                discussion.IsPinned = false;
                break;
            case ModerationActionType.RemoveDiscussion:
                discussion.Status = CommunityContentStatus.Removed;
                break;
            case ModerationActionType.RestoreDiscussion:
                discussion.Status = CommunityContentStatus.Published;
                break;
        }

        return true;
    }

    public bool ApplyCommentModeration(
        int commentId,
        int moderatorId,
        ModerationActionType action,
        string? reason)
    {
        var comment = Comments.FirstOrDefault(item => item.Id == commentId);
        if (comment == null)
            return false;
        comment.Status = action == ModerationActionType.RemoveComment
            ? CommunityContentStatus.Removed
            : CommunityContentStatus.Published;
        return true;
    }

    private static void ToggleVote(
        Dictionary<(int TargetId, int UserId), sbyte> votes,
        int targetId,
        int userId,
        sbyte voteValue)
    {
        var key = (targetId, userId);
        if (votes.TryGetValue(key, out var existing) && existing == voteValue)
            votes.Remove(key);
        else
            votes[key] = voteValue;
    }
}
