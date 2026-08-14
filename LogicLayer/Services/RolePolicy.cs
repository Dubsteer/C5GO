using LogicLayer.Enums;

namespace LogicLayer.Services;

public static class RolePolicy
{
    public static PlatformRole GetHighestRole(IEnumerable<PlatformRole>? roles)
    {
        if (roles == null)
            return PlatformRole.Member;

        return roles
            .Where(Enum.IsDefined)
            .DefaultIfEmpty(PlatformRole.Member)
            .Max();
    }

    public static bool CanAssignRole(PlatformRole actorRole, PlatformRole roleToAssign)
    {
        return actorRole switch
        {
            PlatformRole.Owner => roleToAssign is PlatformRole.Admin or PlatformRole.Moderator,
            PlatformRole.Admin => roleToAssign == PlatformRole.Moderator,
            _ => false
        };
    }

    public static bool CanModerate(PlatformRole actorRole, PlatformRole targetRole)
    {
        return actorRole >= PlatformRole.Moderator &&
               actorRole > targetRole &&
               targetRole != PlatformRole.Owner;
    }

    public static bool CanModerateContent(PlatformRole role)
    {
        return role >= PlatformRole.Moderator;
    }
}
