using LogicLayer.Services;

namespace Unit_Tests
{
    [TestClass]
    public class TestSteamIdParser
    {
        private const string SteamId = "76561198012345678";

        [TestMethod]
        public void TestAcceptsSteamId64()
        {
            var valid = SteamIdParser.TryNormalize(SteamId, out var normalized);

            Assert.IsTrue(valid);
            Assert.AreEqual(SteamId, normalized);
        }

        [TestMethod]
        public void TestExtractsSteamId64FromProfileUrl()
        {
            var valid = SteamIdParser.TryNormalize(
                $"https://steamcommunity.com/profiles/{SteamId}/",
                out var normalized);

            Assert.IsTrue(valid);
            Assert.AreEqual(SteamId, normalized);
        }

        [TestMethod]
        public void TestRejectsVanityProfileUrl()
        {
            Assert.IsFalse(SteamIdParser.TryNormalize(
                "https://steamcommunity.com/id/example-user",
                out _));
        }

        [TestMethod]
        public void TestRejectsInvalidOrUntrustedProfileValue()
        {
            Assert.IsFalse(SteamIdParser.TryNormalize("steam123", out _));
            Assert.IsFalse(SteamIdParser.TryNormalize(
                $"https://example.com/profiles/{SteamId}",
                out _));
        }

        [TestMethod]
        public void TestEmptySteamProfileIsOptional()
        {
            Assert.IsTrue(SteamIdParser.TryNormalize(" ", out var normalized));
            Assert.IsNull(normalized);
        }

        [TestMethod]
        public void TestBuildsCanonicalSteamProfileUrl()
        {
            Assert.AreEqual(
                $"https://steamcommunity.com/profiles/{SteamId}",
                SteamIdParser.BuildProfileUrl(SteamId));
        }
    }
}
