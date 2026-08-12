using LogicLayer.Enums;

namespace LogicLayer.Models.Community;

public class ContentReport
{
    public int Id { get; set; }
    public int ReporterId { get; set; }
    public int? DiscussionId { get; set; }
    public int? CommentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ResolutionNote { get; set; }
}
