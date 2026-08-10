using LogicLayer.Services;

namespace Unit_Tests
{
    [TestClass]
    public class TestPostContentParser
    {
        [TestMethod]
        [DataRow("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        [DataRow("https://youtu.be/dQw4w9WgXcQ?t=10", "dQw4w9WgXcQ")]
        [DataRow("https://youtube.com/shorts/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        [DataRow("https://www.youtube.com/embed/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        [DataRow("https://youtube.com/live/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
        public void ParsesSupportedYouTubeLinks(string url, string expectedVideoId)
        {
            var result = PostContentParser.TryGetYouTubeVideoId(url, out var videoId);

            Assert.IsTrue(result);
            Assert.AreEqual(expectedVideoId, videoId);
        }

        [TestMethod]
        [DataRow("https://youtube.com.evil.test/watch?v=dQw4w9WgXcQ")]
        [DataRow("https://example.com/watch?v=dQw4w9WgXcQ")]
        [DataRow("javascript:alert(1)")]
        [DataRow("https://youtube.com/watch?v=too-short")]
        public void RejectsUnsupportedOrUnsafeLinks(string url)
        {
            Assert.IsFalse(PostContentParser.TryGetYouTubeVideoId(url, out _));
        }

        [TestMethod]
        public void ReplacesVideoUrlWithVideoBlockAndKeepsText()
        {
            var blocks = PostContentParser.Parse(
                "Match highlights https://youtu.be/dQw4w9WgXcQ after the final.");

            Assert.AreEqual(3, blocks.Count);
            Assert.AreEqual("Match highlights ", blocks[0].Text);
            Assert.AreEqual("dQw4w9WgXcQ", blocks[1].VideoId);
            Assert.AreEqual(" after the final.", blocks[2].Text);
            Assert.IsFalse(blocks.Any(block => block.Text.Contains("youtu.be", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void CreatesReadablePreviewWithoutVideoUrl()
        {
            var preview = PostContentParser.GetPreviewText(
                "Watch now: https://youtube.com/watch?v=dQw4w9WgXcQ");

            Assert.AreEqual("Watch now:", preview);
        }

        [TestMethod]
        public void RemovesLegacyHtmlFromDisplayedText()
        {
            var blocks = PostContentParser.Parse("<p>First &amp; second</p><script>alert(1)</script>");
            var text = string.Concat(blocks.Select(block => block.Text));

            Assert.AreEqual("First & second\nalert(1)", text);
            Assert.IsFalse(text.Contains("<script>", StringComparison.OrdinalIgnoreCase));
        }
    }
}
