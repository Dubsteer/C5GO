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
        public bool FromHistory { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id, bool history = false)
        {
            FromHistory = history;

            if (string.IsNullOrEmpty(id))
                return RedirectToPage(history ? "/Matches/History" : "/Matches/Index");

            Match = await _provider.GetMatchDetailsAsync(id, history);

            if (Match == null)
                return RedirectToPage(history ? "/Matches/History" : "/Matches/Index");

            return Page();
        }
    }
}
