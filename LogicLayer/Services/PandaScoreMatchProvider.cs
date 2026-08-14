using System.Text.Json;
using LogicLayer.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace LogicLayer.Services
{
    public class PandaScoreMatchProvider : IExternalMatchProvider
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly TimeSpan CurrentMatchesCacheDuration = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan RecentMatchesCacheDuration = TimeSpan.FromMinutes(5);

        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;

        public PandaScoreMatchProvider(HttpClient httpClient, IMemoryCache cache)
        {
            this.httpClient = httpClient;
            this.cache = cache;
        }

        public async Task<List<ExternalMatchDto>> GetTodayMatchesAsync()
        {
            var cachedMatches = await cache.GetOrCreateAsync(
                "pandascore-current-matches",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = CurrentMatchesCacheDuration;
                    return await LoadCurrentMatchesAsync();
                });

            return cachedMatches ?? [];
        }

        public async Task<List<ExternalMatchDto>> GetRecentMatchesAsync(int limit = 20)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(limit, 50);

            var cachedMatches = await cache.GetOrCreateAsync(
                $"pandascore-recent-matches-{limit}",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = RecentMatchesCacheDuration;
                    var matches = await GetMatchesAsync(
                        $"/csgo/matches/past?sort=-begin_at&page[size]={limit}");
                    return matches.Select(Map).ToList();
                });

            return cachedMatches ?? [];
        }

        private async Task<List<ExternalMatchDto>> LoadCurrentMatchesAsync()
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

        public async Task<ExternalMatchDetailsDto?> GetMatchDetailsAsync(
            string matchId,
            bool preferPast = false)
        {
            if (!long.TryParse(matchId, out _))
                return null;

            var encodedId = Uri.EscapeDataString(matchId);
            var pastEndpoint = $"/csgo/matches/past?filter[id]={encodedId}&page[size]=1";
            string[] endpoints = preferPast
                ? [pastEndpoint]
                :
                [
                    $"/csgo/matches/running?filter[id]={encodedId}&page[size]=1",
                    $"/csgo/matches/upcoming?filter[id]={encodedId}&page[size]=1",
                    pastEndpoint
                ];

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
                Team1LogoUrl = match.Opponents.ElementAtOrDefault(0)?.Opponent?.ImageUrl ?? "",
                Team2LogoUrl = match.Opponents.ElementAtOrDefault(1)?.Opponent?.ImageUrl ?? "",
                EventName = match.League?.Name ?? "Unknown",
                Format = BuildFormat(match),
                Status = NormalizeStatus(match.Status),
                StartTimeUtc = match.BeginAt,
                Score = BuildScore(match),
                WinnerName = GetWinnerName(match),

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

                Score = BuildScore(m),
                WinnerName = GetWinnerName(m)
            };
        }

        private static string GetWinnerName(PandaMatch match)
        {
            return match.Opponents
                .Select(wrapper => wrapper.Opponent)
                .FirstOrDefault(opponent => opponent?.Id == match.WinnerId)
                ?.Name ?? "";
        }

        private static string NormalizeStatus(string? status)
        {
            return status switch
            {
                "running" => "Live",
                "not_started" => "Upcoming",
                "finished" => "Finished",
                _ => "Upcoming"
            };
        }

        private static string BuildFormat(PandaMatch match)
        {
            if (match.NumberOfGames is > 0)
                return $"Best of {match.NumberOfGames}";

            return match.MatchType switch
            {
                "best_of" => "Best of series",
                "first_to" => "First to series",
                _ => "Format unavailable"
            };
        }

        private static string BuildScore(PandaMatch m)
        {
            if (m.Status == "not_started")
                return "";

            if (m.Results == null || m.Results.Count == 0)
                return "";

            var t1 = m.Opponents.ElementAtOrDefault(0)?.Opponent?.Id;
            var t2 = m.Opponents.ElementAtOrDefault(1)?.Opponent?.Id;

            var result1 = m.Results.FirstOrDefault(x => x.TeamId == t1);
            var result2 = m.Results.FirstOrDefault(x => x.TeamId == t2);

            if (result1 == null || result2 == null)
                return "";

            var s1 = result1.Score;
            var s2 = result2.Score;

            if (m.Status == "finished" && s1 == 0 && s2 == 0)
                return "";

            return $"{s1} - {s2}";
        }

        private async Task<List<PandaMatch>> GetMatchesAsync(string url)
        {
            try
            {
                using var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return new List<PandaMatch>();

                await using var stream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<List<PandaMatch>>(
                    stream,
                    SerializerOptions)
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
            catch (TaskCanceledException)
            {
                return new List<PandaMatch>();
            }
        }
    }
}
