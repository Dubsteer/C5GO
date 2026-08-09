using System.Net;
using System.Text;
using LogicLayer.Services;

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
            var provider = new PandaScoreMatchProvider(client);

            var matches = await provider.GetTodayMatchesAsync();

            Assert.AreEqual(2, matches.Count);
            Assert.AreEqual("Live", matches[0].Status);
            Assert.AreEqual("1 - 0", matches[0].Score);
            Assert.AreEqual("https://images.test/navi.png", matches[0].Team1LogoUrl);
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
            var provider = new PandaScoreMatchProvider(client);

            var match = await provider.GetMatchDetailsAsync("42");

            Assert.IsNotNull(match);
            Assert.AreEqual("G2", match.Team1Name);
            Assert.AreEqual("Upcoming", match.Status);
            Assert.AreEqual(2, requestedUris.Count);
            Assert.IsFalse(requestedUris.Any(uri => uri.StartsWith("/csgo/matches/42")));
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
                "opponents": [
                  { "opponent": { "id": 30, "name": "G2" } },
                  { "opponent": { "id": 40, "name": "MOUZ" } }
                ],
                "league": { "name": "BLAST" },
                "results": [],
                "streams_list": []
              }
            ]
            """;
    }
}
