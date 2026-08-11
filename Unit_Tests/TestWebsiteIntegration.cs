using System.Net;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
                            ["AppSettings:AuthUrl"] = "https://localhost"
                        });
                    });
                    builder.ConfigureServices(services =>
                    {
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
            AssertHeaderContains(response, "Strict-Transport-Security", "max-age=");
            Assert.IsFalse(response.Headers.Contains("Server"));
        }

        [TestMethod]
        [DataRow("/Login")]
        [DataRow("/Register")]
        [DataRow("/ForgotPassword")]
        [DataRow("/RegisterSuccess")]
        [DataRow("/ForgotPasswordConfirmation")]
        [DataRow("/ResetPasswordConfirmation")]
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
        public async Task RequiredStaticAssetsAreAvailable(string path)
        {
            using var client = CreateClient();
            using var response = await client.GetAsync(path);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, path);
            Assert.IsTrue(response.Content.Headers.ContentLength > 0, path);
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
