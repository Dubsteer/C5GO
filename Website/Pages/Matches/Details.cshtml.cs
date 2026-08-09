using LogicLayer.Dtos;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Matches
{
    public class DetailsModel : PageModel
    {
        private readonly IExternalMatchProvider _provider;

        public DetailsModel(IExternalMatchProvider provider)
        {
            _provider = provider;
        }

        public ExternalMatchDetailsDto? Match { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage("/Matches/Index");

            Match = await _provider.GetMatchDetailsAsync(id);

            if (Match == null)
                return RedirectToPage("/Matches/Index");

            return Page();
        }
    }
}