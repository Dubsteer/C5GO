using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;

namespace Website.Services
{
    public sealed class PasswordResetTokenService
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);
        private readonly ITimeLimitedDataProtector protector;

        public PasswordResetTokenService(IDataProtectionProvider dataProtectionProvider)
        {
            protector = dataProtectionProvider
                .CreateProtector("C5GO.PasswordReset.v1")
                .ToTimeLimitedDataProtector();
        }

        public string CreateToken(int userId, string currentPasswordHash)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(currentPasswordHash))
                throw new ArgumentException("A valid user and password hash are required.");

            var payload = JsonSerializer.Serialize(
                new PasswordResetTokenPayload(userId, currentPasswordHash));

            return protector.Protect(payload, TokenLifetime);
        }

        public PasswordResetTokenPayload? ReadToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var payload = JsonSerializer.Deserialize<PasswordResetTokenPayload>(
                    protector.Unprotect(token));

                return payload is { UserId: > 0 } &&
                       !string.IsNullOrWhiteSpace(payload.CurrentPasswordHash)
                    ? payload
                    : null;
            }
            catch (CryptographicException)
            {
                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    public sealed record PasswordResetTokenPayload(int UserId, string CurrentPasswordHash);
}
