using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels;

public class DiscussionCommentFormModel
{
    [Range(1, int.MaxValue)]
    public int DiscussionId { get; set; }

    public int? ParentCommentId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}
