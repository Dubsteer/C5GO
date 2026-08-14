using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using LogicLayer.Managers;
using LogicLayer.Services;
using LogicLayer.Enums;
using Microsoft.Extensions.Options;
using Website.Configuration;
using Website.Models;

namespace Website.Pages
{
    public class UserProfileModel : PageModel
    {
        public PublicUserProfileViewModel Profile { get; private set; } = new();
        public PlayerMatchHistoryViewModel MatchHistory { get; private set; } = new();
        public bool IsOwnProfile { get; private set; }

        private readonly UserManager userManager;
        private readonly PlayerManager playerManager;
        private readonly RoleManager roleManager;
        private readonly MatchManager matchManager;
        private readonly TeamManager teamManager;
        private readonly CommunityManager communityManager;
        private readonly FeatureOptions features;

        public UserProfileModel(
            UserManager um,
            PlayerManager pm,
            RoleManager roleManager,
            MatchManager matchManager,
            TeamManager teamManager,
            CommunityManager communityManager,
            IOptions<FeatureOptions> features)
        {
            userManager = um;
            playerManager = pm;
            this.roleManager = roleManager;
            this.matchManager = matchManager;
            this.teamManager = teamManager;
            this.communityManager = communityManager;
            this.features = features.Value;
        }

        public IActionResult OnGet(int id)
        {
            var user = userManager.GetUserById(id);
            if (user == null)
                return NotFound();

            var player = playerManager.GetPlayer(user);
            var matches = player == null
                ? []
                : matchManager.GetPastMatches(user, 5);
            var team = teamManager.GetTeamOfUser(id);
            var highestRole = features.CommunityEnabled
                ? roleManager.GetHighestRole(id)
                : user.IsAdmin ? PlatformRole.Admin : PlatformRole.Member;
            var contributionStats = features.CommunityEnabled &&
                                    communityManager.GetContributionStats()
                                        .TryGetValue(id, out var stats)
                ? stats
                : null;
            Profile = new PublicUserProfileViewModel
            {
                UserId = id,
                Username = user.Username,
                HighestRole = highestRole,
                HasPlayerProfile = player != null,
                TeamId = team?.Id,
                TeamName = team?.Name,
                IsTeamCaptain = team?.Captain.Id == id,
                IsSteamProfilePublic = user.ShowSteamProfile && player != null,
                CommunityEnabled = features.CommunityEnabled,
                DiscussionCount = contributionStats?.DiscussionCount ?? 0,
                CommentCount = contributionStats?.CommentCount ?? 0,
                VoteScore = contributionStats?.VoteScore ?? 0
            };
            MatchHistory = new PlayerMatchHistoryViewModel
            {
                UserId = user.Id.GetValueOrDefault(),
                Matches = matches,
                EmptyMessage = "Completed C5GO solo tournament matches will appear here."
            };
            IsOwnProfile = int.TryParse(User.FindFirst("id")?.Value, out var currentUserId) &&
                           currentUserId == id;

            return Page();
        }

        public IActionResult OnGetSteam(int id)
        {
            var user = userManager.GetUserById(id);
            if (user is not { ShowSteamProfile: true })
                return NotFound();

            var player = playerManager.GetPlayer(user);
            var steamProfileUrl = player?.SteamId is string steamId
                ? SteamIdParser.BuildProfileUrl(steamId)
                : null;

            return steamProfileUrl == null
                ? NotFound()
                : Redirect(steamProfileUrl);
        }
    }
}
