using LogicLayer.Enums;

namespace Website.Models;

public sealed class PublicUserProfileViewModel
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public PlatformRole HighestRole { get; init; }
    public bool HasPlayerProfile { get; init; }
    public int? TeamId { get; init; }
    public string? TeamName { get; init; }
    public bool IsTeamCaptain { get; init; }
    public bool IsSteamProfilePublic { get; init; }
    public bool CommunityEnabled { get; init; }
    public int DiscussionCount { get; init; }
    public int CommentCount { get; init; }
    public int VoteScore { get; init; }
}
