using Microsoft.AspNetCore.DataProtection;
using Website.Services;

namespace Unit_Tests
{
    [TestClass]
    public class TestPasswordResetTokenService
    {
        private static PasswordResetTokenService CreateService()
        {
            return new PasswordResetTokenService(
                new EphemeralDataProtectionProvider());
        }

        [TestMethod]
        public void TestCreateAndReadPasswordResetToken()
        {
            var service = CreateService();

            var token = service.CreateToken(42, "$2a$example-password-hash");
            var payload = service.ReadToken(token);

            Assert.IsNotNull(payload);
            Assert.AreEqual(42, payload.UserId);
            Assert.AreEqual("$2a$example-password-hash", payload.CurrentPasswordHash);
        }

        [TestMethod]
        public void TestTamperedPasswordResetTokenIsRejected()
        {
            var service = CreateService();
            var token = service.CreateToken(42, "$2a$example-password-hash");
            var lastCharacter = token[^1] == 'A' ? 'B' : 'A';
            var tamperedToken = token[..^1] + lastCharacter;

            Assert.IsNull(service.ReadToken(tamperedToken));
        }

        [TestMethod]
        public void TestTokenCannotBeReadByDifferentProtector()
        {
            var firstService = CreateService();
            var secondService = CreateService();
            var token = firstService.CreateToken(42, "$2a$example-password-hash");

            Assert.IsNull(secondService.ReadToken(token));
        }
    }
}
