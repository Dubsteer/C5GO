using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models.Community;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Website.Configuration;
using Website.Services;

namespace Website.Pages.Community;

[Authorize]
[EnableRateLimiting("community")]
[RequestFormLimits(MultipartBodyLengthLimit = 6 * 1024 * 1024)]
public class CreateModel : CommunityPageModel
{
    private readonly CommunityManager communityManager;
    private readonly CommunityImageStorage imageStorage;

    public CreateModel(
        CommunityManager communityManager,
        CommunityImageStorage imageStorage,
        IOptions<FeatureOptions> features)
        : base(features)
    {
        this.communityManager = communityManager;
        this.imageStorage = imageStorage;
    }

    [BindProperty]
    public DiscussionFormModel Form { get; set; } = new();

    [BindProperty]
    public IFormFile? Image { get; set; }

    public IReadOnlyList<CommunityCategory> Categories { get; private set; } = [];

    public IActionResult OnGet()
    {
        if (!CommunityEnabled)
            return NotFound();

        Categories = communityManager.GetCategories();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!CommunityEnabled)
            return NotFound();

        Categories = communityManager.GetCategories();
        if (!ModelState.IsValid)
            return Page();

        var userId = CurrentUserId;
        if (!userId.HasValue)
            return Challenge();

        string? imagePath = null;
        try
        {
            if (Image != null)
                imagePath = await imageStorage.SaveAsync(Image, cancellationToken);

            var discussionId = communityManager.CreateDiscussion(
                userId.Value,
                Form,
                imagePath);
            return RedirectToPage("./Details", new { id = discussionId });
        }
        catch (ImageUploadException exception)
        {
            ModelState.AddModelError(nameof(Image), exception.Message);
        }
        catch (ArgumentException exception)
        {
            imageStorage.Delete(imagePath);
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            imageStorage.Delete(imagePath);
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch
        {
            imageStorage.Delete(imagePath);
            throw;
        }

        return Page();
    }
}
