using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using LogicLayer.FormModels;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Website.Configuration;
using Website.Services;

namespace Unit_Tests
{
    [TestClass]
    public sealed class TestRegistrationSecurity
    {
        [TestMethod]
        public void ConfirmationMustMatchPassword()
        {
            var form = CreateValidForm();
            form.ConfirmPassword = "different-password";
            var results = Validate(form);

            Assert.IsTrue(results.Any(result =>
                result.MemberNames.Contains(nameof(FullUserFormModel.ConfirmPassword)) &&
                result.ErrorMessage == "Passwords do not match"));
        }

        [TestMethod]
        public void MatchingPasswordConfirmationIsValid()
        {
            var results = Validate(CreateValidForm());

            Assert.IsFalse(results.Any(result =>
                result.MemberNames.Contains(nameof(FullUserFormModel.ConfirmPassword))));
        }

        [TestMethod]
        public async Task TurnstileAcceptsSuccessfulServerValidation()
        {
            var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"success\":true}", Encoding.UTF8, "application/json")
            });
            var service = CreateTurnstileService(handler);

            var result = await service.ValidateAsync("valid-token", "127.0.0.1");

            Assert.IsTrue(result);
            Assert.AreEqual(1, handler.RequestCount);
            StringAssert.Contains(handler.LastRequestBody, "secret=test-secret");
            StringAssert.Contains(handler.LastRequestBody, "response=valid-token");
            StringAssert.Contains(handler.LastRequestBody, "remoteip=127.0.0.1");
        }

        [TestMethod]
        public async Task TurnstileRejectsFailedServerValidation()
        {
            var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"success\":false}", Encoding.UTF8, "application/json")
            });
            var service = CreateTurnstileService(handler);

            Assert.IsFalse(await service.ValidateAsync("invalid-token", null));
        }

        [TestMethod]
        public async Task TurnstileRejectsMissingTokenWithoutCallingCloudflare()
        {
            var handler = new StubHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK));
            var service = CreateTurnstileService(handler);

            Assert.IsFalse(await service.ValidateAsync(null, null));
            Assert.AreEqual(0, handler.RequestCount);
        }

        [TestMethod]
        public async Task TurnstileFailsClosedWhenCloudflareIsUnavailable()
        {
            var handler = new StubHttpMessageHandler(_ =>
                throw new HttpRequestException("unavailable"));
            var service = CreateTurnstileService(handler);

            Assert.IsFalse(await service.ValidateAsync("token", null));
        }

        private static FullUserFormModel CreateValidForm()
        {
            return new FullUserFormModel
            {
                Firstname = "Test",
                Lastname = "User",
                Age = 20,
                Username = "test-user",
                Gmail = "test@example.com",
                Password = "secure-password",
                ConfirmPassword = "secure-password"
            };
        }

        private static List<ValidationResult> Validate(FullUserFormModel form)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(
                form,
                new ValidationContext(form),
                results,
                validateAllProperties: true);
            return results;
        }

        private static TurnstileService CreateTurnstileService(HttpMessageHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://challenges.cloudflare.com/turnstile/v0/")
            };
            var options = Options.Create(new TurnstileOptions
            {
                SiteKey = "test-site-key",
                SecretKey = "test-secret"
            });

            return new TurnstileService(
                client,
                options,
                NullLogger<TurnstileService>.Instance);
        }

        private sealed class StubHttpMessageHandler(
            Func<HttpRequestMessage, HttpResponseMessage> responseFactory) : HttpMessageHandler
        {
            public int RequestCount { get; private set; }
            public string LastRequestBody { get; private set; } = string.Empty;

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                RequestCount++;
                LastRequestBody = request.Content is null
                    ? string.Empty
                    : await request.Content.ReadAsStringAsync(cancellationToken);
                return responseFactory(request);
            }
        }
    }
}
