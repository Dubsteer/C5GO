using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests;

[TestClass]
public class TestTeamManagerNotifications
{
    private User captain = null!;
    private User member = null!;
    private User secondMember = null!;
    private List<User> users = null!;
    private MockTeamRepo teamRepo = null!;
    private MockNotificationRepo notificationRepo = null!;
    private TeamManager manager = null!;

    [TestInitialize]
    public void Setup()
    {
        captain = CreateUser(1, "captain");
        member = CreateUser(2, "member");
        secondMember = CreateUser(3, "second-member");
        users = [captain, member, secondMember];
        teamRepo = new MockTeamRepo(users);
        teamRepo.SeedTeam(
            new Team(1, "C5GO Squad", captain),
            captain,
            member,
            secondMember);
        notificationRepo = new MockNotificationRepo();
        manager = new TeamManager(teamRepo, new MockUserRepo(users), notificationRepo);
    }

    [TestMethod]
    public void CreatingTeamTrimsNameAndUsesExistingCaptain()
    {
        manager.CreateTeam("  New Team  ", captain.Id!.Value);

        Assert.IsNotNull(teamRepo.Teams.SingleOrDefault(team => team.Name == "New Team"));
    }

    [TestMethod]
    public void CreatingTeamRejectsMissingCaptain()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            manager.CreateTeam("New Team", 999));
    }

    [TestMethod]
    public void CreatingTeamRequiresCaptainSteamId()
    {
        var userWithoutSteamId = CreateUser(4, "no-steam");
        userWithoutSteamId.SteamId = null;
        users.Add(userWithoutSteamId);

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            manager.CreateTeam("New Team", userWithoutSteamId.Id!.Value));
    }

    [TestMethod]
    public void LeavingMemberNotifiesCaptain()
    {
        manager.LeaveTeam(member.Id!.Value);

        Assert.HasCount(1, notificationRepo.Notifications);
        Assert.AreEqual(captain.Id, notificationRepo.Notifications[0].UserId);
        StringAssert.Contains(notificationRepo.Notifications[0].Message, member.Username);
        Assert.IsNull(teamRepo.GetTeamByUser(member.Id.Value));
    }

    [TestMethod]
    public void KickedMemberReceivesNotification()
    {
        manager.KickMember(captain.Id!.Value, member.Id!.Value);

        Assert.HasCount(1, notificationRepo.Notifications);
        Assert.AreEqual(member.Id, notificationRepo.Notifications[0].UserId);
        StringAssert.Contains(notificationRepo.Notifications[0].Message, "removed");
        Assert.IsNull(teamRepo.GetTeamByUser(member.Id.Value));
    }

    [TestMethod]
    public void DisbandingTeamNotifiesEveryOtherMember()
    {
        manager.LeaveTeam(captain.Id!.Value);

        Assert.HasCount(2, notificationRepo.Notifications);
        CollectionAssert.AreEquivalent(
            new[] { member.Id!.Value, secondMember.Id!.Value },
            notificationRepo.Notifications.Select(item => item.UserId).ToArray());
        Assert.IsNull(teamRepo.GetTeamById(1));
    }

    private static User CreateUser(int id, string username) =>
        new(id, "Test", "User", 22, username, $"{username}@test.local", "hash", false, $"7656119800000000{id}");
}
