namespace LogicLayer.Services
{
    public static class SteamIdParser
    {
        private const string ProfileHost = "steamcommunity.com";

        public static bool TryNormalize(string? value, out string? steamId)
        {
            steamId = null;

            if (string.IsNullOrWhiteSpace(value) || value.Trim() == "0")
                return true;

            var candidate = value.Trim();
            if (IsSteamId64(candidate))
            {
                steamId = candidate;
                return true;
            }

            if (!Uri.TryCreate(candidate, UriKind.Absolute, out var profileUri) ||
                (profileUri.Scheme != Uri.UriSchemeHttps && profileUri.Scheme != Uri.UriSchemeHttp) ||
                !string.IsNullOrEmpty(profileUri.UserInfo) ||
                !IsSteamCommunityHost(profileUri.Host))
            {
                return false;
            }

            var segments = profileUri.AbsolutePath
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length != 2 ||
                !segments[0].Equals("profiles", StringComparison.OrdinalIgnoreCase) ||
                !IsSteamId64(segments[1]))
            {
                return false;
            }

            steamId = segments[1];
            return true;
        }

        public static string BuildProfileUrl(string steamId)
        {
            if (!IsSteamId64(steamId))
                throw new ArgumentException("A valid SteamID64 is required.", nameof(steamId));

            return $"https://{ProfileHost}/profiles/{steamId}";
        }

        private static bool IsSteamCommunityHost(string host)
        {
            return host.Equals(ProfileHost, StringComparison.OrdinalIgnoreCase) ||
                   host.Equals($"www.{ProfileHost}", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSteamId64(string value)
        {
            return value.Length == 17 &&
                   value.StartsWith("7656119", StringComparison.Ordinal) &&
                   value.All(char.IsAsciiDigit);
        }
    }
}
