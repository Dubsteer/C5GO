using LogicLayer.Models;
using LogicLayer.Models.Community;

namespace LogicLayer.Services;

public static class RoleEligibilityPolicy
{
    public const int MinimumContributions = 3;

    public static bool IsModeratorCandidate(
        User user,
        CommunityContributionStats stats)
    {
        return user.EmailConfirmed &&
               stats.ContributionCount >= MinimumContributions &&
               stats.VoteScore >= 0 &&
               stats.PendingReportCount == 0;
    }
}
