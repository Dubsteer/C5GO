using System.Security.Claims;
using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Models.Community;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly UserManager userManager;
        private readonly PlayerManager playerManager;
        private readonly RoleManager roleManager;
        private readonly CommunityManager communityManager;
        private readonly FeatureOptions features;

        public List<User> Users { get; private set; } = new();
        public int CurrentAdminId { get; private set; }
        public PlatformRole CurrentRole { get; private set; } = PlatformRole.Admin;
        public IReadOnlyDictionary<int, IReadOnlyList<PlatformRole>> UserRoles { get; private set; } =
            new Dictionary<int, IReadOnlyList<PlatformRole>>();
        public IReadOnlyDictionary<int, CommunityContributionStats> ContributionStats { get; private set; } =
            new Dictionary<int, CommunityContributionStats>();
        public bool CommunityRolesEnabled => features.CommunityEnabled;

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IndexModel(
            UserManager userManager,
            PlayerManager playerManager,
            RoleManager roleManager,
            CommunityManager communityManager,
            IOptions<FeatureOptions> features)
        {
            this.userManager = userManager;
            this.playerManager = playerManager;
            this.roleManager = roleManager;
            this.communityManager = communityManager;
            this.features = features.Value;
        }

        public IActionResult OnGet()
        {
            if (!TryLoadCurrentAdminId())
                return Forbid();

            LoadUsers();
            return Page();
        }

        public IActionResult OnPostRemovePlayer(int id)
        {
            if (!TryLoadCurrentAdminId())
                return Forbid();

            try
            {
                playerManager.RemovePlayerRole(id);
                TempData["SuccessMessage"] = "Player profile removed.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The player profile could not be removed.";
            }

            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public IActionResult OnPostDelete(int id)
        {
            if (!TryLoadCurrentAdminId())
                return Forbid();

            try
            {
                if (features.CommunityEnabled &&
                    roleManager.GetHighestRole(id) > PlatformRole.Member)
                {
                    throw new InvalidOperationException(
                        "Revoke the staff role before deleting this account.");
                }

                userManager.DeleteUserAsAdmin(
                    id,
                    CurrentAdminId,
                    User.IsInRole(PlatformRole.Owner.ToString()) ||
                    User.IsInRole(PlatformRole.Admin.ToString()));
                TempData["SuccessMessage"] = "User account deleted.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] =
                    "The account could not be deleted because it is still connected to other data.";
            }

            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public IActionResult OnPostAssignRole(
            int id,
            PlatformRole role,
            string? reason)
        {
            if (!features.CommunityEnabled)
                return NotFound();
            if (!TryLoadCurrentAdminId())
                return Forbid();

            try
            {
                var changed = roleManager.AssignRole(CurrentAdminId, id, role, reason);
                TempData[changed ? "SuccessMessage" : "ErrorMessage"] = changed
                    ? $"{role} role assigned. Access updates on the user's next request."
                    : "The user already has this role.";
            }
            catch (Exception exception) when (
                exception is ArgumentException or InvalidOperationException)
            {
                TempData["ErrorMessage"] = exception.Message;
            }

            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public IActionResult OnPostRevokeRole(
            int id,
            PlatformRole role,
            string? reason)
        {
            if (!features.CommunityEnabled)
                return NotFound();
            if (!TryLoadCurrentAdminId())
                return Forbid();

            try
            {
                var changed = roleManager.RevokeRole(CurrentAdminId, id, role, reason);
                TempData[changed ? "SuccessMessage" : "ErrorMessage"] = changed
                    ? $"{role} role revoked. Access updates on the user's next request."
                    : "The user does not have this role.";
            }
            catch (Exception exception) when (
                exception is ArgumentException or InvalidOperationException)
            {
                TempData["ErrorMessage"] = exception.Message;
            }

            return RedirectToPage(new { searchTerm = SearchTerm });
        }

        public IReadOnlyList<PlatformRole> GetRoles(User user)
        {
            return user.Id is int userId && UserRoles.TryGetValue(userId, out var roles)
                ? roles
                : [user.IsAdmin ? PlatformRole.Admin : PlatformRole.Member];
        }

        public bool IsPlayer(User user) =>
            !string.IsNullOrWhiteSpace(user.SteamId) && user.SteamId != "0";

        public CommunityContributionStats GetContributionStats(User user)
        {
            return user.Id is int userId && ContributionStats.TryGetValue(userId, out var stats)
                ? stats
                : new CommunityContributionStats { UserId = user.Id ?? 0 };
        }

        public bool IsModeratorCandidate(User user)
        {
            return GetRoles(user).Max() == PlatformRole.Member &&
                   RoleEligibilityPolicy.IsModeratorCandidate(
                       user,
                       GetContributionStats(user));
        }

        private bool TryLoadCurrentAdminId()
        {
            var claim = User.FindFirstValue("id");
            if (!int.TryParse(claim, out var currentAdminId))
                return false;

            CurrentAdminId = currentAdminId;
            return true;
        }

        private void LoadUsers()
        {
            Users = string.IsNullOrWhiteSpace(SearchTerm)
                ? userManager.GetAllUsers()
                : userManager.SearchUser(SearchTerm.Trim());

            Users = Users.OrderBy(user => user.Username).ToList();

            if (!features.CommunityEnabled)
                return;

            UserRoles = Users
                .Where(user => user.Id.HasValue)
                .ToDictionary(
                    user => user.Id!.Value,
                    user => roleManager.GetRolesForUser(user.Id!.Value));
            ContributionStats = communityManager.GetContributionStats();
            CurrentRole = roleManager.GetHighestRole(CurrentAdminId);
        }
    }
}
