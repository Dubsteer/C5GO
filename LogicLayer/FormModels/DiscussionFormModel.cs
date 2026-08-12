using System.ComponentModel.DataAnnotations;

namespace LogicLayer.FormModels;

public class DiscussionFormModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Choose a category.")]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(160, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [StringLength(10000)]
    public string? Content { get; set; }

    [StringLength(500)]
    [Display(Name = "YouTube URL")]
    public string? YouTubeUrl { get; set; }

    [Display(Name = "Mark media as spoiler")]
    public bool IsSpoiler { get; set; }
}
