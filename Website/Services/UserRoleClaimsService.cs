using System.Security.Claims;
using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Services;

public sealed class UserRoleClaimsService
{
    private readonly RoleManager roleManager;
    private readonly FeatureOptions features;

    public UserRoleClaimsService(
        RoleManager roleManager,
        IOptions<FeatureOptions> features)
    {
        this.roleManager = roleManager;
        this.features = features.Value;
    }

    public IEnumerable<Claim> CreateRoleClaims(User user)
    {
        if (!features.CommunityEnabled || user.Id is not int userId)
        {
            if (user.IsAdmin)
                yield return new Claim(ClaimTypes.Role, PlatformRole.Admin.ToString());

            yield break;
        }

        foreach (var role in roleManager.GetRolesForUser(userId))
            yield return new Claim(ClaimTypes.Role, role.ToString());
    }
}
