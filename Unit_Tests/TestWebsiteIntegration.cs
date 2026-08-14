using System.Net;
using System.Xml.Linq;
using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Models.Community;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public sealed class TestWebsiteIntegration
    {
        private WebApplicationFactory<Program> factory = null!;

        [TestInitialize]
        public void TestInit()
        {
            factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                    });
                    builder.ConfigureAppConfiguration((_, configuration) =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:DefaultConnection"] =
                                "Server=127.0.0.1;Port=65535;Database=c5go_test;User ID=test;Password=test;",
                            ["PandaScore:ApiKey"] = string.Empty,
                            ["EmailSettings:SmtpServer"] = "localhost",
                            ["EmailSettings:Port"] = "2525",
                            ["EmailSettings:SenderEmail"] = "test@c5go.local",
                            ["EmailSettings:Username"] = "test",
                            ["EmailSettings:Password"] = "test",
                            ["AppSettings:AuthUrl"] = "https://localhost",
                            ["Turnstile:SiteKey"] = "1x00000000000000000000AA",
                            ["Turnstile:SecretKey"] = "1x0000000000000000000000000000000AA"
                        });
                    });
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<IMatchRepo>();
                        services.AddSingleton<IMatchRepo>(
                            new MockMatchRepo(CreatePlayerHistoryMatches()));
                        services.RemoveAll<ITeamMatchRepo>();
                        services.AddSingleton<ITeamMatchRepo>(
                            new MockTeamMatchRepo(CreateTeamHistoryMatches()));
                        services.PostConfigure<KeyManagementOptions>(options =>
                        {
                            options.XmlRepository = new InMemoryXmlRepository();
                            options.XmlEncryptor = null;
                        });
                    });
                });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            factory.Dispose();
        }

        [TestMethod]
        public async Task HealthEndpointReturnsSecurityHeaders()
        {
            using var client = CreateClient();
            using var response = await client.GetAsync("/health");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            StringAssert.Contains(await response.Content.ReadAsStringAsync(), "healthy");
            AssertHeader(response, "X-Content-Type-Options", "nosniff");
            AssertHeader(response, "X-Frame-Options", "DENY");
            AssertHeader(response, "Referrer-Policy", "strict-origin-when-cross-origin");
            AssertHeader(response, "Permissions-Policy", "camera=(), microphone=(), geolocation=()");
            AssertHeaderContains(response, "Content-Security-Policy", "frame-ancestors 'none'");
            AssertHeaderContains(
                response,
                "Content-Security-Policy",
                "frame-src https://www.youtube-nocookie.com https://challenges.cloudflare.com");
            AssertHeaderContains(
                response,
                "Content-Security-Policy",
                "script-src 'self' https://challenges.cloudflare.com");
            AssertHeaderContains(response, "Strict-Transport-Security", "max-age=");
            Assert.IsFalse(response.Headers.Contains("Server"));
        }

        [TestMethod]
        [DataRow("/Register")]
        [DataRow("/ForgotPassword")]
        public async Task ProtectedPublicFormsRenderTurnstile(string path)
        {
            using var client = CreateClient();
            using var response = await client.GetAsync(path);
            var html = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
            StringAssert.Contains(html, "cf-turnstile");
            StringAssert.Contains(html, "1x00000000000000000000AA");
            StringAssert.Contains(
                html,
                "https://challenges.cloudflare.com/turnstile/v0/api.js");
        }

        [TestMethod]
        [DataRow("/Login")]
        [DataRow("/Register")]
        [DataRow("/ForgotPassword")]
        [DataRow("/RegisterSuccess")]
        [DataRow("/ForgotPasswordConfirmation")]
        [DataRow("/ResetPasswordConfirmation")]
        [DataRow("/Matches/History")]
        [DataRow("/Errors/401")]
        [DataRow("/Errors/403")]
        [DataRow("/Errors/404")]
        [DataRow("/Errors/500")]
        public async Task PublicPagesReturnSuccess(string path)
        {
            using var client = CreateClient();
            using var response = await client.GetAsync(path);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
        }

        [TestMethod]
        [DataRow("/lib/bootstrap/dist/css/bootstrap.min.css")]
        [DataRow("/lib/bootstrap/dist/js/bootstrap.bundle.min.js")]
        [DataRow("/lib/jquery/dist/jquery.min.js")]
        [DataRow("/lib/jquery-validation/dist/jquery.validate.min.js")]
        [DataRow("/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js")]
        [DataRow("/js/community.js")]
        public async Task RequiredStaticAssetsAreAvailable(string path)
        {
            using var client = CreateClient();
            using var response = await client.GetAsync(path);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
            Assert.IsTrue(response.Content.Headers.ContentLength > 0, path);
        }

        [TestMethod]
        public async Task MatchHistoryRendersProfessionalAndCommunityResults()
        {
            using var client = CreateClient();
            using var response = await client.GetAsync("/Matches/History");
            var html = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            StringAssert.Contains(html, "Professional results");
            StringAssert.Contains(html, "Vitality");
            StringAssert.Contains(html, "Winner");
            StringAssert.Contains(html, "solo-player-one");
            StringAssert.Contains(html, "Team Alpha");
            StringAssert.Contains(html, "C5GO Championship");
        }

        [TestMethod]
        public async Task FinishedMatchDetailsShowWinnerWhenScoreIsUnavailable()
        {
            using var client = CreateClient();
            using var response = await client.GetAsync("/Matches/Details?id=5&history=true");
            var html = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            StringAssert.Contains(html, "Back to history");
            StringAssert.Contains(html, "Winner");
            StringAssert.Contains(html, "MOUZ");
        }

        [TestMethod]
        public async Task AdminPageRequiresAuthentication()
        {
            using var client = CreateClient();
            using var response = await client.GetAsync("/Admin");

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("/Login", response.Headers.Location?.AbsolutePath);
            StringAssert.Contains(response.Headers.Location?.Query, "ReturnUrl=%2FAdmin");
        }

        [TestMethod]
        public async Task DisabledCommunityIsNotPubliclyExposed()
        {
            using var client = CreateClient();
            using var response = await client.GetAsync("/Community");

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("/errors/404", response.Headers.Location?.OriginalString);
        }

        [TestMethod]
        public async Task CommunityFeedRendersWhenEnabled()
        {
            var communityRepo = new MockCommunityRepo();
            communityRepo.Discussions.Add(new Discussion
            {
                Id = 1,
                AuthorId = 1,
                CategoryId = 1,
                Title = "Useful community discussion",
                Content = "A helpful topic for the community.",
                Status = CommunityContentStatus.Published,
                CreatedAt = DateTime.UtcNow,
                Author = new LogicLayer.Models.User(1) { Username = "member" },
                Category = communityRepo.Categories[0]
            });

            using var communityFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Features:CommunityEnabled"] = "true"
                    }));
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<ICommunityRepo>();
                    services.AddSingleton<ICommunityRepo>(communityRepo);
                });
            });
            using var client = communityFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://c5g0.com")
            });

            using var response = await client.GetAsync("/Community");
            var html = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            StringAssert.Contains(html, "Useful community discussion");
            StringAssert.Contains(html, "Log in to post");
        }

        [TestMethod]
        public async Task UnknownPageRedirectsToCustomNotFoundPage()
        {
            using var client = CreateClient();
            using var response = await client.GetAsync("/page-that-does-not-exist");

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual("/errors/404", response.Headers.Location?.OriginalString);
        }

        private HttpClient CreateClient()
        {
            return factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://c5g0.com")
            });
        }

        private static List<Match> CreatePlayerHistoryMatches()
        {
            var playerOne = new Player(
                1,
                "Solo",
                "One",
                20,
                "solo-player-one",
                "one@c5go.local",
                string.Empty,
                "76561198000000001",
                false);
            var playerTwo = new Player(
                2,
                "Solo",
                "Two",
                21,
                "solo-player-two",
                "two@c5go.local",
                string.Empty,
                "76561198000000002",
                false);

            return
            [
                new Match(
                    1,
                    1,
                    playerOne,
                    playerTwo,
                    13,
                    9,
                    DateTime.Now.AddDays(-1),
                    Status.Closed)
                {
                    TournamentName = "C5GO Championship"
                }
            ];
        }

        private static List<TeamMatch> CreateTeamHistoryMatches()
        {
            return
            [
                new TeamMatch(
                    1,
                    1,
                    new Team(1, "Team Alpha", null),
                    new Team(2, "Team Bravo", null),
                    2,
                    1,
                    DateTime.Now.AddDays(-1),
                    Status.Closed)
                {
                    TournamentName = "C5GO Championship"
                }
            ];
        }

        private static void AssertHeader(
            HttpResponseMessage response,
            string headerName,
            string expectedValue)
        {
            Assert.IsTrue(response.Headers.TryGetValues(headerName, out var values), headerName);
            CollectionAssert.Contains(values.ToList(), expectedValue);
        }

        private static void AssertHeaderContains(
            HttpResponseMessage response,
            string headerName,
            string expectedPart)
        {
            Assert.IsTrue(response.Headers.TryGetValues(headerName, out var values), headerName);
            StringAssert.Contains(string.Join(" ", values), expectedPart);
        }

        private sealed class InMemoryXmlRepository : IXmlRepository
        {
            private readonly List<XElement> elements = [];

            public IReadOnlyCollection<XElement> GetAllElements()
            {
                lock (elements)
                {
                    return elements.Select(element => new XElement(element)).ToList().AsReadOnly();
                }
            }

            public void StoreElement(XElement element, string friendlyName)
            {
                lock (elements)
                {
                    elements.Add(new XElement(element));
                }
            }
        }
    }
}
