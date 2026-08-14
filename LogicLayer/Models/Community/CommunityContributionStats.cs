namespace LogicLayer.Models.Community;

public class CommunityContributionStats
{
    public int UserId { get; set; }
    public int DiscussionCount { get; set; }
    public int CommentCount { get; set; }
    public int VoteScore { get; set; }
    public int PendingReportCount { get; set; }

    public int ContributionCount => DiscussionCount + CommentCount;
}
