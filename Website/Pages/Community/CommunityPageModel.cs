using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Pages.Community;

public abstract class CommunityPageModel : PageModel
{
    private readonly FeatureOptions features;

    protected CommunityPageModel(IOptions<FeatureOptions> features)
    {
        this.features = features.Value;
    }

    protected bool CommunityEnabled => features.CommunityEnabled;

    public int? CurrentUserId =>
        int.TryParse(User.FindFirst("id")?.Value, out var userId)
            ? userId
            : null;
}
