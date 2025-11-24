using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Website.Pages
{
    [Authorize]
    public class TournamentsModel : PageModel
    {
        public List<Tournament> Tournaments { get; set; } = new();
        public Player CurrentPlayer { get; set; }
        public bool IsPlayer => CurrentPlayer != null;

        private readonly UserManager userManager;
        private readonly TournamentManager tournamentManager;
        private readonly PlayerManager playerManager;

        public TournamentsModel(TournamentManager tournamentManager, PlayerManager playerManager, UserManager userManager)
        {
            this.tournamentManager = tournamentManager;
            this.playerManager = playerManager;
            this.userManager = userManager;
        }

        // ----------------------------------------------------
        // LOAD CURRENT PLAYER
        // ----------------------------------------------------
        private void LoadCurrentPlayer()
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("id").Value);
                var user = userManager.GetUserById(userId);

                // If user has no SteamID ? player = null
                CurrentPlayer = playerManager.GetPlayer(user);
            }
            catch
            {
                CurrentPlayer = null;
            }
        }

        // ----------------------------------------------------
        // GET ALL TOURNAMENTS
        // ----------------------------------------------------
        public IActionResult OnGet()
        {
            LoadCurrentPlayer();

            Tournaments = tournamentManager.GetAllTournaments();

            foreach (var t in Tournaments)
            {
                t.Players = tournamentManager.GetAllPlayersInTournament(t);
                t.Matches = tournamentManager.GetAllMatchesInTournament(t);
            }

            return Page();
        }

        // ----------------------------------------------------
        // JOIN TOURNAMENT
        // ----------------------------------------------------
        public IActionResult OnPostApply(int id)
        {
            LoadCurrentPlayer();

            if (CurrentPlayer == null)
                return Redirect("/Profile");

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                t.Players = tournamentManager.GetAllPlayersInTournament(t);

                if (!t.Players.Any(p => p.Id == CurrentPlayer.Id))
                    tournamentManager.AddTournamentApp(CurrentPlayer, t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("JoinError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }

        // ----------------------------------------------------
        // LEAVE TOURNAMENT
        // ----------------------------------------------------
        public IActionResult OnPostLeave(int id)
        {
            LoadCurrentPlayer();

            if (CurrentPlayer == null)
                return Redirect("/Profile");

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.RemovePlayerFromTournament(CurrentPlayer, t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LeaveError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }

        // ----------------------------------------------------
        // ADMIN: CLOSE TOURNAMENT
        // ----------------------------------------------------
        public IActionResult OnPostClose(int id)
        {
            LoadCurrentPlayer();

            if (CurrentPlayer == null || !CurrentPlayer.IsAdmin)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                t.Status = LogicLayer.Enums.Status.Closed;
                tournamentManager.UpdateTournament(t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CloseError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }

        // ----------------------------------------------------
        // ADMIN: DELETE TOURNAMENT
        // ----------------------------------------------------
        public IActionResult OnPostDelete(int id)
        {
            LoadCurrentPlayer();

            if (CurrentPlayer == null || !CurrentPlayer.IsAdmin)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.RemoveTournament(t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DeleteError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }
    }
}
