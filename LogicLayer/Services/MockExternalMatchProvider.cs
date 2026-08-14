using LogicLayer.Dtos;

namespace LogicLayer.Services
{
    public class MockExternalMatchProvider : IExternalMatchProvider
    {
        public Task<List<ExternalMatchDto>> GetTodayMatchesAsync()
        {
            var matches = new List<ExternalMatchDto>
            {
                new ExternalMatchDto
                {
                    Id = "1",
                    Team1Name = "Eternal Fire",
                    Team2Name = "ENCE",
                    EventName = "BLAST Premier",
                    Status = "Live",
                    Score = "13 - 10",
                    StartTimeUtc = DateTime.UtcNow.AddMinutes(-20)
                },
                new ExternalMatchDto
                {
                    Id = "2",
                    Team1Name = "FOKUS",
                    Team2Name = "Wildcard",
                    EventName = "CCT Europe",
                    Status = "Live",
                    Score = "7 - 3",
                    StartTimeUtc = DateTime.UtcNow.AddMinutes(-10)
                },
                new ExternalMatchDto
                {
                    Id = "3",
                    Team1Name = "MOUZ NXT",
                    Team2Name = "BESTIA",
                    EventName = "ESL Challenger",
                    Status = "Upcoming",
                    Score = "-",
                    StartTimeUtc = DateTime.UtcNow.AddHours(2)
                }
            };

            return Task.FromResult(matches);
        }

        public Task<ExternalMatchDetailsDto?> GetMatchDetailsAsync(
            string matchId,
            bool preferPast = false)
        {
            var matches = new Dictionary<string, ExternalMatchDetailsDto>
            {
                ["1"] = new ExternalMatchDetailsDto
                {
                    Id = "1",
                    Team1Name = "Eternal Fire",
                    Team2Name = "ENCE",
                    EventName = "BLAST Premier",
                    Format = "Best of 3",
                    Status = "Live",
                    Score = "13 - 10",
                    StartTimeUtc = DateTime.UtcNow.AddMinutes(-20),
                    Maps = new List<ExternalMapDto>
                    {
                        new ExternalMapDto { MapName = "Inferno", Score = "13 - 10" }
                    },
                    Streams = new List<ExternalStreamDto>
                    {
                        new ExternalStreamDto
                        {
                            Platform = "Twitch",
                            StreamerName = "BLAST",
                            Url = "https://twitch.tv/blast",
                            Viewers = 12000
                        }
                    }
                },

                ["2"] = new ExternalMatchDetailsDto
                {
                    Id = "2",
                    Team1Name = "FOKUS",
                    Team2Name = "Wildcard",
                    EventName = "CCT Europe",
                    Format = "Best of 3",
                    Status = "Live",
                    Score = "7 - 3",
                    StartTimeUtc = DateTime.UtcNow.AddMinutes(-10),
                    Maps = new List<ExternalMapDto>
                    {
                        new ExternalMapDto { MapName = "Mirage", Score = "7 - 3" }
                    },
                    Streams = new List<ExternalStreamDto>
                    {
                        new ExternalStreamDto
                        {
                            Platform = "Kick",
                            StreamerName = "Hyper",
                            Url = "https://kick.com/hyper",
                            Viewers = 596
                        }
                    }
                },

                ["3"] = new ExternalMatchDetailsDto
                {
                    Id = "3",
                    Team1Name = "MOUZ NXT",
                    Team2Name = "BESTIA",
                    EventName = "ESL Challenger",
                    Format = "Best of 3",
                    Status = "Upcoming",
                    Score = "-",
                    StartTimeUtc = DateTime.UtcNow.AddHours(2),
                    Maps = new List<ExternalMapDto>(),
                    Streams = new List<ExternalStreamDto>()
                },

                ["4"] = new ExternalMatchDetailsDto
                {
                    Id = "4",
                    Team1Name = "Vitality",
                    Team2Name = "Spirit",
                    EventName = "IEM Cologne",
                    Format = "Best of 3",
                    Status = "Finished",
                    Score = "2 - 1",
                    WinnerName = "Vitality",
                    StartTimeUtc = DateTime.UtcNow.AddDays(-1),
                    Maps = [],
                    Streams = []
                },

                ["5"] = new ExternalMatchDetailsDto
                {
                    Id = "5",
                    Team1Name = "NAVI",
                    Team2Name = "MOUZ",
                    EventName = "BLAST Premier",
                    Format = "Best of 3",
                    Status = "Finished",
                    Score = "",
                    WinnerName = "MOUZ",
                    StartTimeUtc = DateTime.UtcNow.AddDays(-2),
                    Maps = [],
                    Streams = []
                }
            };

            matches.TryGetValue(matchId, out var result);

            return Task.FromResult(result);
        }

        public Task<List<ExternalMatchDto>> GetRecentMatchesAsync(int limit = 20)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(limit, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(limit, 50);

            var matches = new List<ExternalMatchDto>
            {
                new()
                {
                    Id = "4",
                    Team1Name = "Vitality",
                    Team2Name = "Spirit",
                    EventName = "IEM Cologne",
                    Status = "Finished",
                    Score = "2 - 1",
                    WinnerName = "Vitality",
                    StartTimeUtc = DateTime.UtcNow.AddDays(-1)
                },
                new()
                {
                    Id = "5",
                    Team1Name = "NAVI",
                    Team2Name = "MOUZ",
                    EventName = "BLAST Premier",
                    Status = "Finished",
                    Score = "",
                    WinnerName = "MOUZ",
                    StartTimeUtc = DateTime.UtcNow.AddDays(-2)
                }
            };

            return Task.FromResult(matches.Take(limit).ToList());
        }
    }
}
