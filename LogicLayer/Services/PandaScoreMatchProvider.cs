using System.Text.Json;
using LogicLayer.Dtos;

namespace LogicLayer.Services
{
    public class PandaScoreMatchProvider : IExternalMatchProvider
    {
        private readonly HttpClient _http;

        public PandaScoreMatchProvider(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ExternalMatchDto>> GetTodayMatchesAsync()
        {
            var runningTask = GetMatchesAsync(
                "/csgo/matches/running?sort=begin_at&page[size]=50");
            var upcomingTask = GetMatchesAsync(
                "/csgo/matches/upcoming?sort=begin_at&page[size]=20");

            await Task.WhenAll(runningTask, upcomingTask);

            return runningTask.Result
                .Concat(upcomingTask.Result)
                .DistinctBy(match => match.Id)
                .Select(Map)
                .ToList();
        }

        public async Task<ExternalMatchDetailsDto?> GetMatchDetailsAsync(string matchId)
        {
            if (!long.TryParse(matchId, out _))
                return null;

            var encodedId = Uri.EscapeDataString(matchId);
            var endpoints = new[]
            {
                $"/csgo/matches/running?filter[id]={encodedId}&page[size]=1",
                $"/csgo/matches/upcoming?filter[id]={encodedId}&page[size]=1",
                $"/csgo/matches/past?filter[id]={encodedId}&page[size]=1"
            };

            PandaMatch? match = null;

            foreach (var endpoint in endpoints)
            {
                match = (await GetMatchesAsync(endpoint)).FirstOrDefault();

                if (match != null)
                    break;
            }

            if (match == null)
                return null;

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
                    .Where(s => !string.IsNullOrWhiteSpace(s.RawUrl))
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

                Team1LogoUrl = m.Opponents.ElementAtOrDefault(0)?.Opponent?.ImageUrl ?? "",
                Team2LogoUrl = m.Opponents.ElementAtOrDefault(1)?.Opponent?.ImageUrl ?? "",

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
            if (m.Status == "not_started")
                return "";

            if (m.Results == null || m.Results.Count == 0)
                return "";

            var t1 = m.Opponents.ElementAtOrDefault(0)?.Opponent?.Id;
            var t2 = m.Opponents.ElementAtOrDefault(1)?.Opponent?.Id;

            var s1 = m.Results.FirstOrDefault(x => x.TeamId == t1)?.Score ?? 0;
            var s2 = m.Results.FirstOrDefault(x => x.TeamId == t2)?.Score ?? 0;

            return $"{s1} - {s2}";
        }

        private async Task<List<PandaMatch>> GetMatchesAsync(string url)
        {
            try
            {
                using var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<PandaMatch>();

                await using var stream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<List<PandaMatch>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<PandaMatch>();
            }
            catch (HttpRequestException)
            {
                return new List<PandaMatch>();
            }
            catch (JsonException)
            {
                return new List<PandaMatch>();
            }
        }
    }
}
