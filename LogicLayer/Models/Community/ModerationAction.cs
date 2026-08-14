using LogicLayer.Enums;

namespace LogicLayer.Models.Community;

public class ModerationAction
{
    public long Id { get; set; }
    public int? ModeratorId { get; set; }
    public ModerationActionType ActionType { get; set; }
    public ModerationTargetType TargetType { get; set; }
    public long TargetId { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
}
