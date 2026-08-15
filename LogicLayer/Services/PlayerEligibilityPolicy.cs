using LogicLayer.Models;

namespace LogicLayer.Services
{
    public static class PlayerEligibilityPolicy
    {
        public static bool HasValidSteamId(User? user)
        {
            return user != null &&
                   SteamIdParser.TryNormalize(user.SteamId, out var steamId) &&
                   !string.IsNullOrWhiteSpace(steamId);
        }
    }
}
