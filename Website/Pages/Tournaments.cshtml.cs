using LogicLayer.Models;
using LogicLayer.Managers;
using LogicLayer.IRepos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using LogicLayer.FormModels;
using Microsoft.AspNetCore.Authorization;
using LogicLayer.Enums;
using Microsoft.AspNetCore.Identity;

namespace Website.Pages
{
    [Authorize]
    public class TournamentsModel : PageModel
    {
        [BindProperty]
        public TournamentFormModel FormModel { get; set; }
        public List<Tournament> Tournaments { get; set; }

        private readonly UserManager userManager;
        private readonly TournamentManager tournamentmanager;
        private readonly PlayerManager playerManager;

        public Player steamaccountId { get; set; }
        public TournamentsModel(TournamentManager tournamentmanager, PlayerManager playerManager, UserManager userManager)
        {
            this.tournamentmanager = tournamentmanager;
            this.playerManager = playerManager;
            this.userManager = userManager;
        }

        public bool checkPlayer()
        {
            var currentUser = userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value));
            var playerCheck = new Player((int)currentUser.Id, currentUser.Firstname, currentUser.Lastname, currentUser.Age, currentUser.Username, currentUser.Gmail, currentUser.Password, "0", currentUser.IsAdmin);

            foreach (var p in playerManager.GetAllPlayers())
            {
                if (p.Id == currentUser.Id)
                    return true;
            }
            return false;
        }

        public bool checkIfPlayerInTournament(int id)
        {
            var currentPlayer = playerManager.GetPlayer(userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value)));
            Tournament currentTournament = tournamentmanager.GetTournamentById(id);
            foreach (Player p in currentTournament.Players)
            {
                if (p.Id == currentPlayer.Id)
                    return true;
            }
            return false;
        }

        public IActionResult OnGet()
        {
            try
            {
                Tournaments = tournamentmanager.GetAllTournaments();

                foreach (var tournament in Tournaments)
                {
                    tournament.Matches = tournamentmanager.GetAllMatchesInTournament(tournament);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // internal server error
                return StatusCode(500);
            }

            return Page();
        }

        public IActionResult OnPostApply(int id)
        {
            try
            {
                var currentPlayer = playerManager.GetPlayer(userManager.GetUserById(Convert.ToInt32(User.FindFirst("id").Value)));
                Tournament currentTournament = tournamentmanager.GetTournamentById(id);
                if (currentTournament.Closed)
                {
                    throw new Exception("This tournament is closed and not accepting new players");
                }
                if (!checkIfPlayerInTournament(id))
                {
                    tournamentmanager.AddTournamentApp(currentPlayer, currentTournament);
                }
                else
                {
                    throw new Exception("You are already in this tournament");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // internal server error
                return StatusCode(500);
            }

            return Redirect("/Tournaments");
        }
    }
}
