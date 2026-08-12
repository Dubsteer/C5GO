using LogicLayer.Models;
using LogicLayer.Models.Community;
using LogicLayer.Services;

namespace Unit_Tests;

[TestClass]
public class TestRoleEligibilityPolicy
{
    [TestMethod]
    public void VerifiedContributorWithoutOpenReportsIsCandidate()
    {
        var user = CreateUser(emailConfirmed: true);
        var stats = CreateStats(contributions: 3, voteScore: 2);

        Assert.IsTrue(RoleEligibilityPolicy.IsModeratorCandidate(user, stats));
    }

    [TestMethod]
    public void UnverifiedUserIsNotCandidate()
    {
        var user = CreateUser(emailConfirmed: false);
        var stats = CreateStats(contributions: 5, voteScore: 3);

        Assert.IsFalse(RoleEligibilityPolicy.IsModeratorCandidate(user, stats));
    }

    [TestMethod]
    public void UserWithoutEnoughContributionsIsNotCandidate()
    {
        var user = CreateUser(emailConfirmed: true);
        var stats = CreateStats(contributions: 2, voteScore: 3);

        Assert.IsFalse(RoleEligibilityPolicy.IsModeratorCandidate(user, stats));
    }

    [TestMethod]
    public void UserWithOpenReportIsNotCandidate()
    {
        var user = CreateUser(emailConfirmed: true);
        var stats = CreateStats(contributions: 4, voteScore: 3);
        stats.PendingReportCount = 1;

        Assert.IsFalse(RoleEligibilityPolicy.IsModeratorCandidate(user, stats));
    }

    [TestMethod]
    public void UserWithNegativeScoreIsNotCandidate()
    {
        var user = CreateUser(emailConfirmed: true);
        var stats = CreateStats(contributions: 4, voteScore: -1);

        Assert.IsFalse(RoleEligibilityPolicy.IsModeratorCandidate(user, stats));
    }

    private static User CreateUser(bool emailConfirmed)
    {
        return new User
        {
            Id = 1,
            Username = "candidate",
            Gmail = "candidate@test.local",
            EmailConfirmed = emailConfirmed
        };
    }

    private static CommunityContributionStats CreateStats(
        int contributions,
        int voteScore)
    {
        return new CommunityContributionStats
        {
            UserId = 1,
            DiscussionCount = contributions,
            VoteScore = voteScore
        };
    }
}
