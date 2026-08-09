using System.Text.Json;
using LogicLayer.Dtos;

namespace LogicLayer.Services
{
    public class PandaScoreMatchProvider : IExternalMatchProvider
    {
        private readonly HttpClient _http;

        private const string API_KEY = "YOUR_API_KEY"; // prebaci kasnije u config

        public PandaScoreMatchProvider(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://api.pandascore.co");

            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
        }

        public async Task<List<ExternalMatchDto>> GetTodayMatchesAsync()
        {
            var url = "/csgo/matches?sort=begin_at&page[size]=20";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<ExternalMatchDto>();

            var stream = await response.Content.ReadAsStreamAsync();

            var matches = await JsonSerializer.DeserializeAsync<List<PandaMatch>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return matches?
                .Select(Map)
                .Where(x => x != null)
                .ToList()!;
        }

        public async Task<ExternalMatchDetailsDto?> GetMatchDetailsAsync(string matchId)
        {
            var url = $"/csgo/matches/{matchId}";

            var response = await _http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var stream = await response.Content.ReadAsStreamAsync();

            var match = await JsonSerializer.DeserializeAsync<PandaMatch>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (match == null) return null;

            return new ExternalMatchDetailsDto
            {
                Id = match.Id.ToString(),
                Team1Name = match.Opponents.ElementAtOrDefault(0)?.Opponent?.Name ?? "TBD",
                Team2Name = match.Opponents.ElementAtOrDefault(1)?.Opponent?.Name ?? "TBD",
                EventName = match.League?.Name ?? "Unknown",
                Status = NormalizeStatus(match.Status),
                StartTimeUtc = match.BeginAt,
                Score = BuildScore(match),

                Maps = new List<ExternalMapDto>(),

                Streams = match.StreamsList?
                    .Select(s => new ExternalStreamDto
                    {
                        Platform = "Stream",
                        StreamerName = "Official",
                        Url = s.RawUrl ?? "",
                        Viewers = 0
                    }).ToList() ?? new()
            };
        }

        // ---------------- MAP ----------------

        private ExternalMatchDto Map(PandaMatch m)
        {
            return new ExternalMatchDto
            {
                Id = m.Id.ToString(),
                Team1Name = m.Opponents.ElementAtOrDefault(0)?.Opponent?.Name ?? "TBD",
                Team2Name = m.Opponents.ElementAtOrDefault(1)?.Opponent?.Name ?? "TBD",

                Team1LogoUrl = "",
                Team2LogoUrl = "",

                EventName = m.League?.Name ?? "Unknown",

                StartTimeUtc = m.BeginAt,

                Status = NormalizeStatus(m.Status),

                Score = BuildScore(m)
            };
        }

        private string NormalizeStatus(string? status)
        {
            return status switch
            {
                "running" => "Live",
                "not_started" => "Upcoming",
                "finished" => "Finished",
                _ => "Upcoming"
            };
        }

        private string BuildScore(PandaMatch m)
        {
            if (m.Results == null || m.Results.Count == 0)
                return "";

            var t1 = m.Opponents.ElementAtOrDefault(0)?.Opponent?.Id;
            var t2 = m.Opponents.ElementAtOrDefault(1)?.Opponent?.Id;

            var s1 = m.Results.FirstOrDefault(x => x.TeamId == t1)?.Score ?? 0;
            var s2 = m.Results.FirstOrDefault(x => x.TeamId == t2)?.Score ?? 0;

            return $"{s1} - {s2}";
        }
    }
}