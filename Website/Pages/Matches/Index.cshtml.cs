using LogicLayer.Dtos;
using LogicLayer.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Matches
{
    public class IndexModel : PageModel
    {
        private readonly IExternalMatchProvider _provider;

        public IndexModel(IExternalMatchProvider provider)
        {
            _provider = provider;
        }

        public List<ExternalMatchDto> Matches { get; set; } = new();

        public async Task OnGetAsync()
        {
            Matches = await _provider.GetTodayMatchesAsync();
        }
    }
}
