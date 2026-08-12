using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests;

[TestClass]
public class TestRoleManager
{
    private MockRoleRepo roleRepo = null!;
    private RoleManager roleManager = null!;

    [TestInitialize]
    public void Setup()
    {
        var users = new List<User>
        {
            CreateUser(1, "owner"),
            CreateUser(2, "admin"),
            CreateUser(3, "moderator"),
            CreateUser(4, "member")
        };
        users.ForEach(user => user.EmailConfirmed = true);

        roleRepo = new MockRoleRepo();
        roleRepo.Seed(1, PlatformRole.Member, PlatformRole.Owner);
        roleRepo.Seed(2, PlatformRole.Member, PlatformRole.Admin);
        roleRepo.Seed(3, PlatformRole.Member, PlatformRole.Moderator);
        roleRepo.Seed(4, PlatformRole.Member);
        roleManager = new RoleManager(roleRepo, new MockUserRepo(users));
    }

    [TestMethod]
    public void OwnerCanPromoteMemberToAdministrator()
    {
        var changed = roleManager.AssignRole(1, 4, PlatformRole.Admin, "Trusted user");

        Assert.IsTrue(changed);
        CollectionAssert.Contains(
            roleRepo.GetRolesForUser(4).ToList(),
            PlatformRole.Admin);
    }

    [TestMethod]
    public void AdministratorCanPromoteMemberToModerator()
    {
        Assert.IsTrue(roleManager.AssignRole(2, 4, PlatformRole.Moderator, null));
    }

    [TestMethod]
    public void AdministratorCannotAssignAdministratorRole()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            roleManager.AssignRole(2, 4, PlatformRole.Admin, null));
    }

    [TestMethod]
    public void UserCannotChangeOwnRole()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            roleManager.AssignRole(1, 1, PlatformRole.Admin, null));
    }

    [TestMethod]
    public void OwnerCanRevokeAdministratorRole()
    {
        Assert.IsTrue(roleManager.RevokeRole(1, 2, PlatformRole.Admin, "Review complete"));
        CollectionAssert.DoesNotContain(
            roleRepo.GetRolesForUser(2).ToList(),
            PlatformRole.Admin);
    }

    private static User CreateUser(int id, string username)
    {
        return new User(id, "Test", "User", 22, username, $"{username}@test.local", "hash", false);
    }
}
