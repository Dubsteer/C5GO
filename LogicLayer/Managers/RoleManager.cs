using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Services;

namespace LogicLayer.Managers;

public class RoleManager
{
    private readonly IRoleRepo roleRepo;
    private readonly IUserRepo userRepo;

    public RoleManager(IRoleRepo roleRepo, IUserRepo userRepo)
    {
        this.roleRepo = roleRepo;
        this.userRepo = userRepo;
    }

    public IReadOnlyList<PlatformRole> GetRolesForUser(int userId)
    {
        var user = userRepo.GetUserById(userId)
            ?? throw new InvalidOperationException("User was not found.");

        var roles = roleRepo.GetRolesForUser(userId).ToHashSet();
        roles.Add(PlatformRole.Member);

        if (user.IsAdmin &&
            !roles.Contains(PlatformRole.Admin) &&
            !roles.Contains(PlatformRole.Owner))
        {
            roles.Add(PlatformRole.Admin);
        }

        return roles.OrderBy(role => role).ToArray();
    }

    public PlatformRole GetHighestRole(int userId)
    {
        return RolePolicy.GetHighestRole(GetRolesForUser(userId));
    }

    public bool AssignRole(
        int actingUserId,
        int targetUserId,
        PlatformRole role,
        string? reason)
    {
        ValidateDifferentUsers(actingUserId, targetUserId);
        ValidateManageableRole(role);

        var targetUser = userRepo.GetUserById(targetUserId)
            ?? throw new InvalidOperationException("User was not found.");
        if (!targetUser.EmailConfirmed)
            throw new InvalidOperationException("Only verified users can receive a staff role.");

        var actorRole = GetHighestRole(actingUserId);
        var targetRole = GetHighestRole(targetUserId);

        if (!RolePolicy.CanAssignRole(actorRole, role) || role <= targetRole)
            throw new InvalidOperationException("You cannot assign this role.");

        return roleRepo.AssignRole(
            targetUserId,
            role,
            actingUserId,
            NormalizeReason(reason));
    }

    public bool RevokeRole(
        int actingUserId,
        int targetUserId,
        PlatformRole role,
        string? reason)
    {
        ValidateDifferentUsers(actingUserId, targetUserId);
        ValidateManageableRole(role);

        var actorRole = GetHighestRole(actingUserId);
        var targetRoles = GetRolesForUser(targetUserId);
        var targetRole = RolePolicy.GetHighestRole(targetRoles);

        if (!targetRoles.Contains(role) ||
            !RolePolicy.CanAssignRole(actorRole, role) ||
            actorRole <= targetRole)
        {
            throw new InvalidOperationException("You cannot revoke this role.");
        }

        return roleRepo.RevokeRole(
            targetUserId,
            role,
            actingUserId,
            NormalizeReason(reason));
    }

    private static void ValidateDifferentUsers(int actingUserId, int targetUserId)
    {
        if (actingUserId <= 0 || targetUserId <= 0)
            throw new ArgumentOutOfRangeException(nameof(targetUserId));

        if (actingUserId == targetUserId)
            throw new InvalidOperationException("You cannot change your own role.");
    }

    private static void ValidateManageableRole(PlatformRole role)
    {
        if (role is PlatformRole.Member or PlatformRole.Owner || !Enum.IsDefined(role))
            throw new InvalidOperationException("This role cannot be managed here.");
    }

    private static string? NormalizeReason(string? reason)
    {
        var normalized = reason?.Trim();
        if (normalized?.Length > 255)
            throw new ArgumentException("The reason must not exceed 255 characters.", nameof(reason));

        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
