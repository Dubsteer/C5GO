using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Models.Community;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Website.Configuration;

namespace Website.Pages.Community;

public class IndexModel : CommunityPageModel
{
    private readonly CommunityManager communityManager;

    public IndexModel(
        CommunityManager communityManager,
        IOptions<FeatureOptions> features)
        : base(features)
    {
        this.communityManager = communityManager;
    }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public CommunitySort Sort { get; set; } = CommunitySort.Active;

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public IReadOnlyList<CommunityCategory> Categories { get; private set; } = [];
    public PagedResult<Discussion> Discussions { get; private set; } =
        new([], 1, 12, 0);

    public IActionResult OnGet()
    {
        if (!CommunityEnabled)
            return NotFound();

        if (!Enum.IsDefined(Sort))
            Sort = CommunitySort.Active;

        Categories = communityManager.GetCategories();
        if (CategoryId.HasValue && Categories.All(item => item.Id != CategoryId.Value))
            CategoryId = null;

        Discussions = communityManager.GetDiscussions(
            CategoryId,
            Sort,
            PageNumber,
            12,
            CurrentUserId);
        PageNumber = Discussions.Page;
        return Page();
    }
}
