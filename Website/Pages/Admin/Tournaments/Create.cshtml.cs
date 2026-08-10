using System.ComponentModel.DataAnnotations;
using LogicLayer.Enums;
using LogicLayer.Managers;
using LogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin.Tournaments
{
    public class CreateModel : PageModel
    {
        private readonly TournamentManager tournamentManager;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public CreateModel(TournamentManager tournamentManager)
        {
            this.tournamentManager = tournamentManager;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                tournamentManager.AddTournament(new Tournament
                {
                    Name = Input.Name,
                    Description = Input.Description,
                    Status = Status.Open,
                    IsTeamTournament = Input.IsTeamTournament,
                    TeamSizeRequired = Input.IsTeamTournament ? 5 : 1
                });

                TempData["SuccessMessage"] = "Tournament created.";
                return RedirectToPage("/Admin/Tournaments/Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "The tournament could not be created.");
                return Page();
            }
        }

        public class InputModel
        {
            [Required]
            [StringLength(50)]
            public string Name { get; set; } = string.Empty;

            [Required]
            [StringLength(300)]
            public string Description { get; set; } = string.Empty;

            [Display(Name = "Team tournament")]
            public bool IsTeamTournament { get; set; }
        }
    }
}
