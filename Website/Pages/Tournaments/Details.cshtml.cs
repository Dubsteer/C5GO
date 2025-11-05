using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LogicLayer.Managers;
using LogicLayer.Models;
using System;

namespace Website.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly TournamentManager tournamentManager;

        public Tournament Tournament { get; set; }

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
