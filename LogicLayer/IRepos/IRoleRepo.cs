using LogicLayer.Enums;

namespace LogicLayer.IRepos;

public interface IRoleRepo
{
    IReadOnlyList<PlatformRole> GetRolesForUser(int userId);
    bool AssignRole(int userId, PlatformRole role, int? assignedBy, string? reason);
    bool RevokeRole(int userId, PlatformRole role, int? performedBy, string? reason);
}
