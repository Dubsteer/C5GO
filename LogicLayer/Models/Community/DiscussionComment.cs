using LogicLayer.Enums;

namespace LogicLayer.Models.Community;

public class DiscussionComment
{
    public int Id { get; set; }
    public int DiscussionId { get; set; }
    public int AuthorId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public CommunityContentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Score { get; set; }
    public sbyte CurrentUserVote { get; set; }
    public User? Author { get; set; }
    public IReadOnlyList<DiscussionComment> Replies { get; set; } = [];
}
