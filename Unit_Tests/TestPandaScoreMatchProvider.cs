using System.Net;
using System.Text;
using LogicLayer.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Unit_Tests
{
    [TestClass]
    public class TestPandaScoreMatchProvider
    {
        [TestMethod]
        public async Task GetTodayMatchesAsync_ReturnsRunningAndUpcomingMatches()
        {
            var requestedPaths = new List<string>();
            var handler = new StubHttpMessageHandler(request =>
            {
                requestedPaths.Add(request.RequestUri!.AbsolutePath);

                var json = request.RequestUri.AbsolutePath.EndsWith("/running")
                    ? RunningMatchJson
                    : UpcomingMatchJson;

                return JsonResponse(json);
            });

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.pandascore.co")
            };
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var provider = new PandaScoreMatchProvider(client, cache);

            var matches = await provider.GetTodayMatchesAsync();

            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual("Live", matches[0].Status);
            Assert.AreEqual("1 - 0", matches[0].Score);
            Assert.AreEqual("https://images.test/navi.png", matches[0].Team1LogoUrl);
            Assert.AreEqual("", matches[1].Score);
            CollectionAssert.Contains(requestedPaths, "/csgo/matches/running");
            CollectionAssert.Contains(requestedPaths, "/csgo/matches/upcoming");
        }

        [TestMethod]
        public async Task GetMatchDetailsAsync_UsesFreeListEndpoints()
        {
            var requestedUris = new List<string>();
            var handler = new StubHttpMessageHandler(request =>
            {
                requestedUris.Add(request.RequestUri!.PathAndQuery);

                var json = request.RequestUri.AbsolutePath.EndsWith("/upcoming")
                    ? UpcomingMatchJson
                    : "[]";

                return JsonResponse(json);
            });

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.pandascore.co")
            };
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var provider = new PandaScoreMatchProvider(client, cache);

            var match = await provider.GetMatchDetailsAsync("42");

            Assert.IsNotNull(match);
            Assert.AreEqual("G2", match.Team1Name);
            Assert.AreEqual("Upcoming", match.Status);
            Assert.AreEqual("Best of 3", match.Format);
            Assert.AreEqual(2, requestedUris.Count);
            Assert.IsFalse(requestedUris.Any(uri => uri.StartsWith("/csgo/matches/42")));
        }

        [TestMethod]
        public async Task GetRecentMatchesAsync_ReturnsFinishedMatchesAndUsesCache()
        {
            var requestCount = 0;
            string? requestedUri = null;
            var handler = new StubHttpMessageHandler(request =>
            {
                requestCount++;
                requestedUri = request.RequestUri!.PathAndQuery;
                return JsonResponse(PastMatchJson);
            });

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.pandascore.co")
            };
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var provider = new PandaScoreMatchProvider(client, cache);

            var firstResult = await provider.GetRecentMatchesAsync(10);
            var secondResult = await provider.GetRecentMatchesAsync(10);

            Assert.AreEqual(1, requestCount);
            Assert.AreEqual(1, firstResult.Count);
            Assert.AreEqual("Finished", firstResult[0].Status);
            Assert.AreEqual("2 - 1", firstResult[0].Score);
            Assert.AreEqual("Spirit", firstResult[0].WinnerName);
            Assert.AreEqual(firstResult[0].Id, secondResult[0].Id);
            StringAssert.Contains(requestedUri, "/csgo/matches/past");
            StringAssert.Contains(requestedUri, "sort=-begin_at");
            StringAssert.Contains(requestedUri, "page[size]=10");
        }

        [TestMethod]
        public async Task GetRecentMatchesAsync_DoesNotInventZeroScoreWhenResultsAreMissing()
        {
            var handler = new StubHttpMessageHandler(_ => JsonResponse(PastMatchWithoutScoresJson));

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.pandascore.co")
            };
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var provider = new PandaScoreMatchProvider(client, cache);

            var matches = await provider.GetRecentMatchesAsync();

            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual("", matches[0].Score);
            Assert.AreEqual("Spirit", matches[0].WinnerName);
        }

        [TestMethod]
        public async Task GetMatchDetailsAsync_PreferPastUsesSinglePastRequest()
        {
            var requestedUris = new List<string>();
            var handler = new StubHttpMessageHandler(request =>
            {
                requestedUris.Add(request.RequestUri!.PathAndQuery);
                return JsonResponse(PastMatchJson);
            });

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.pandascore.co")
            };
            using var cache = new MemoryCache(new MemoryCacheOptions());
            var provider = new PandaScoreMatchProvider(client, cache);

            var match = await provider.GetMatchDetailsAsync("43", preferPast: true);

            Assert.IsNotNull(match);
            Assert.AreEqual("Finished", match.Status);
            Assert.AreEqual("Spirit", match.WinnerName);
            Assert.AreEqual(1, requestedUris.Count);
            StringAssert.Contains(requestedUris[0], "/csgo/matches/past");
        }

        private static HttpResponseMessage JsonResponse(string json)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private sealed class StubHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

            public StubHttpMessageHandler(
                Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
            {
                _responseFactory = responseFactory;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(_responseFactory(request));
            }
        }

        private const string RunningMatchJson = """
            [
              {
                "id": 41,
                "status": "running",
                "begin_at": "2026-08-09T10:00:00Z",
                "opponents": [
                  { "opponent": { "id": 10, "name": "NAVI", "image_url": "https://images.test/navi.png" } },
                  { "opponent": { "id": 20, "name": "Vitality", "image_url": "https://images.test/vitality.png" } }
                ],
                "league": { "name": "IEM" },
                "results": [
                  { "team_id": 10, "score": 1 },
                  { "team_id": 20, "score": 0 }
                ],
                "streams_list": []
              }
            ]
            """;

        private const string UpcomingMatchJson = """
            [
              {
                "id": 42,
                "status": "not_started",
                "begin_at": "2026-08-09T14:00:00Z",
                "match_type": "best_of",
                "number_of_games": 3,
                "opponents": [
                  { "opponent": { "id": 30, "name": "G2" } },
                  { "opponent": { "id": 40, "name": "MOUZ" } }
                ],
                "league": { "name": "BLAST" },
                "results": [
                  { "team_id": 30, "score": 0 },
                  { "team_id": 40, "score": 0 }
                ],
                "streams_list": []
              }
            ]
            """;

        private const string PastMatchJson = """
            [
              {
                "id": 43,
                "status": "finished",
                "begin_at": "2026-08-08T18:00:00Z",
                "winner_id": 50,
                "opponents": [
                  { "opponent": { "id": 50, "name": "Spirit" } },
                  { "opponent": { "id": 60, "name": "Vitality" } }
                ],
                "league": { "name": "IEM Cologne" },
                "results": [
                  { "team_id": 50, "score": 2 },
                  { "team_id": 60, "score": 1 }
                ],
                "streams_list": []
              }
            ]
            """;

        private const string PastMatchWithoutScoresJson = """
            [
              {
                "id": 44,
                "status": "finished",
                "begin_at": "2026-08-07T18:00:00Z",
                "winner_id": 50,
                "opponents": [
                  { "opponent": { "id": 50, "name": "Spirit" } },
                  { "opponent": { "id": 60, "name": "Vitality" } }
                ],
                "league": { "name": "IEM Cologne" },
                "results": [],
                "streams_list": []
              }
            ]
            """;
    }
}
