using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models.Community;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Website.Configuration;
using Website.Pages.Community;

namespace Website.Pages.Moderation;

[Authorize(Policy = "ModeratorOnly")]
[EnableRateLimiting("community")]
public class ReportsModel : CommunityPageModel
{
    private readonly CommunityManager communityManager;

    public ReportsModel(
        CommunityManager communityManager,
        IOptions<FeatureOptions> features)
        : base(features)
    {
        this.communityManager = communityManager;
    }

    public IReadOnlyList<ContentReport> Reports { get; private set; } = [];

    public IActionResult OnGet()
    {
        if (!CommunityEnabled)
            return NotFound();
        if (!CurrentUserId.HasValue)
            return Challenge();

        Reports = communityManager.GetPendingReports(CurrentUserId.Value);
        return Page();
    }

    public IActionResult OnPostReview(
        long reportId,
        ReportStatus status,
        string? resolutionNote)
    {
        if (!CommunityEnabled)
            return NotFound();
        if (!CurrentUserId.HasValue)
            return Challenge();

        try
        {
            communityManager.ReviewReport(
                reportId,
                CurrentUserId.Value,
                status,
                resolutionNote);
            TempData["CommunityMessage"] = "Report reviewed.";
        }
        catch (Exception exception) when (
            exception is ArgumentException or InvalidOperationException)
        {
            TempData["CommunityError"] = exception.Message;
        }

        return RedirectToPage();
    }
}
