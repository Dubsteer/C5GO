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
        public Team MyTeam { get; set; }

        public string Filter { get; set; } = "all";
        public bool IsPlayer => CurrentPlayer != null;

        private readonly UserManager userManager;
        private readonly TournamentManager tournamentManager;
        private readonly PlayerManager playerManager;
        private readonly TeamManager teamManager;

        public TournamentsModel(
            TournamentManager tournamentManager,
            PlayerManager playerManager,
            UserManager userManager,
            TeamManager teamManager)
        {
            this.tournamentManager = tournamentManager;
            this.playerManager = playerManager;
            this.userManager = userManager;
            this.teamManager = teamManager;
        }

        private void LoadCurrent()
        {
            try
            {
                var uid = int.Parse(User.FindFirst("id").Value);
                var user = userManager.GetUserById(uid);

                // Ako user nema SteamID ? NE MOŽE BITI PLAYER
                if (string.IsNullOrWhiteSpace(user.SteamId) || user.SteamId == "0")
                {
                    CurrentPlayer = null;
                    MyTeam = null;
                    return;
                }

                CurrentPlayer = playerManager.GetPlayer(user);
                MyTeam = teamManager.GetTeamOfUser(uid);
            }
            catch
            {
                CurrentPlayer = null;
                MyTeam = null;
            }
        }

        // GET
        public IActionResult OnGet(string filter = "all")
        {
            LoadCurrent();
            Filter = filter;

            Tournaments = tournamentManager.GetAllTournaments();

            foreach (var t in Tournaments)
            {
                t.Players = tournamentManager.GetAllPlayersInTournament(t);
                t.Matches = tournamentManager.GetAllMatchesInTournament(t);

                tournamentManager.UpdateTournamentStatus(t);
            }

            Tournaments = Filter switch
            {
                "solo" => Tournaments.Where(t => !t.IsTeamTournament).ToList(),
                "team" => Tournaments.Where(t => t.IsTeamTournament).ToList(),
                _ => Tournaments
            };

            return Page();
        }

        // SOLO APPLY
        public IActionResult OnPostApplySolo(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
                return Redirect("/ViewProfile");

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                t.Players = tournamentManager.GetAllPlayersInTournament(t);

                if (!t.Players.Any(p => p.Id == CurrentPlayer.Id))
                    tournamentManager.AddTournamentApp(CurrentPlayer, t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SoloApplyError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }

        // TEAM APPLY
        public IActionResult OnPostApplyTeam(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
                return Redirect("/ViewProfile");

            if (MyTeam == null)
                return Redirect("/Teams/Teams");

            if (MyTeam.Captain.Id != CurrentPlayer.Id)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);

                if (MyTeam.Members.Count != t.TeamSizeRequired)
                    throw new Exception("Your team does not have the required number of members.");

                foreach (var member in MyTeam.Members)
                {
                    var player = playerManager.GetPlayer(member);

                    // Member must have valid SteamID
                    if (string.IsNullOrWhiteSpace(member.SteamId) || member.SteamId == "0")
                        throw new Exception($"Player {member.Username} does not have a SteamID.");

                    tournamentManager.AddTournamentApp(player, t);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TeamApplyError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }

        // LEAVE
        public IActionResult OnPostLeave(int id)
        {
            LoadCurrent();

            if (!IsPlayer)
                return Redirect("/ViewProfile");

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

        // ADMIN CLOSE
        public IActionResult OnPostClose(int id)
        {
            LoadCurrent();

            if (!IsPlayer || !CurrentPlayer.IsAdmin)
                return Unauthorized();

            try
            {
                var t = tournamentManager.GetTournamentById(id);
                tournamentManager.SetStatus(t, LogicLayer.Enums.Status.Closed);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CloseError: " + ex.Message);
            }

            return Redirect("/Tournaments");
        }

        // ADMIN DELETE
        public IActionResult OnPostDelete(int id)
        {
            LoadCurrent();

            if (!IsPlayer || !CurrentPlayer.IsAdmin)
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
