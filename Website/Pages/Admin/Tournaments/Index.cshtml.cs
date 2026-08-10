using LogicLayer.Enums;
using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.Tournaments
{
    public class IndexModel : PageModel
    {
        private readonly TournamentManager tournamentManager;

        public List<Tournament> Tournaments { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "all";

        public IndexModel(TournamentManager tournamentManager)
        {
            this.tournamentManager = tournamentManager;
        }

        public void OnGet()
        {
            var tournaments = tournamentManager.GetAllTournaments();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim();
                tournaments = tournaments
                    .Where(tournament =>
                        tournament.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        tournament.Description.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            tournaments = (Filter ?? "all").ToLowerInvariant() switch
            {
                "solo" => tournaments.Where(tournament => !tournament.IsTeamTournament).ToList(),
                "team" => tournaments.Where(tournament => tournament.IsTeamTournament).ToList(),
                "open" => tournaments.Where(tournament => tournament.Status == Status.Open).ToList(),
                "active" => tournaments.Where(tournament => tournament.Status == Status.InProgress).ToList(),
                "closed" => tournaments.Where(tournament => tournament.Status == Status.Closed).ToList(),
                _ => tournaments
            };

            Tournaments = tournaments.OrderByDescending(tournament => tournament.Id).ToList();
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                var tournament = tournamentManager.GetTournamentById(id);
                tournamentManager.RemoveTournament(tournament);
                TempData["SuccessMessage"] = "Tournament deleted.";
            }
            catch (TournamentNotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch
            {
                TempData["ErrorMessage"] = "The tournament could not be deleted.";
            }

            return RedirectToPage();
        }
    }
}
