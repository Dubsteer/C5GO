using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Website.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly TournamentManager tournamentManager;

        public Tournament Tournament { get; set; }

        // Bracket rounds
        public List<Match> Round1 { get; set; } = new();
        public List<Match> Round2 { get; set; } = new();
        public List<Match> Round3 { get; set; } = new();

        public DetailsModel(TournamentManager tournamentManager)
        {
            this.tournamentManager = tournamentManager;
        }

        public IActionResult OnGet(int id)
        {
            try
            {
                Tournament = tournamentManager.GetTournamentById(id);

                if (Tournament == null)
                    return NotFound();

                Tournament.Players = tournamentManager.GetAllPlayersInTournament(Tournament);
                Tournament.Matches = tournamentManager.GetAllMatchesInTournament(Tournament);

                var matches = Tournament.Matches.OrderBy(m => m.Id).ToList();

                int count = matches.Count;

                if (count >= 4)
                    Round1 = matches.Take(4).ToList();

                if (count >= 6)
                    Round2 = matches.Skip(4).Take(2).ToList();

                if (count >= 7)
                    Round3 = matches.Skip(6).Take(1).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }

            return Page();
        }
    }
}
