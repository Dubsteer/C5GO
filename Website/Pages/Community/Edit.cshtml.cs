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
public class EditModel : CommunityPageModel
{
    private readonly CommunityManager communityManager;
    private readonly CommunityImageStorage imageStorage;

    public EditModel(
        CommunityManager communityManager,
        CommunityImageStorage imageStorage,
        IOptions<FeatureOptions> features)
        : base(features)
    {
        this.communityManager = communityManager;
        this.imageStorage = imageStorage;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty]
    public DiscussionFormModel Form { get; set; } = new();

    [BindProperty]
    public IFormFile? Image { get; set; }

    [BindProperty]
    public bool RemoveImage { get; set; }

    public string? ExistingImagePath { get; private set; }
    public IReadOnlyList<CommunityCategory> Categories { get; private set; } = [];

    public IActionResult OnGet()
    {
        if (!CommunityEnabled)
            return NotFound();

        var discussion = communityManager.GetDiscussion(Id, CurrentUserId);
        if (discussion == null)
            return NotFound();
        if (CurrentUserId != discussion.AuthorId)
            return Forbid();

        Categories = communityManager.GetCategories();
        ExistingImagePath = discussion.ImagePath;
        Form = new DiscussionFormModel
        {
            CategoryId = discussion.CategoryId,
            Title = discussion.Title,
            Content = discussion.Content,
            YouTubeUrl = discussion.YouTubeVideoId == null
                ? null
                : $"https://youtu.be/{discussion.YouTubeVideoId}",
            IsSpoiler = discussion.IsSpoiler
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!CommunityEnabled)
            return NotFound();

        var userId = CurrentUserId;
        if (!userId.HasValue)
            return Challenge();

        var existing = communityManager.GetDiscussion(Id, userId);
        if (existing == null)
            return NotFound();
        if (existing.AuthorId != userId.Value)
            return Forbid();

        Categories = communityManager.GetCategories();
        ExistingImagePath = existing.ImagePath;
        if (!ModelState.IsValid)
            return Page();

        string? newImagePath = null;
        try
        {
            if (Image != null)
                newImagePath = await imageStorage.SaveAsync(Image, cancellationToken);

            var selectedImagePath = newImagePath ?? (RemoveImage ? null : existing.ImagePath);
            if (!communityManager.UpdateDiscussion(
                    Id,
                    userId.Value,
                    Form,
                    selectedImagePath))
            {
                throw new InvalidOperationException("The discussion could not be updated.");
            }

            if ((RemoveImage || newImagePath != null) && existing.ImagePath != null)
                imageStorage.Delete(existing.ImagePath);

            return RedirectToPage("./Details", new { id = Id });
        }
        catch (ImageUploadException exception)
        {
            ModelState.AddModelError(nameof(Image), exception.Message);
        }
        catch (ArgumentException exception)
        {
            imageStorage.Delete(newImagePath);
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            imageStorage.Delete(newImagePath);
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch
        {
            imageStorage.Delete(newImagePath);
            throw;
        }

        return Page();
    }
}
