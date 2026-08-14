using LogicLayer.Enums;
using LogicLayer.IRepos;

namespace Unit_Tests.MockRepos;

public class MockRoleRepo : IRoleRepo
{
    private readonly Dictionary<int, HashSet<PlatformRole>> roles = [];

    public IReadOnlyList<PlatformRole> GetRolesForUser(int userId)
    {
        return roles.TryGetValue(userId, out var userRoles)
            ? userRoles.OrderBy(role => role).ToArray()
            : [];
    }

    public bool AssignRole(
        int userId,
        PlatformRole role,
        int? assignedBy,
        string? reason)
    {
        if (!roles.TryGetValue(userId, out var userRoles))
        {
            userRoles = [];
            roles[userId] = userRoles;
        }

        return userRoles.Add(role);
    }

    public bool RevokeRole(
        int userId,
        PlatformRole role,
        int? performedBy,
        string? reason)
    {
        return roles.TryGetValue(userId, out var userRoles) && userRoles.Remove(role);
    }

    public void Seed(int userId, params PlatformRole[] assignedRoles)
    {
        roles[userId] = assignedRoles.ToHashSet();
    }
}
