using LogicLayer.Enums;
using LogicLayer.Services;

namespace Unit_Tests;

[TestClass]
public class TestRolePolicy
{
    [TestMethod]
    public void GetHighestRoleReturnsMostPrivilegedRole()
    {
        var role = RolePolicy.GetHighestRole(
            [PlatformRole.Member, PlatformRole.Moderator, PlatformRole.Admin]);

        Assert.AreEqual(PlatformRole.Admin, role);
    }

    [TestMethod]
    public void GetHighestRoleDefaultsToMember()
    {
        Assert.AreEqual(PlatformRole.Member, RolePolicy.GetHighestRole([]));
        Assert.AreEqual(PlatformRole.Member, RolePolicy.GetHighestRole(null));
    }

    [TestMethod]
    [DataRow(PlatformRole.Owner, PlatformRole.Admin, true)]
    [DataRow(PlatformRole.Owner, PlatformRole.Moderator, true)]
    [DataRow(PlatformRole.Admin, PlatformRole.Moderator, true)]
    [DataRow(PlatformRole.Admin, PlatformRole.Admin, false)]
    [DataRow(PlatformRole.Moderator, PlatformRole.Moderator, false)]
    public void CanAssignRoleEnforcesHierarchy(
        PlatformRole actorRole,
        PlatformRole roleToAssign,
        bool expected)
    {
        Assert.AreEqual(expected, RolePolicy.CanAssignRole(actorRole, roleToAssign));
    }

    [TestMethod]
    [DataRow(PlatformRole.Owner, PlatformRole.Admin, true)]
    [DataRow(PlatformRole.Admin, PlatformRole.Moderator, true)]
    [DataRow(PlatformRole.Moderator, PlatformRole.Member, true)]
    [DataRow(PlatformRole.Moderator, PlatformRole.Admin, false)]
    [DataRow(PlatformRole.Member, PlatformRole.Member, false)]
    public void CanModerateRequiresHigherPrivilegedRole(
        PlatformRole actorRole,
        PlatformRole targetRole,
        bool expected)
    {
        Assert.AreEqual(expected, RolePolicy.CanModerate(actorRole, targetRole));
    }
}
