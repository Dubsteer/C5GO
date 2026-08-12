using LogicLayer.Models.Community;

namespace Website.Models;

public class CommunityCommentsViewModel
{
    public int DiscussionId { get; set; }
    public IReadOnlyList<DiscussionComment> Comments { get; set; } = [];
    public int? CurrentUserId { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool CanModerate { get; set; }
    public IReadOnlyList<string> ReportReasons { get; set; } = [];
}
