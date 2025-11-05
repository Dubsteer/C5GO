using LogicLayer.Models;
using LogicLayer.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Website.Pages
{
    [Authorize]
    public class TournamentsModel : PageModel
    {
        public List<Tournament> Tournaments { get; set; }
        public Player CurrentPlayer { get; set; }

        private readonly UserManager userManager;
        private readonly TournamentManager tournamentmanager;
        private readonly PlayerManager playerManager;

        public TournamentsModel(TournamentManager tournamentmanager, PlayerManager playerManager, UserManager userManager)
        {
            this.tournamentmanager = tournamentmanager;
            this.playerManager = playerManager;
            this.userManager = userManager;
        }

        private void LoadCurrentPlayer()
        {
            var u = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));
            CurrentPlayer = playerManager.GetPlayer(u);
        }

        public IActionResult OnGet()
        {
            LoadCurrentPlayer();

            Tournaments = tournamentmanager.GetAllTournaments();

            foreach (var t in Tournaments)
            {
                t.Players = tournamentmanager.GetAllPlayersInTournament(t);
                t.Matches = tournamentmanager.GetAllMatchesInTournament(t);
            }

            return Page();
        }

        public IActionResult OnPostApply(int id)
        {
            try
            {
                LoadCurrentPlayer();

                var t = tournamentmanager.GetTournamentById(id);
                t.Players = tournamentmanager.GetAllPlayersInTournament(t);

                if (!t.IsClosed && !t.Players.Any(p => p.Id == CurrentPlayer.Id))
                {
                    tournamentmanager.AddTournamentApp(CurrentPlayer, t);
                }
            }
            catch (Exception)
            {
            }

            return Redirect("/Tournaments");
        }

        public IActionResult OnPostLeave(int id)
        {
            try
            {
                LoadCurrentPlayer();

                var t = tournamentmanager.GetTournamentById(id);
                tournamentmanager.RemovePlayerFromTournament(CurrentPlayer, t);
            }
            catch (Exception)
            {
            }

            return Redirect("/Tournaments");
        }

        public IActionResult OnPostClose(int id)
        {
            try
            {
                LoadCurrentPlayer();

                if (!CurrentPlayer.IsAdmin)
                    return Unauthorized();

                var t = tournamentmanager.GetTournamentById(id);
                tournamentmanager.CloseTournament(t);
            }
            catch (Exception)
            {
            }

            return Redirect("/Tournaments");
        }

        public IActionResult OnPostDelete(int id)
        {
            LoadCurrentPlayer();

            if (!CurrentPlayer.IsAdmin)
                return Unauthorized();

            try
            {
                var t = tournamentmanager.GetTournamentById(id);
                tournamentmanager.RemoveTournament(t);
            }
            catch (Exception)
            {
            }

            return Redirect("/Tournaments");
        }
    }
}
