using LogicLayer.Enums;
using LogicLayer.Models.Community;

namespace LogicLayer.IRepos;

public interface ICommunityRepo
{
    IReadOnlyList<CommunityCategory> GetCategories();
    IReadOnlyDictionary<int, CommunityContributionStats> GetContributionStats();
    IReadOnlyList<Discussion> GetDiscussions(
        int? categoryId,
        CommunitySort sort,
        int page,
        int pageSize,
        int? viewerId);
    int GetDiscussionCount(int? categoryId);
    Discussion? GetDiscussionById(int discussionId, int? viewerId);
    int CreateDiscussion(Discussion discussion);
    bool UpdateDiscussion(Discussion discussion, int authorId);
    bool RemoveOwnDiscussion(int discussionId, int authorId);
    int SetDiscussionVote(int discussionId, int userId, sbyte voteValue);

    IReadOnlyList<DiscussionComment> GetComments(int discussionId, int? viewerId);
    DiscussionComment? GetCommentById(int commentId);
    int CreateComment(DiscussionComment comment);
    bool RemoveOwnComment(int commentId, int authorId);
    int SetCommentVote(int commentId, int userId, sbyte voteValue);

    bool CreateReport(ContentReport report);
    IReadOnlyList<ContentReport> GetPendingReports();
    bool ReviewReport(
        long reportId,
        int reviewerId,
        ReportStatus status,
        string? resolutionNote);

    bool ApplyDiscussionModeration(
        int discussionId,
        int moderatorId,
        ModerationActionType action,
        string? reason);
    bool ApplyCommentModeration(
        int commentId,
        int moderatorId,
        ModerationActionType action,
        string? reason);
}
