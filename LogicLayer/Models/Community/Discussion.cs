using LogicLayer.Enums;

namespace LogicLayer.Models.Community;

public class Discussion
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? ImagePath { get; set; }
    public string? YouTubeVideoId { get; set; }
    public bool IsSpoiler { get; set; }
    public bool IsLocked { get; set; }
    public bool IsPinned { get; set; }
    public CommunityContentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Score { get; set; }
    public int CommentCount { get; set; }
    public User? Author { get; set; }
    public CommunityCategory? Category { get; set; }
}
