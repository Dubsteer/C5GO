using LogicLayer.Dtos;
using LogicLayer.Managers;
using LogicLayer.Models;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Matches
{
    public class HistoryModel : PageModel
    {
        private const int ProfessionalMatchLimit = 20;
        private const int CommunityMatchLimit = 12;

        private readonly IExternalMatchProvider externalMatchProvider;
        private readonly MatchManager matchManager;
        private readonly TeamMatchManager teamMatchManager;

        public HistoryModel(
            IExternalMatchProvider externalMatchProvider,
            MatchManager matchManager,
            TeamMatchManager teamMatchManager)
        {
            this.externalMatchProvider = externalMatchProvider;
            this.matchManager = matchManager;
            this.teamMatchManager = teamMatchManager;
        }

        public List<ExternalMatchDto> ProfessionalMatches { get; private set; } = [];
        public List<Match> PlayerMatches { get; private set; } = [];
        public List<TeamMatch> TeamMatches { get; private set; } = [];

        public async Task OnGetAsync()
        {
            var professionalMatchesTask = externalMatchProvider.GetRecentMatchesAsync(
                ProfessionalMatchLimit);

            PlayerMatches = matchManager.GetRecentCompletedMatches(CommunityMatchLimit);
            TeamMatches = teamMatchManager.GetRecentCompletedMatches(CommunityMatchLimit);
            ProfessionalMatches = await professionalMatchesTask;
        }
    }
}
