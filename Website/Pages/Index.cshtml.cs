using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Services;
using LogicLayer.Enums;
using LogicLayer.Models.Community;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Pages
{
    public class IndexModel : PageModel
    {
        private const int LatestDiscussionLimit = 10;

        private readonly PostManager postManager;
        private readonly CommunityManager communityManager;
        private readonly FeatureOptions features;
        private readonly ILogger<IndexModel> logger;

        public List<Post> Posts { get; set; } = [];
        public IReadOnlyList<Discussion> LatestDiscussions { get; private set; } = [];
        public bool ShowCommunitySidebar => features.CommunityEnabled;
        public bool CommunityFeedUnavailable { get; private set; }

        public IndexModel(
            PostManager postManager,
            CommunityManager communityManager,
            IOptions<FeatureOptions> features,
            ILogger<IndexModel> logger)
        {
            this.postManager = postManager;
            this.communityManager = communityManager;
            this.features = features.Value;
            this.logger = logger;
        }

        public IActionResult OnGet()
        {
            ViewData["Message"] = TempData["Message"];
            TempData.Clear();

            try
            {
                Posts = postManager.GetAllPosts();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to load news posts.");
                return StatusCode(500);
            }

            if (features.CommunityEnabled)
            {
                try
                {
                    LatestDiscussions = communityManager.GetDiscussions(
                        categoryId: null,
                        sort: CommunitySort.Newest,
                        page: 1,
                        pageSize: LatestDiscussionLimit,
                        viewerId: null).Items;
                }
                catch (Exception ex)
                {
                    CommunityFeedUnavailable = true;
                    logger.LogWarning(ex, "Unable to load the Community preview on the News page.");
                }
            }

            return Page();
        }

        public string TruncateString(string input, int maxLength)
        {
            return input.Length > maxLength
                ? input[..maxLength] + "..."
                : input;
        }

        public string GetPostPreview(string content)
        {
            var preview = PostContentParser.GetPreviewText(content);
            return string.IsNullOrWhiteSpace(preview)
                ? "Open this announcement for more details."
                : preview;
        }
    }
}
