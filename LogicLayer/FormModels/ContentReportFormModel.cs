using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels;

public class ContentReportFormModel
{
    public int? DiscussionId { get; set; }
    public int? CommentId { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 3)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Details { get; set; }
}
