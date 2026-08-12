using LogicLayer.Enums;

namespace LogicLayer.Models.Community;

public class UserRoleAssignment
{
    public int UserId { get; set; }
    public PlatformRole Role { get; set; }
    public int? AssignedBy { get; set; }
    public DateTime AssignedAt { get; set; }
    public string? Reason { get; set; }
}
